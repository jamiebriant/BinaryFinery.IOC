// 
// Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using BinaryFinery.IOC.Runtime;
using BinaryFinery.IOC.Runtime.Build;
using BinaryFinery.IOC.Runtime.Meta;
using BinaryFinery.IOC.UnitTests.Inputs;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace BinaryFinery.IOC.UnitTests.Tests.Building
{
    public interface ITestContext : IContext
    {
        Foo Foo { get; }
    }



    public class TestContextImpl : BaseContextImpl, ITestContext, IEagerTestContext, ILazyTestContext
    {
        public Foo Foo
        {
            get { return (Foo) ObjectForProperty("Foo"); }
        }
    }

    public interface IInterface1
    {
        
    }
    public interface IInterface2
    {
        
    }

    class EagerFoo : Foo
    {
        public static bool IsContructed = false;

        public EagerFoo()
        {
            IsContructed = true;
        }
    }

    public interface IEagerTestContext : ITestContext
    {
        [Implementation(typeof(EagerFoo),InstantiationTiming.Eager)]
        Foo Foo { get; }
    }

    public interface ILazyTestContext : ITestContext
    {
        [Implementation(typeof(EagerFoo))]
        Foo Foo { get; }
    }

    public class MyImpl : IInterface1, IInterface2
    {
        
    }

    public interface ITestContext2 : IContext
    {
        [Implementation(typeof(MyImpl))]
        IInterface1 Iface1 { get; }

        [Implementation(typeof(MyImpl))]
        IInterface2 Iface2 { get; }
    }

    public class TestContext2Impl : BaseContextImpl, ITestContext2
    {
        public IInterface1 Iface1
        {
            get { return (IInterface1)ObjectForProperty("Iface1"); }
        }

        public IInterface2 Iface2
        {
            get { return (IInterface2)ObjectForProperty("Iface2"); }
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
            CM.RegisterCustomContextImplementation(typeof(TestContext2Impl));
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
        [Test]
        public void CreatedFooIsSingletonAcrossAllProperties()
        {
            ITestContext2 context = CM.Create<ITestContext2>();
            IInterface1 foo = context.Iface1;
            IInterface2 foo2 = context.Iface2;
            Assert.That(foo, Is.SameAs(foo2));
        }
        [Test]
        public void EagerImplementaitonsAreCreatedImmediately()
        {
            EagerFoo.IsContructed = false;
            IEagerTestContext context = CM.Create<IEagerTestContext>();
            Assert.That(EagerFoo.IsContructed, Is.True);
        }
        [Test]
        public void LazyImplementaitonsAreNotCreatedImmediately()
        {
            EagerFoo.IsContructed = false;
            ILazyTestContext context = CM.Create<ILazyTestContext>();
            Assert.That(EagerFoo.IsContructed, Is.False);
            Foo x = context.Foo;
            Assert.That(EagerFoo.IsContructed, Is.True);
        }
    }
}