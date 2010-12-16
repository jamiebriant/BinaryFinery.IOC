using System;
using System.Collections.Generic;
using System.Text;
using BinaryFinery.IOC.Runtime.Build;
using BinaryFinery.IOC.UnitTests.Inputs;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace BinaryFinery.IOC.UnitTests.Tests.Building
{
    public class TestContextManagement
    {
        [Test]
        public void RegisteringAContextProxyShouldRegisterAllItsInterfaces()
        {
            ContextManager cm = ContextSystem.ManagerForTestingIocItself;

            cm.RegisterCustomContextImplementation(typeof(DependencyTestContextTop));

            var context = cm.Create<IDependencyTestMethodInjection2>();
            Assert.IsNotNull(context);
        }

        [Test]
        public void TestThatCreatedContextsAreIndependent()
        {
            ContextManager cm = ContextSystem.ManagerForTestingIocItself;

            cm.RegisterCustomContextImplementation(typeof(DependencyTestContextTop));
            var context = cm.Create<IDependencyTestMethodInjection2>();
            var context2 = cm.Create<IDependencyTestMethodInjection2>();

            Assert.That(context.FooP,Is.Not.EqualTo(context2.FooP));
            
        }
    }
}
