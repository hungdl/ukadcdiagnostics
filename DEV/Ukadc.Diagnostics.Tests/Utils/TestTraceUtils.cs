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
using Ukadc.Diagnostics.Utils;

namespace Ukadc.Diagnostics.Tests.Utils
{
    [TestClass]
    public class TestTraceUtils
    {
        [TestMethod]
        public void ConstructProvidedSourceFilter()
        {
            // test backwards compatibility with no need to specify full type name...
            SourceFilter filter = (SourceFilter)TraceUtils.GetRuntimeObject(
                "System.Diagnostics.SourceFilter",
                typeof(TraceFilter),
                "MySource");

            Assert.AreEqual("MySource", filter.Source);
        }

        [TestMethod]
        public void ConstructProvidedEventTypeFilter()
        {
            EventTypeFilter filter = (EventTypeFilter)TraceUtils.GetRuntimeObject(
                "System.Diagnostics.EventTypeFilter, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                typeof(TraceFilter),
                "Warning");

            Assert.AreEqual(SourceLevels.Warning, filter.EventType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullClassName()
        {
            TraceUtils.GetRuntimeObject(null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestEmptyClassName()
        {
            TraceUtils.GetRuntimeObject("", null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullBaseType()
        {
            TraceUtils.GetRuntimeObject("bob", null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TestIncompatibleTypes()
        {
            TraceUtils.GetRuntimeObject("System.String", this.GetType(), "");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TestGetInvalidClassname()
        {
            TraceUtils.GetRuntimeObject("This Type Does Not Exist", this.GetType(), "");
        }

        [TestMethod]
        public void TestValidRuntimeObject()
        {
            // Test I can create an object with a default ctor
            object o = TraceUtils.GetRuntimeObject("Ukadc.Diagnostics.Tests.Utils.TestWithNoCtorArguments, Ukadc.Diagnostics.Tests",
                typeof(BaseForTests), null);
            Assert.IsNotNull(o, "The object was not created by GetRuntimeObject");
            Assert.IsInstanceOfType(o, typeof(TestWithNoCtorArguments), "The created object is not an instance of TestWithNoCtorArguments");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TestGetRuntimeObjectNoValidCtor()
        {
            // Test that it blows up when I pass an object that has a ctor with a string argument, but I've not passed in a string
            object o = TraceUtils.GetRuntimeObject("Ukadc.Diagnostics.Tests.Utils.TestWithCtorArguments, Ukadc.Diagnostics.Tests",
                typeof(BaseForTests), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TestUnexpectedCtorArgument()
        {
            // Test that it blows if there is a default ctor but I've passed in an argument
            object o = TraceUtils.GetRuntimeObject("Ukadc.Diagnostics.Tests.Utils.TestWithNoCtorArguments, Ukadc.Diagnostics.Tests",
                typeof(BaseForTests), "This is an error");
        }

        [TestMethod]
        public void TestWithEnumArg()
        {
            object o = TraceUtils.GetRuntimeObject("Ukadc.Diagnostics.Tests.Utils.TestWithEnumArgument, Ukadc.Diagnostics.Tests",
                typeof(BaseForTests), "All");

            Assert.IsNotNull(o, "Expected to have a constructed object but got nothing");
            Assert.IsInstanceOfType(o, typeof(TestWithEnumArgument), "Expected an instance of TestWithEnumArgument");
            TestWithEnumArgument twea = o as TestWithEnumArgument;

            Assert.AreEqual<SourceLevels>(SourceLevels.All, twea.Levels, "Expected SourceLevels.All");
        }

        [TestMethod]
        public void TestWithIntArg()
        {
            object o = TraceUtils.GetRuntimeObject("Ukadc.Diagnostics.Tests.Utils.TestWithIntArgument, Ukadc.Diagnostics.Tests",
                typeof(BaseForTests), "123");

            Assert.IsNotNull(o, "Expected to have a constructed object but got nothing");
            Assert.IsInstanceOfType(o, typeof(TestWithIntArgument), "Expected an instance of TestWithIntArgument");
            TestWithIntArgument twia = o as TestWithIntArgument;

            Assert.AreEqual<int>(123, twia.Something, "Expected 123");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TestWithNoChangeOfWorking()
        {
            object o = TraceUtils.GetRuntimeObject("Ukadc.Diagnostics.Tests.Utils.TestWithEnumArgument, Ukadc.Diagnostics.Tests",
                typeof(BaseForTests), "Fail Please");
        }

    }

    public class BaseForTests
    {
    }

    public class TestWithCtorArguments : BaseForTests
    {
        public TestWithCtorArguments(string arg)
        {
        }
    }

    public class TestWithNoCtorArguments : BaseForTests
    {
    }

    public class TestWithEnumArgument : BaseForTests
    {
        public TestWithEnumArgument(SourceLevels levels)
        {
            Levels = levels;
        }

        public SourceLevels Levels { get; set; }
    }

    public class TestWithIntArgument : BaseForTests
    {
        public TestWithIntArgument(int something)
        {
            Something = something;
        }

        public int Something { get; set; }
    }

}
