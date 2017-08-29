// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ukadc.Diagnostics.Filters;
using Ukadc.Diagnostics.Utils;

namespace Ukadc.Diagnostics.Tests.Utils
{
    [TestClass]
    public class TestNumericComparator
    {
        private readonly NumericComparator _comparator = new NumericComparator();

        [TestMethod]
        public void TestEquals()
        {
            Assert.IsTrue(
                _comparator.Compare(Operation.Equals, 1, 1));

            Assert.IsFalse(
                _comparator.Compare(Operation.Equals, 1, 2));
        }

        [TestMethod]
        public void TestNotEquals()
        {
            Assert.IsTrue(
                _comparator.Compare(Operation.NotEquals, 1, 2));

            Assert.IsFalse(
                _comparator.Compare(Operation.NotEquals, 1, 1));
        }

        [TestMethod]
        public void TestGreaterThan()
        {
            Assert.IsTrue(
                _comparator.Compare(Operation.GreaterThan, 2, 1));

            Assert.IsFalse(
                _comparator.Compare(Operation.GreaterThan, 1, 1));
        }

        [TestMethod]
        public void TestGreaterThanOrEqualTo()
        {
            Assert.IsTrue(
                _comparator.Compare(Operation.GreaterThanOrEqualTo, 1, 1));

            Assert.IsTrue(
                _comparator.Compare(Operation.GreaterThanOrEqualTo, 2, 1));

            Assert.IsFalse(
                _comparator.Compare(Operation.GreaterThanOrEqualTo, 1, 2));
        }

        [TestMethod]
        public void TestLessThan()
        {
            Assert.IsTrue(
                _comparator.Compare(Operation.LessThan, 1, 2));

            Assert.IsFalse(
                _comparator.Compare(Operation.LessThan, 1, 1));
        }

        [TestMethod]
        public void TestLessThanOrEqualTo()
        {
            Assert.IsTrue(
                _comparator.Compare(Operation.LessThanOrEqualTo, 1, 1));

            Assert.IsTrue(
                _comparator.Compare(Operation.LessThanOrEqualTo, 1, 2));

            Assert.IsFalse(
                _comparator.Compare(Operation.LessThanOrEqualTo, 2, 1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),"The Type 'System.Object' does not implement IComparable and cannot be used by the 'Ukadc.Diagnostics.Utils.NumericComparator'.")]
        public void TestNotIComparable()
        {
            _comparator.Compare(Operation.Equals, new object(), new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestValue1Null()
        {
            _comparator.Compare(Operation.Equals, null, null);
        }

        [TestMethod]
        public void TestValue2Null()
        {
            Assert.IsTrue(
                _comparator.Compare(Operation.NotEquals, 1, null));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestInvalidOperation()
        {
            _comparator.Compare(Operation.IsNull, 1, 2);
        }
    }
}
