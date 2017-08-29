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
using Ukadc.Diagnostics.Filters.Configuration;

namespace Ukadc.Diagnostics.Tests.Listeners
{
    [TestClass]
    public class TestMultiFilter
    {
        private const string CORRECT_SOURCE = "correct source";
        private const string INCORRECT_SOURCE = "incorrect source";

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestArgumentValidation()
        {
            new MultiFilter("");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException), "MultiFilter configuration error: No filterGroup with the name 'unknownGroup' could be found")]
        public void TestNoGroupFound_NoConfig()
        {
            MultiFilter filter = new MultiFilter("unkownGroup");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TestNoFilters_NoConfig()
        {
            MultiFilter filter = new MultiFilter(new MultiFilterGroup());
        }

        [TestMethod]
        public void TestSingle_NoConfig()
        {
            MultiFilterGroup group = new MultiFilterGroup();
            group.Add(new SourceFilter(CORRECT_SOURCE));
            MultiFilter filter = new MultiFilter(group);

            Assert.IsTrue(filter.ShouldTrace(null, CORRECT_SOURCE, TraceEventType.Critical, 0, null, null, null, null));
            Assert.IsFalse(filter.ShouldTrace(null, INCORRECT_SOURCE, TraceEventType.Critical, 0, null, null, null, null));
        }

        [TestMethod]
        public void TestSingleNegated_NoConfig()
        {
            MultiFilterGroup group = new MultiFilterGroup();
            // enable negation at the FilterMember
            group.Add(new SourceFilter(CORRECT_SOURCE), true);
            MultiFilter filter = new MultiFilter(group);

            Assert.IsFalse(filter.ShouldTrace(null, CORRECT_SOURCE, TraceEventType.Critical, 0, null, null, null, null));
            Assert.IsTrue(filter.ShouldTrace(null, INCORRECT_SOURCE, TraceEventType.Critical, 0, null, null, null, null));
        }


        [TestMethod]
        public void TestAnd_NoConfig()
        {
            MultiFilterGroup group = new MultiFilterGroup();
            group.Logic = FilterGroupLogic.And;
            group.Add(new EventTypeFilter(SourceLevels.Critical));
            group.Add(new SourceFilter(CORRECT_SOURCE));
            MultiFilter filter = new MultiFilter(group);

            Assert.IsTrue(filter.ShouldTrace(null, CORRECT_SOURCE, TraceEventType.Critical, 0, null, null, null, null));
            Assert.IsFalse(filter.ShouldTrace(null, INCORRECT_SOURCE, TraceEventType.Critical, 0, null, null, null, null));
            Assert.IsFalse(filter.ShouldTrace(null, CORRECT_SOURCE, TraceEventType.Error, 0, null, null, null, null));
            Assert.IsFalse(filter.ShouldTrace(null, INCORRECT_SOURCE, TraceEventType.Error, 0, null, null, null, null));
        }

        [TestMethod]
        public void TestOr_NoConfig()
        {
            MultiFilterGroup group = new MultiFilterGroup();
            group.Logic = FilterGroupLogic.Or;
            group.Add(new EventTypeFilter(SourceLevels.Critical));
            group.Add(new SourceFilter(CORRECT_SOURCE));
            MultiFilter filter = new MultiFilter(group);

            Assert.IsTrue(filter.ShouldTrace(null, CORRECT_SOURCE, TraceEventType.Critical, 0, null, null, null, null));
            Assert.IsTrue(filter.ShouldTrace(null, INCORRECT_SOURCE, TraceEventType.Critical, 0, null, null, null, null));
            Assert.IsTrue(filter.ShouldTrace(null, CORRECT_SOURCE, TraceEventType.Error, 0, null, null, null, null));
            Assert.IsFalse(filter.ShouldTrace(null, INCORRECT_SOURCE, TraceEventType.Error, 0, null, null, null, null));
        }

        [TestMethod]
        public void TestMultiFilter_WithConfig()
        {
            TraceSource source1 = new TraceSource("source1");
            TraceSource source2 = new TraceSource("source2");
            InMemoryTraceListener.TraceObjects.Clear();

            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);

            source1.TraceData(TraceEventType.Critical, 0, null);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);

            source1.TraceData(TraceEventType.Error, 0, null);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
            
            source2.TraceData(TraceEventType.Critical, 0, null);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);

        }

        [TestMethod]
        public void TestNegatedFilter_WithConfig()
        {
            TraceSource source = new TraceSource("source3");
            InMemoryTraceListener.TraceObjects.Clear();

            source.TraceData(TraceEventType.Critical, 0, null);
            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);

            source.TraceData(TraceEventType.Error, 0, null);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestMultiFilterGroupAddNull()
        {
            MultiFilterGroup group = new MultiFilterGroup();
            group.Add(null, false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestMultiFilterGroupNullFilterGroupElement()
        {
            new MultiFilterGroup(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestMultiFilterMemberNullTraceFilter()
        {
            new MultiFilterMember(null, true);
        }
    }
}
