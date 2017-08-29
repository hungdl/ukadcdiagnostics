using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ukadc.Diagnostics.Utils;
using System.ComponentModel;

namespace Ukadc.Diagnostics.Tests.Utils
{
    [TestClass]
    public class TestOperationConverter
    {
        [TestMethod]
        public void TestAllConversions()
        {
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(Operation));
            
            Assert.AreEqual(Operation.Equals, tc.ConvertFromString("="));
            Assert.AreEqual(Operation.GreaterThan, tc.ConvertFromString(">"));
            Assert.AreEqual(Operation.GreaterThanOrEqualTo, tc.ConvertFromString(">="));
            Assert.AreEqual(Operation.LessThan, tc.ConvertFromString("<"));
            Assert.AreEqual(Operation.LessThanOrEqualTo, tc.ConvertFromString("<="));
            Assert.AreEqual(Operation.NotEquals, tc.ConvertFromString("!="));
        }
    }
}
