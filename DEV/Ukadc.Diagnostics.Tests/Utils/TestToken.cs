using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ukadc.Diagnostics.Utils;

namespace Ukadc.Diagnostics.Tests.Utils
{
    [TestClass]
    public class TestToken
    {
        [TestMethod]
        public void TestSimpleToken()
        {
            Token token = new Token("{Hello}");
            Assert.AreEqual("Hello", token.TokenName);
            Assert.IsNull(token.SubProperty);
            Assert.IsNull(token.FormatString);
            Assert.AreEqual("{Hello}", token.OriginalString);
        }

        [TestMethod]
        public void TestFormattedToken()
        {
            Token token = new Token("{Hello:Goodbye}");
            Assert.AreEqual("Hello", token.TokenName);
            Assert.AreEqual("Goodbye", token.FormatString);
            Assert.IsNull(token.SubProperty);
            Assert.AreEqual("{Hello:Goodbye}", token.OriginalString);
        }

        [TestMethod]
        public void TestComplexFormattedToken()
        {
            Token token = new Token("{Hello:Goodbye:..#@$%}");
            Assert.AreEqual("Hello", token.TokenName);
            Assert.AreEqual("Goodbye:..#@$%", token.FormatString);
            Assert.IsNull(token.SubProperty);
            Assert.AreEqual("{Hello:Goodbye:..#@$%}", token.OriginalString);
        }

        [TestMethod]
        public void TestSubPropertyName()
        {
            Token token = new Token("{Hello.Prop}");
            Assert.AreEqual("Hello", token.TokenName);
            Assert.AreEqual("Prop", token.SubProperty);
            Assert.IsNull(token.FormatString);
            Assert.AreEqual("{Hello.Prop}", token.OriginalString);
        }

    }
}
