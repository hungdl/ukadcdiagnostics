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
using TheJoyOfCode.QualityTools;
using Rhino.Mocks;
using Ukadc.Diagnostics.Utils.PropertyReaders;

namespace Ukadc.Diagnostics.Tests
{
    [TestClass]
    public class AutomatedTests
    {
        [TestMethod]
        public void TestAllTypesSomeExclusions()
        {
            AssemblyTester tester = new AssemblyTester("Ukadc.Diagnostics", new CustomTypeFactory(), true);
            tester.Exclusions.Add(new AbstractOrStaticTestExclusion());
            tester.Exclusions.AddType(typeof(MultiFilter));
            tester.Exclusions.AddType(typeof(PropertyFilter));
            tester.Exclusions.AddType(typeof(FastPropertyGetter));
            tester.Exclusions.AddType(typeof(FastPropertyReader));
            tester.Exclusions.AddType(typeof(DynamicPropertyReader));
            tester.Exclusions.AddType(typeof(SqlDataAccessCommand));
            tester.Exclusions.AddType(typeof(ExtendedSource));
            tester.Exclusions.AddType(typeof(Disposer));
            tester.Exclusions.Add(new OpenGenericExclude());
            tester.Exclusions.Add(new TraceListenerExclude());
            tester.TestAssembly(true, true);
        }

        private class OpenGenericExclude : TestExclusion
        {
            public override bool IsExcluded(Type type)
            {
                return type.IsGenericTypeDefinition;
            }
        }

        private class TraceListenerExclude : TestExclusion
        {
            public override bool IsExcluded(Type type)
            {
                return typeof(TraceListener).IsAssignableFrom(type);
            }
        }

        private class CustomTypeFactory : TypeFactory
        {
            private MockRepository _mocks = new MockRepository();

            public override bool CanCreateInstance(Type type)
            {
                if (type.IsInterface || type.IsAbstract || type.IsArray)
                {
                    return true;
                }
                
                return base.CanCreateInstance(type);
            }

            public override object CreateRandomValue(Type type)
            {
                if (type.IsInterface || type.IsAbstract)
                {
                    return _mocks.DynamicMock(type);
                }
                if (type.IsArray)
                {
                    return Array.CreateInstance(type.GetElementType(), _random.Next(100));
                }

                return base.CreateRandomValue(type);
            }
        }

        private class AbstractOrStaticTestExclusion : TestExclusion
        {
            public override bool IsExcluded(Type type)
            {
                Trace.WriteLine(type);
                return type.IsAbstract
                    || type.IsInterface
                    || type.GetConstructors().Length == 0
                    || type.IsSubclassOf(typeof(Delegate));
            }
        }
    }
}
