// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Ukadc.Diagnostics.Listeners;
using Ukadc.Diagnostics.Filters;
using System.Configuration;
using System.Reflection;
using Ukadc.Diagnostics.Utils.PropertyReaders;
using Rhino.Mocks;
using System.Data;
using System.IO;
using System.Data.SqlClient;

namespace Ukadc.Diagnostics.Tests.Listeners
{
    [TestClass]
    public class TestSqlTraceListener
    {
        private TraceSource _traceSource;

        [TestInitialize]
        public void Setup()
        {
            _traceSource = new TraceSource("Test Source", SourceLevels.All);
        }

        [TestMethod]
        public void LogDataUsingMockDataAccess()
        {
            MockRepository mocks = new MockRepository();
            IDataAccessAdapter mockAdapter = mocks.CreateMock<IDataAccessAdapter>();
            IDataAccessCommand mockCommand = mocks.CreateMock<IDataAccessCommand>();

            SqlTraceListener listener = new SqlTraceListener(mockAdapter);
            listener.Parameters.Add(new SqlTraceParameter("@level", new EventTypePropertyReader(), false));
            listener.Parameters.Add(new SqlTraceParameter("@eventType", new EventTypePropertyReader(), true));
            listener.Parameters.Add(new SqlTraceParameter("@message", new MessagePropertyReader(), false));
            listener.Parameters.Add(new SqlTraceParameter("@relatedActivityId", new RelatedActivityIdPropertyReader(), false));
            listener.Parameters.Add(new SqlTraceParameter("@activityId", new ActivityIdPropertyReader(), false));

            _traceSource.Listeners.Add(listener);

            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();

            // make mock assertions
            
            using (mocks.Record())
            using (mocks.Ordered())
            {
                Expect.Call(mockAdapter.CreateCommand()).Return(mockCommand);

                mockCommand.AddParameter("@level", TraceEventType.Critical);
                mockCommand.AddParameter("@eventType", "Critical");
                mockCommand.AddParameter("@message", null);
                mockCommand.AddParameter("@relatedActivityId", null);
                mockCommand.AddParameter("@activityId", Guid.Empty);
                mockCommand.Execute();
                mockCommand.Dispose();

                Expect.Call(mockAdapter.CreateCommand()).Return(mockCommand);

                mockCommand.AddParameter("@level", TraceEventType.Transfer);
                mockCommand.AddParameter("@eventType", "Transfer");
                mockCommand.AddParameter("@message", "boo, relatedActivityId=" + guid1.ToString());
                mockCommand.AddParameter("@relatedActivityId", guid1);
                mockCommand.AddParameter("@activityId", guid2);
                mockCommand.Execute();
                mockCommand.Dispose();
            }

            // make it all happen!
            mocks.ReplayAll(); 
            _traceSource.TraceData(TraceEventType.Critical, 1, null);
            Trace.CorrelationManager.ActivityId = guid2;
            _traceSource.TraceTransfer(1, "boo", guid1 );

            //verify!
            mocks.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TestNoMatchingConfigSection()
        {
            new SqlTraceListener("Nonsense");
        }


        // only run in Debug, can't use [Conditional] as called via reflection
        [TestMethod]
        [Ignore]
        [Conditional("DEBUG")] // only run for Debug
        public void TestAgainstSqlExpressDatabase()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            DatabaseHelper.CreateDatabaseIfNotExists("TestAX307", path);
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
                            
            Guid activityId = Guid.NewGuid();

            Trace.CorrelationManager.ActivityId = activityId;

            try
            {
                // Create table in database.
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connString2"].ConnectionString))
                {
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = @"
IF EXISTS(SELECT 1 FROM sys.objects WHERE OBJECT_ID = OBJECT_ID(N'TestLog') AND type = (N'U')) DROP TABLE TestLog;
CREATE TABLE TestLog (ActivityId uniqueidentifier NOT NULL, Level int NOT NULL, EventType varchar(50) NOT NULL, DynamicColumn int)";
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    TraceSource source = new TraceSource("source12");
                    source.TraceInformation("hello");
                    // blow the activityid so next event logs with something else
                    Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                    source.TraceData(TraceEventType.Critical, 0, TimeSpan.FromMilliseconds(1111));

                    cmd = conn.CreateCommand();
                    cmd.CommandText = @"SELECT * FROM TestLog";
                    cmd.CommandType = CommandType.Text;

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        // read the first row
                        reader.Read();

                        Assert.AreEqual(activityId, reader["ActivityId"]);
                        Assert.AreEqual((int)TraceEventType.Information, reader["Level"]);
                        Assert.AreEqual(TraceEventType.Information.ToString(), reader["EventType"]);
                        Assert.AreNotEqual(null, reader["DynamicColumn"]);

                        // read the second row
                        reader.Read();

                        Assert.AreNotEqual(activityId, reader["ActivityId"]);
                        Assert.AreEqual((int)TraceEventType.Critical, reader["Level"]);
                        Assert.AreEqual(TraceEventType.Critical.ToString(), reader["EventType"]);
                        Assert.AreEqual(1111, reader["DynamicColumn"]);

                        Assert.IsFalse(reader.Read(), "Too many rows returned");
                    }
                }
            }
            catch (Exception exc)
            {
                Trace.Write(exc);

                // If it errors lets try to clean up - might help the next run (for example, if the mdf files went missing)
                DatabaseHelper.DropDatabase("TestAX307");

                throw;
            }
        }

