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
    public class TestStringComparator
    {
        private StringComparator _stringComparator = new StringComparator();

        [TestMethod]
        public void TestEquals()
        {
            Assert.IsTrue(
                _stringComparator.Compare(Operation.Equals, "hello", "hello"));

            Assert.IsFalse(
                _stringComparator.Compare(Operation.Equals, "hello", "goodbye"));
        }

        [TestMethod]
        public void TestNotEquals()
        {
            Assert.IsTrue(
                _stringComparator.Compare(Operation.NotEquals, "hello", "goodbye"));

            Assert.IsFalse(
                _stringComparator.Compare(Operation.NotEquals, "hello", "hello"));
        }

        [TestMethod]
        public void TestIsNull()
        {
            Assert.IsTrue(
                _stringComparator.Compare(Operation.IsNull, null, "goodbye"));

            Assert.IsFalse(
                _stringComparator.Compare(Operation.IsNull, "hello", "hello"));
        }

        [TestMethod]
        public void TestIsNotNull()
        {
            Assert.IsTrue(
                _stringComparator.Compare(Operation.IsNotNull, "hello", "goodbye"));

            Assert.IsFalse(
                _stringComparator.Compare(Operation.IsNotNull, null, "hello"));
        }

        [TestMethod]
        public void TestStartsWith()
        {
            Assert.IsTrue(
                _stringComparator.Compare(Operation.StartsWith, "hello billy", "hello"));

            Assert.IsFalse(
                _stringComparator.Compare(Operation.StartsWith, "hello billy", "goodbye"));

            Assert.IsFalse(_stringComparator.Compare(Operation.StartsWith, null, null));
            Assert.IsFalse(_stringComparator.Compare(Operation.StartsWith, null, "bob"));
            Assert.IsFalse(_stringComparator.Compare(Operation.StartsWith, "bob", null));
        }

        [TestMethod]
        public void TestEndsWith()
        {
            Assert.IsTrue(
                _stringComparator.Compare(Operation.EndsWith, "hello billy", "billy"));

            Assert.IsFalse(
                _stringComparator.Compare(Operation.EndsWith, "hello billy", "hello"));

            Assert.IsFalse(_stringComparator.Compare(Operation.EndsWith, null, null));
            Assert.IsFalse(_stringComparator.Compare(Operation.EndsWith, null, "bob"));
            Assert.IsFalse(_stringComparator.Compare(Operation.EndsWith, "bob", null));
        }

        [TestMethod]
        public void TestContains()
        {
            Assert.IsTrue(
                _stringComparator.Compare(Operation.Contains, "hello billy", "o b"));

            Assert.IsFalse(
                _stringComparator.Compare(Operation.Contains, "hello billy", "goodbye"));
        }

        [TestMethod]
        public void TestContainsWithNulls()
        {
            // a null string can't *contain* anything
            Assert.IsFalse(
                 _stringComparator.Compare(Operation.Contains, null, null));

            Assert.IsFalse(
                _stringComparator.Compare(Operation.Contains, "hello billy", null));
        }

        [TestMethod]
        public void TestRegexMatch()
        {
            Assert.IsTrue(_stringComparator.Compare(Operation.RegexMatch, "This is a simple test", "test"));
            Assert.IsFalse(_stringComparator.Compare(Operation.RegexMatch, "This is a simple test", "not here"));
            Assert.IsTrue(_stringComparator.Compare(Operation.RegexMatch, "This is a simple test", "t..t"));
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestInvalidOperation()
        {
            _stringComparator.Compare(Operation.LessThan, "", "");
        }

        [TestMethod]
        public void TestStringComparitorOperations()
        {
            Assert.AreEqual<Operations>(Operations.Equals | Operations.NotEquals | Operations.Contains | Operations.StartsWith |
                                        Operations.EndsWith | Operations.IsNotNull | Operations.IsNull | Operations.RegexMatch,
                                        _stringComparator.SupportedOperations, "The list of supported operations appears to be wrong");
        }
    }
}
