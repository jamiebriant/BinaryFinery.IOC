using System;
using System.Collections.Generic;
using System.Text;
using BinaryFinery.IOC.Runtime;
using BinaryFinery.IOC.Runtime.Build;
using BinaryFinery.IOC.Runtime.Meta;
using BinaryFinery.IOC.UnitTests.Inputs;
using NUnit.Framework;

namespace BinaryFinery.IOC.UnitTests.Tests.Building
{
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Property, AllowMultiple = true)]
    public class TestImplementationAttribute : ImplementationAttribute
    {
        private readonly bool applies;

        public TestImplementationAttribute(Type type, bool applies) : base(type)
        {
            this.applies = applies;
        }

        public TestImplementationAttribute(Type type, InstantiationTiming timing, bool applies) : base(type, timing)
        {
            this.applies = applies;
        }

        public override bool Applies
        {
            get { return applies; }
        }
    }

    public interface IImpTestContext : IContext
    {
        IFoo Foo { get; }
    }

    public interface IImpTestContext1 : IImpTestContext
    {
        [TestImplementation(typeof(Foo), true)]
        [TestImplementation(typeof(OtherFoo), false)]
        IFoo Foo { get; }
    }

    public interface IImpTestContext2 : IImpTestContext
    {
        [TestImplementation(typeof(Foo), false)]
        [TestImplementation(typeof(OtherFoo), true)]
        IFoo Foo { get; }
    }

    public interface IImpTestContext2B : IImpTestContext2
    {
        [TestImplementation(typeof(FooToo), false)]
        [TestImplementation(typeof(Foo), false)]
        IFoo Foo { get; }
    }

    public interface IImpTestContext1B : IImpTestContext1
    {
        [TestImplementation(typeof(Foo), true)]
        [TestImplementation(typeof(FooToo), true)]
        [TestImplementation(typeof(Foo), true)]
        IFoo Foo { get; }
    }
    public interface IImpTestContext1C : IImpTestContext1
    {
        [TestImplementation(typeof(Foo), true)]
        [TestImplementation(typeof(OtherFoo), true)]
        [TestImplementation(typeof(Foo), true)]
        IFoo Foo { get; }
    }


    public class ImpTestContextImpl : BaseContextImpl, IImpTestContext, IImpTestContext1, IImpTestContext2, IImpTestContext2B, IImpTestContext1B, IImpTestContext1C
    {
        public IFoo Foo
        {
            get { return (IFoo)ObjectForProperty("Foo"); }
        }
    }

    public class TestImplementationStuff
    {
        private ContextManager CM;

        [SetUp]
        public void Setup()
        {
            CM = ContextSystem.Manager;
            CM.RegisterCustomContextImplementation(typeof(ImpTestContextImpl));
        }

        [Test]
        public void TestDerivedImplementationAttributesWorkAtAll1()
        {
            IImpTestContext context = CM.Create<IImpTestContext1>();
            IFoo foo = context.Foo;
            Assert.IsInstanceOfType(typeof(Foo),foo);
        }
        [Test]
        public void TestDerivedImplementationAttributesWorkAtAll2()
        {
            IImpTestContext context = CM.Create<IImpTestContext2>();
            IFoo foo = context.Foo;
            Assert.IsInstanceOfType(typeof(OtherFoo), foo);
        }
        [Test]
        public void WillStillUseOlderDefinitions()
        {
            IImpTestContext context = CM.Create<IImpTestContext2B>();
            IFoo foo = context.Foo;
            Assert.IsInstanceOfType(typeof(OtherFoo), foo);
        }
        [Test]
        public void MultipleActiveImplsWillUseMostSpecificType()
        {
            IImpTestContext context = CM.Create<IImpTestContext1B>();
            IFoo foo = context.Foo;
            Assert.IsInstanceOfType(typeof(FooToo), foo);
        }
        [Test]
        [ExpectedException(typeof(ImplementationsMismatchException))]
        public void MultipleActiveImplsWillThrowUsefulExceptionIfImplsAreIncompatible()
        {
            IImpTestContext context = CM.Create<IImpTestContext1C>();
            IFoo foo = context.Foo;
            Assert.IsInstanceOfType(typeof(FooToo), foo);
        }

    }
}
