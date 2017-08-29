// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ukadc.Diagnostics.Utils;

namespace Ukadc.Diagnostics.Tests.Utils
{
    [TestClass]
    public class TestRelatedActivityIdStore
    {
        [TestMethod]
        public void TestUsingBlock()
        {
            Guid value = Guid.NewGuid();
            Assert.IsFalse(RelatedActivityIdStore.TryGetRelatedActivityId(out value));
            Assert.AreEqual(Guid.Empty, value);
            Guid test = Guid.NewGuid();
            using (RelatedActivityIdStore.SetRelatedActivityId(test))
            {
                 Assert.IsTrue(RelatedActivityIdStore.TryGetRelatedActivityId(out value));
                 Assert.AreEqual(test, value);   
            }
            Assert.IsFalse(RelatedActivityIdStore.TryGetRelatedActivityId(out value));
            Assert.AreEqual(Guid.Empty, value);
        }


    }

}