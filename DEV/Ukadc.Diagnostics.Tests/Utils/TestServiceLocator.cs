using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ukadc.Diagnostics.Utils;
using Ukadc.Diagnostics.Utils.PropertyReaders;
using System.ComponentModel.Design;

namespace Ukadc.Diagnostics.Tests.Utils
{
    [TestClass]
    public class TestServiceLocator
    {
        [TestMethod]
        public void TestIServiceProviderImpl()
        {
            ServiceLocator locator = new ServiceLocator();
            locator.RegisterType<ITestService, TestService>();
            IServiceProvider provider = (IServiceProvider)locator;
            Assert.IsInstanceOfType(provider.GetService(typeof(ITestService)), typeof(TestService));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetWithoutRegister()
        {
            ServiceLocator locator = new ServiceLocator();
            locator.GetService<ITestService>();
        }

        [TestMethod]
        public void BasicRegisterAndGet()
        {
            ServiceLocator locator = new ServiceLocator();
            locator.RegisterType<ITestService>(delegate { return new TestService(); });
            locator.RegisterType<ITestService2>(delegate { return new TestService2(); });
            Assert.IsInstanceOfType(locator.GetService<ITestService>(), typeof(TestService));
            Assert.IsInstanceOfType(locator.GetService<ITestService2>(), typeof(TestService2));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void OverrideWithoutRegister()
        {
            ServiceLocator locator = new ServiceLocator();
            locator.OverrideType<ITestService>(delegate { return new TestService(); });
        }

        [TestMethod]
        public void RegisterAndOverrideService()
        {
            ServiceLocator locator = new ServiceLocator();
            locator.RegisterType<ITestService>(delegate { return new TestService(); });
            Assert.IsInstanceOfType(locator.GetService<ITestService>(), typeof(TestService));
            // We want to temporarily override the implementation of ITestService (this is what we'll do in testing circumstances)
            using (locator.OverrideType<ITestService>(delegate { return new TestServiceOther(); }))
            {
                Assert.IsInstanceOfType(locator.GetService<ITestService>(), typeof(TestServiceOther));
            }
            Assert.IsInstanceOfType(locator.GetService<ITestService>(), typeof(TestService));
        }

        [TestMethod]
        public void RegisterInstanceAndOverride()
        {
            TestService ts = new TestService();
            TestServiceOther ts2 = new TestServiceOther();
            ServiceLocator locator = new ServiceLocator();
            locator.RegisterType<ITestService>(ts);
            Assert.IsTrue(Object.ReferenceEquals(ts, locator.GetService<ITestService>()));
            using (locator.OverrideType<ITestService>(ts2))
            {
                Assert.IsTrue(Object.ReferenceEquals(ts2, locator.GetService<ITestService>()));
            }
            Assert.IsTrue(Object.ReferenceEquals(ts, locator.GetService<ITestService>()));
        }

        [TestMethod]
        public void RegisterLazySingleton()
        {
            ServiceLocator locator = new ServiceLocator();
            locator.RegisterType<ITestService, TestService>();
            Assert.IsTrue(object.ReferenceEquals(locator.GetService<ITestService>(), locator.GetService<ITestService>()));
        }

        [TestMethod]
        public void OverrideLazySingleton()
        {
            ServiceLocator locator = new ServiceLocator();
            locator.RegisterType<ITestService, TestService>();
            Assert.IsInstanceOfType(locator.GetService<ITestService>(), typeof(TestService));
            using (locator.OverrideType<ITestService, TestServiceOther>())
            {
                Assert.IsInstanceOfType(locator.GetService<ITestService>(), typeof(TestServiceOther));
            }
            Assert.IsInstanceOfType(locator.GetService<ITestService>(), typeof(TestService));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RegisterTwice()
        {
            ServiceLocator locator = new ServiceLocator();
            locator.RegisterType<ITestService>(delegate { return new TestService();  });
            locator.RegisterType<ITestService>(delegate { return new TestServiceOther(); });
        }
    }

    public interface ITestService
    {
        void DoNothing();
    }

    public interface ITestService2
    {
        void DoSomething();
    }

    public class TestService : ITestService
    {
        public void DoNothing()
        {
            throw new NotImplementedException();
        }
    }

    public class TestServiceOther : ITestService
    {
        #region ITestService Members

        public void DoNothing()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class TestService2 : ITestService2
    {
        #region ITestService2 Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