        [TestMethod]
        public void TestConfigBuildUp()
        {
            MockRepository mocks = new MockRepository();
            IDataAccessAdapter mockAdapter = mocks.CreateMock<IDataAccessAdapter>();
            IDataAccessCommand mockCommand = mocks.CreateMock<IDataAccessCommand>();

            TraceSource source = new TraceSource("source11");

            SqlTraceListener stl = (SqlTraceListener)source.Listeners["sqlTraceListener"];

            SqlDataAccessAdapter sdaa = (SqlDataAccessAdapter) stl.DataAccessAdapter;

            Assert.AreEqual("CONNECTION-STRING", sdaa.ConnectionString);
            Assert.AreEqual("COMMAND-TEXT", sdaa.CommandText);
            Assert.AreEqual(CommandType.Text, sdaa.CommandType);

            Assert.AreEqual(3, stl.Parameters.Count);
            Assert.AreEqual("@message", stl.Parameters[0].Name);
            Assert.AreEqual(typeof(IdPropertyReader), stl.Parameters[1].PropertyReader.GetType());
            Assert.AreEqual(typeof(DynamicPropertyReader), stl.Parameters[2].PropertyReader.GetType());
            
            DynamicPropertyReader dpr = stl.Parameters[2].PropertyReader as DynamicPropertyReader;
            Assert.IsNotNull(dpr, "Parameter PropertyReader was not of type DynamicPropertyReader");
            Assert.AreEqual(typeof(int), dpr.PropertyType);
        }

        [TestMethod]
        public void TestWithFilter()
        {
            MockRepository mocks = new MockRepository();
            IDataAccessAdapter mockAdapter = mocks.CreateMock<IDataAccessAdapter>();
            IDataAccessCommand mockCommand = mocks.CreateMock<IDataAccessCommand>();

            SqlTraceListener listener = new SqlTraceListener(mockAdapter);
            listener.Parameters.Add(new SqlTraceParameter("@level", new EventTypePropertyReader(), false));

            listener.Filter = new EventTypeFilter(SourceLevels.Critical);
            _traceSource.Listeners.Add(listener);

            // record expectations
            using (mocks.Record())
            using (mocks.Ordered())
            {
                Expect.Call(mockAdapter.CreateCommand()).Return(mockCommand);

                mockCommand.AddParameter("@level", TraceEventType.Critical);
                mockCommand.Execute();
                mockCommand.Dispose();
            }

            // make it all happen!
            mocks.ReplayAll();
            _traceSource.TraceData(TraceEventType.Critical, 1, "poo");
            _traceSource.TraceData(TraceEventType.Warning, 1, "boo");

            //verify!
            mocks.VerifyAll();

        }
    }
}
