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
        [Test]
        public void TestCanCreateBasicContext()
        {
            ContextManager cm = ContextSystem.ManagerForTestingIoCItselfWithoutTestingFlags;
            var context = cm.CreateBasic<IDependencyTestMethodInjection2>();
            var foo = context.Get<IFoo>("FooP");
            Assert.That(foo,Is.InstanceOfType(typeof(Foo)));
        }
        [Test]
        public void TestCanCreateBasicContextWithAllInterfaces()
        {
            ContextManager cm = ContextSystem.ManagerForTestingIoCItselfWithoutTestingFlags;
            var context = cm.CreateBasic<ICompound>();
            var foo = context.Get<Foo>("Foo");
            Assert.That(foo, Is.InstanceOfType(typeof(Foo)));
            var bar = context.Get<Bar>("Bar");
            Assert.That(bar, Is.InstanceOfType(typeof(Bar)));
        }
    }
}
