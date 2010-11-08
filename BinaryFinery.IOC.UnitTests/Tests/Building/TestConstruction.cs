// 
// Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using BinaryFinery.IOC.Runtime;
using BinaryFinery.IOC.Runtime.Build;
using BinaryFinery.IOC.UnitTests.Inputs;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace BinaryFinery.IOC.UnitTests.Tests.Building
{
    public interface ITestContext : IContext
    {
        Foo Foo { get; }
    }


    public class TestContextImpl : BaseContextImpl, ITestContext
    {
        public Foo Foo
        {
            get { return (Foo) Factory.ObjectForProperty("Foo"); }
        }
    }

    public class TestConstruction
    {
        private ContextManager CM;

        [SetUp]
        public void Setup()
        {
            CM = ContextSystem.Manager;
            CM.RegisterCustomContextImplementation(typeof(TestContextImpl));
        }

        [Test]
        public void CanCreateNewContextFromInterface()
        {
            ITestContext context = CM.Create<ITestContext>();
            Assert.That(context, Is.Not.Null);
        }

        [Test]
        public void CanCreateFoo()
        {
            ITestContext context = CM.Create<ITestContext>();
            IFoo foo = context.Foo;
            Assert.IsInstanceOfType(typeof(Foo), foo);
        }

        [Test]
        public void CreatedFooIsSingleton()
        {
            ITestContext context = CM.Create<ITestContext>();
            IFoo foo = context.Foo;
            IFoo foo2 = context.Foo;
            Assert.That(foo, Is.SameAs(foo2));
        }
    }
}