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
    public class FastPropertyGetterTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullPropertyName()
        {
            FastPropertyGetter fpg = new FastPropertyGetter(null, typeof(LogEvent));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestEmptyPropertyName()
        {
            FastPropertyGetter fpg = new FastPropertyGetter("", typeof(LogEvent));
        }

        [TestMethod]
        public void BasicCall_RefType()
        {
            FastPropertyGetter fpg = new FastPropertyGetter("ClassName", typeof(LogEvent));
            LogEvent le = new LogEvent("myclass");
            PropertyResult gr = fpg.GetValue(le);
            Assert.AreEqual(fpg.PropertyType, typeof(string));
            Assert.AreEqual("myclass", gr.Data);
            Assert.IsTrue(gr.ObjectMatched);
        }

        [TestMethod]
        public void BasicCall_RefTypeOverriden()
        {
            FastPropertyGetter fpg = new FastPropertyGetter("ClassName", typeof(LogEvent2));
            LogEvent2 le = new LogEvent2("myclass");
            PropertyResult gr = fpg.GetValue(le);
            Assert.AreEqual(fpg.PropertyType, typeof(string));
            Assert.AreEqual("myclass", gr.Data);
            Assert.IsTrue(gr.ObjectMatched);
        }

        [TestMethod]
        public void BasicCall_ValueType()
        {
            FastPropertyGetter fpg = new FastPropertyGetter("Id", typeof(LogEvent));
            LogEvent le = new LogEvent("myclass") { Id = 3 };
            PropertyResult gr = fpg.GetValue(le);
            Assert.AreEqual(fpg.PropertyType, typeof(int));
            Assert.AreEqual(3, fpg.GetValue(le).Data);
            Assert.IsTrue(gr.ObjectMatched);
        }

        [TestMethod]
        public void BasicCall_NullRef()
        {
            FastPropertyGetter fpg = new FastPropertyGetter("ClassName", typeof(LogEvent));
            PropertyResult gr = fpg.GetValue(null);
            Assert.AreEqual(fpg.PropertyType, typeof(string));
            Assert.IsNull(gr.Data);
            Assert.IsFalse(gr.ObjectMatched);
        }

        [TestMethod]
        public void WrongType()
        {
            FastPropertyGetter fpg = new FastPropertyGetter("ClassName", typeof(LogEvent));
            PropertyResult gr = fpg.GetValue("Some random type");
            Assert.AreEqual(fpg.PropertyType, typeof(string));
            Assert.IsFalse(gr.ObjectMatched);
            Assert.IsNull(gr.Data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNonsense()
        {
            FastPropertyGetter fpg = new FastPropertyGetter("as.ldfkjlsadfj", typeof(LogEvent));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullType()
        {
            FastPropertyGetter fpg = new FastPropertyGetter("asdfasdf", null);
        }

        [TestMethod]
        public void TestStructWithIncorrectType()
        {
            String s = "Boo!";

            FastPropertyGetter fpg = new FastPropertyGetter("Length", typeof(Test));
            PropertyResult pr = fpg.GetValue(s);

            Assert.IsFalse(pr.ObjectMatched);
        }

        [TestMethod]
        public void TestStruct()
        {
            Test data = new Test() { Length = 1 };

            FastPropertyGetter fpg = new FastPropertyGetter("Length", typeof(Test));
            PropertyResult pr = fpg.GetValue(data);

            Assert.IsTrue(pr.ObjectMatched);
            Assert.AreEqual(data.Length, pr.Data);
        }

        public struct Test
        {
            public int Length { get; set; }
        }

                /// <summary>
        /// Test to ensure parameters are validated
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFpgAllNull()
        {
            new FastPropertyGetter(null, null);
        }

        /// <summary>
        /// Test to ensure papameters are validated
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFpgNullProperties()
        {
            new FastPropertyGetter (null, typeof(string));
        }

        /*********************************************************************************************/
        /*                                                                                           */
        /* Tests for properties and fields on a reference type                                       */
        /*                                                                                           */
        /*********************************************************************************************/

        [TestMethod]
        public void Test_ReferenceType_ReferenceType_Field()
        {
            ReferenceType vt = new ReferenceType();
            vt.ReferenceTypeField = "Test";

            FastPropertyGetter gpv = new FastPropertyGetter("ReferenceTypeField", typeof(ReferenceType));

            PropertyResult o = gpv.GetValue(vt);

            Assert.IsNotNull(o, "No value returned from GetPropertyValue delegate");
            Assert.AreEqual<bool>(true, o.ObjectMatched, "ObjectMatched not set");
            Assert.IsInstanceOfType(o.Data, typeof(string), "string not returned");
            Assert.AreEqual<string>(vt.ReferenceTypeField, (string)o.Data, "ReferenceTypeField value returned was incorrect");
        }

        [TestMethod]
        public void Test_ReferenceType_ValueType_Field()
        {
            ReferenceType vt = new ReferenceType();
            vt.ValueTypeField = _firstDateTime;

            FastPropertyGetter gpv = new FastPropertyGetter("ValueTypeField",typeof(ReferenceType));

            PropertyResult o = gpv.GetValue(vt);

            Assert.IsNotNull(o, "No value returned from GetPropertyValue delegate");
            Assert.AreEqual<bool>(true, o.ObjectMatched, "ObjectMatched not set");
            Assert.IsInstanceOfType(o.Data, typeof(DateTime), "string not returned");
            Assert.AreEqual<DateTime>(vt.ValueTypeField, (DateTime)o.Data, "ValueTypeField value returned was incorrect");
        }

        [TestMethod]
        public void Test_ReferenceType_ReferenceType_Property()
        {
            ReferenceType vt = new ReferenceType();
            vt.ReferenceTypeProperty = "Test";

            FastPropertyGetter gpv = new FastPropertyGetter("ReferenceTypeProperty", typeof(ReferenceType));

            PropertyResult o = gpv.GetValue(vt);

            Assert.IsNotNull(o, "No value returned from GetPropertyValue delegate");
            Assert.AreEqual<bool>(true, o.ObjectMatched, "ObjectMatched not set");
            Assert.IsInstanceOfType(o.Data, typeof(string), "string not returned");
            Assert.AreEqual<string>(vt.ReferenceTypeProperty, (string)o.Data, "ReferenceTypeProperty value returned was incorrect");
        }

        [TestMethod]
        public void Test_ReferenceType_ValueType_Property()
        {
            ReferenceType vt = new ReferenceType();
            vt.ValueTypeProperty = _firstDateTime;

            FastPropertyGetter gpv = new FastPropertyGetter("ValueTypeProperty", typeof(ReferenceType));

            PropertyResult o = gpv.GetValue(vt);

            Assert.IsNotNull(o, "No value returned from GetPropertyValue delegate");
            Assert.AreEqual<bool>(true, o.ObjectMatched, "ObjectMatched not set");
            Assert.IsInstanceOfType(o.Data, typeof(DateTime), "string not returned");
            Assert.AreEqual<DateTime>(vt.ValueTypeProperty, (DateTime)o.Data, "ValueTypeProperty value returned was incorrect");
        }

        /*********************************************************************************************/
        /*                                                                                           */
        /* Tests for properties and fields on a value type                                           */
        /*                                                                                           */
        /*********************************************************************************************/
        
        [TestMethod]
        public void Test_ValueType_ReferenceType_Field()
        {
            ValueType vt = new ValueType();
            vt.ReferenceTypeField = "Test";

            FastPropertyGetter gpv = new FastPropertyGetter("ReferenceTypeField", typeof(ValueType));

            PropertyResult o = gpv.GetValue(vt);

            Assert.IsNotNull(o, "No value returned from GetPropertyValue delegate");
            Assert.AreEqual<bool>(true, o.ObjectMatched, "ObjectMatched not set");
            Assert.IsInstanceOfType(o.Data, typeof(string), "string not returned");
            Assert.AreEqual<string>(vt.ReferenceTypeField, (string)o.Data, "ReferenceTypeField value returned was incorrect");
        }

        [TestMethod]
        public void Test_ValueType_ValueType_Field()
        {
            ValueType vt = new ValueType();
            vt.ValueTypeField = _firstDateTime;

            FastPropertyGetter gpv = new FastPropertyGetter("ValueTypeField", typeof(ValueType));

            PropertyResult o = gpv.GetValue(vt);

            Assert.IsNotNull(o, "No value returned from GetPropertyValue delegate");
            Assert.AreEqual<bool>(true, o.ObjectMatched, "ObjectMatched not set");
            Assert.IsInstanceOfType(o.Data, typeof(DateTime), "string not returned");
            Assert.AreEqual<DateTime>(vt.ValueTypeField, (DateTime)o.Data, "ValueTypeField value returned was incorrect");
        }

        [TestMethod]
        public void Test_ValueType_ReferenceType_Property()
        {
            ValueType vt = new ValueType();
            vt.ReferenceTypeProperty= "Test";

            FastPropertyGetter gpv = new FastPropertyGetter("ReferenceTypeProperty", typeof(ValueType));

            PropertyResult o = gpv.GetValue(vt);

            Assert.IsNotNull(o, "No value returned from GetPropertyValue delegate");
            Assert.AreEqual<bool>(true, o.ObjectMatched, "ObjectMatched not set");
            Assert.IsInstanceOfType(o.Data, typeof(string), "string not returned");
            Assert.AreEqual<string>(vt.ReferenceTypeProperty, (string)o.Data, "ReferenceTypeProperty value returned was incorrect");
        }

        [TestMethod]
        public void Test_ValueType_ValueType_Property()
        {
            ValueType vt = new ValueType();
            vt.ValueTypeProperty = _firstDateTime;

            FastPropertyGetter gpv = new FastPropertyGetter("ValueTypeProperty", typeof(ValueType));

            PropertyResult o = gpv.GetValue(vt);

            Assert.IsNotNull(o, "No value returned from GetPropertyValue delegate");
            Assert.AreEqual<bool>(true, o.ObjectMatched, "ObjectMatched not set");
            Assert.IsInstanceOfType(o.Data, typeof(DateTime), "string not returned");
            Assert.AreEqual<DateTime>(vt.ValueTypeProperty, (DateTime)o.Data, "ValueTypeProperty value returned was incorrect");
        }

        /*********************************************************************************************/
        /*                                                                                           */
        /* Other tests for good measure                                                              */
        /*                                                                                           */
        /*********************************************************************************************/

        /// <summary>
        /// Test to see if I can get the value of a property of a value type
        /// </summary>
        [TestMethod]
        public void TestGetValueTypeProperty()
        {
            DateTime dt = DateTime.Now;

            FastPropertyGetter gpv = new FastPropertyGetter("Ticks", typeof(DateTime));

            PropertyResult o = gpv.GetValue(dt);

            Assert.IsNotNull(o, "No value returned from GetPropertyValue delegate");
            Assert.AreEqual<bool>(true, o.ObjectMatched, "ObjectMatched not set");
            Assert.IsInstanceOfType(o.Data, typeof(Int64), "Int64 not returned");
            Assert.AreEqual<Int64>(dt.Ticks, Convert.ToInt64(o.Data), "Tick value returned was incorrect");
        }

        /// <summary>
        /// Test to see I can get the value of a property of a reference type
        /// </summary>
        [TestMethod]
        public void TestGetReferenceTypeProperty()
        {
            TestRefOne tr1 = new TestRefOne();
            tr1.Property = new TestRefTwo(_firstDateTime, _secondDateTime);

            FastPropertyGetter gpv = new FastPropertyGetter("Property", typeof(TestRefOne));

            PropertyResult o = gpv.GetValue(tr1);

            Assert.IsNotNull(o, "No value returned from GetPropertyValue delegate");
            Assert.AreEqual<bool>(true, o.ObjectMatched, "ObjectMatched not set");
            Assert.IsInstanceOfType(o.Data, typeof(TestRefTwo), "TestRefTwo not returned");

            TestRefTwo tr2 = o.Data as TestRefTwo;

            Assert.AreEqual<DateTime>(_firstDateTime, tr2.DateTimeField);
            Assert.AreEqual<DateTime>(_secondDateTime, tr2.DateTimeProperty);
        }

        /// <summary>
        /// Test to ensure that an error is thrown if I try to read a non-public property
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetReferenceTypeNonPublicProperty()
        {
            TestRefOne tr1 = new TestRefOne();
            FastPropertyGetter gpv = new FastPropertyGetter("PrivateDateTimeProperty", typeof(TestRefOne));
        }

        /// <summary>
        /// Test to see that I can get a value from Reference.Field
        /// </summary>
        [TestMethod]
        public void TestGetReferenceTypeField()
        {
            TestRefOne tr1 = new TestRefOne();
            tr1.Field = new TestRefTwo(_firstDateTime, _secondDateTime);

            FastPropertyGetter gpv = new FastPropertyGetter("Field", typeof(TestRefOne));

            PropertyResult o = gpv.GetValue(tr1);

            Assert.IsNotNull(o, "No value returned from GetPropertyValue delegate");
            Assert.AreEqual<bool>(true, o.ObjectMatched, "ObjectMatched not set");
            Assert.IsInstanceOfType(o.Data, typeof(TestRefTwo), "TestRefTwo not returned");

            TestRefTwo tr2 = o.Data as TestRefTwo;

            Assert.AreEqual<DateTime>(_firstDateTime, tr2.DateTimeField);
            Assert.AreEqual<DateTime>(_secondDateTime, tr2.DateTimeProperty);
        }

        /// <summary>
        /// Test to see that I can get a value from Reference.ValueTypeField.ReferenceTypeProperty
        /// </summary>
        [TestMethod]
        public void TestGetReferenceValueTypeFieldReferenceTypeProperty()
        {
            TestRefOne tr1 = new TestRefOne();
            tr1.ValueTypeField = _firstDateTime;

            FastPropertyGetter gpv = new FastPropertyGetter("ValueTypeField.Hour", typeof(TestRefOne));

            PropertyResult o = gpv.GetValue(tr1);

            Assert.IsNotNull(o, "No value returned from GetPropertyValue delegate");
            Assert.AreEqual<bool>(true, o.ObjectMatched, "ObjectMatched not set");
            Assert.IsInstanceOfType(o.Data, typeof(int), "int not returned");
            Assert.AreEqual<int>(tr1.ValueTypeField.Hour, (int)o.Data);
        }

        /// <summary>
        /// Test to see that I can get a value from Reference.ValueTypeProperty.ReferenceTypeProperty
        /// </summary>
        [TestMethod]
        public void TestGetReferenceValueTypePropertyReferenceTypeProperty()
        {
            TestRefOne tr1 = new TestRefOne();
            tr1.ValueTypeProperty = _firstDateTime;

            FastPropertyGetter gpv = new FastPropertyGetter("ValueTypeProperty.Hour", typeof(TestRefOne));

            PropertyResult o = gpv.GetValue(tr1);

            Assert.IsNotNull(o, "No value returned from GetPropertyValue delegate");
            Assert.AreEqual<bool>(true, o.ObjectMatched, "ObjectMatched not set");
            Assert.IsInstanceOfType(o.Data, typeof(int), "int not returned");
            Assert.AreEqual<int>(tr1.ValueTypeProperty.Hour, (int)o.Data);
        }

        /// <summary>
        /// Test the code where an overridden method is marked as sealed
        /// </summary>
        [TestMethod]
        public void TestPeskyMethodInfoIsFinal()
        {
            TestRefThree tr3 = new TestRefThree();
            tr3.NameProperty = "Chuff";

            FastPropertyGetter gpv = new FastPropertyGetter("NameProperty", typeof(TestRefThree));

            PropertyResult o = gpv.GetValue(tr3);

            Assert.IsNotNull(o, "No value returned from GetPropertyValue delegate");
            Assert.AreEqual<bool>(true, o.ObjectMatched, "ObjectMatched not set");
            Assert.IsInstanceOfType(o.Data, typeof(string), "string not returned");
            Assert.AreEqual<string>(tr3.NameProperty, (string)o.Data);
        }

        /// <summary>
        /// Test that it works when I pass a null into the GetData delegate
        /// </summary>
        [TestMethod]
        public void TestWithNullObject()
        {
            TestRefThree tr3 = new TestRefThree();
            tr3.NameProperty = "Chuff";

            FastPropertyGetter gpv = new FastPropertyGetter("NameProperty", typeof(TestRefThree));

            PropertyResult o = gpv.GetValue(null);

            Assert.IsNotNull(o, "No property result returned");
            Assert.IsFalse(o.ObjectMatched, "Did not expect an object match");
        }

        /// <summary>
        /// Test that it works when I pass in an unexpected value type to the GetData delegate
        /// </summary>
        [TestMethod]
        public void TestWithWrongValueType()
        {
            FastPropertyGetter gv = new FastPropertyGetter("Hour", typeof(DateTime));
            PropertyResult o = gv.GetValue(new ValueType());

            Assert.IsNotNull(o, "No property result returned");
            Assert.IsFalse(o.ObjectMatched, "Did not expect an object match");
        }

        /// <summary>
        /// Test that it blows when I pass in an unexpected reference type to the GetData delegate
        /// </summary>
        [TestMethod]
        public void TestWithWrongReferenceType()
        {
            FastPropertyGetter gv = new FastPropertyGetter("ReferenceTypeProperty", typeof(ReferenceType));
            PropertyResult o = gv.GetValue(new object());

            Assert.IsNotNull(o, "No property result returned");
            Assert.IsFalse(o.ObjectMatched, "Did not expect an object match");
        }

        [TestMethod]
        public void TestNullRefInChain()
        {
            FastPropertyGetter fpg1 = new FastPropertyGetter("ReferenceTypeProperty.Length", typeof(ReferenceType));
            FastPropertyGetter fpg2 = new FastPropertyGetter("ReferenceTypeProperty.Length", typeof(ValueType));
            
            object result1 = fpg1.GetValue(new ReferenceType()).Data;
            object result2 = fpg2.GetValue(new ValueType()).Data;

            Assert.IsNull(result1);
            Assert.IsNull(result2);
        }

        private static DateTime _firstDateTime = new DateTime(2000, 01, 02, 03, 04, 05);

        private static DateTime _secondDateTime = new DateTime(2010, 09, 08, 07, 06, 05);
    }

    public class TestRefOne
    {
        public TestRefTwo Field;

        public TestRefTwo Property { get; set; }

        public DateTime ValueTypeField;

        public DateTime ValueTypeProperty { get; set; }

        public virtual string NameProperty { get; set; }
    }

    public class TestRefTwo
    {
        public TestRefTwo(DateTime fieldValue, DateTime propertyValue)
        {
            DateTimeField = fieldValue;
            DateTimeProperty = propertyValue;
        }

        public DateTime DateTimeField;

        public DateTime DateTimeProperty { get; set; }

        public string NameProperty { get; set; }
    }

    public class TestRefThree : TestRefOne
    {
        public sealed override string NameProperty
        {
            get
            {
                return base.NameProperty;
            }
            set
            {
                base.NameProperty = value;
            }
        }
    }

    public struct ValueType
    {
        public DateTime ValueTypeField;

        public DateTime ValueTypeProperty { get; set; }

        public string ReferenceTypeField;

        public string ReferenceTypeProperty { get; set; }
    }

    public class ReferenceType
    {
        public DateTime ValueTypeField;

        public DateTime ValueTypeProperty { get; set; }

        public string ReferenceTypeField;

        public string ReferenceTypeProperty { get; set; }
    }

    public class LogEvent
    {
        public LogEvent(string className)
        {
            ClassName = className;
        }

        public virtual string ClassName { get; set; }
        public string MethodName { get; set; }
        public object Data { get; set; }
        public int Id { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]{2}", ClassName, MethodName, Data);
        }
        public int GetID()
        {
            return 3;
        }
    }

    public class LogEvent2 : LogEvent
    {
        public LogEvent2(string className)
            : base(className)
        { }

        public sealed override string ClassName
        {
            get
            {
                return base.ClassName;
            }
            set
            {
                base.ClassName = value;
            }
        }
    }
}
