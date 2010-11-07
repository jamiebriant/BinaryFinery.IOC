using System;
using BinaryFinery.IOC.Runtime;
using BinaryFinery.IOC.Runtime.Build;
using BinaryFinery.IOC.Runtime.Meta;
using BinaryFinery.IOC.UnitTests.Inputs;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace BinaryFinery.IOC.UnitTests.Tests.Meta
{
    public interface IImplementationTestContext : IContext
    {
        [Implementation(typeof(Foo))]
        IFoo FooP { get;  }
    }

    public interface IDerivedImplementationTestContext : IImplementationTestContext
    {
        [Implementation(typeof (FooToo))]
        IFoo FooP { get; }
    }

    
    public class TestComprehension
    {
        [Test]
        public void TestContextIsForAType()
        {
            ContextManager cf = ContextSystem.Manager;
            IContextFactory factory = cf.GetFactory<IRootContext>();
            Assert.That( factory.ContextType, Is.EqualTo(typeof(IRootContext)));
        }
        [Test]
        public void TestFooPIsFoo()
        {
            ContextManager cf = ContextSystem.Manager;
            IContextFactory factory = cf.GetFactory<IRootContext>();
            Type foot = factory.TypeForProperty("FooP");
            Assert.That(foot,Is.EqualTo(typeof(Foo)));
        }
        [Test]
        public void TestFooPIsIFoo()
        {
            ContextManager cf = ContextSystem.Manager;
            IContextFactory factory = cf.GetFactory<IImplementationTestContext>();
            Type foot = factory.TypeForProperty("FooP");
            Assert.That(foot, Is.EqualTo(typeof(IFoo)));
        }
        [Test]
        public void TestImplementationAttribute()
        {
            ContextManager cf = ContextSystem.Manager;
            IContextFactory factory = cf.GetFactory<IImplementationTestContext>();
            Type foot = factory.ImplementationTypeForProperty("FooP");
            Assert.That(foot, Is.EqualTo(typeof(Foo)));
        }
        [Test]
        public void TestImplementationTypeWorksWithNoAttribute()
        {
            ContextManager cf = ContextSystem.Manager;
            IContextFactory factory = cf.GetFactory<IRootContext>();
            Type foot = factory.ImplementationTypeForProperty("FooP");
            Assert.That(foot, Is.EqualTo(typeof(Foo)));
        }
        [Test]
        public void TestDerivedFooP()
        {
            ContextManager cf = ContextSystem.Manager;
            IContextFactory factory = cf.GetFactory<IDerivedImplementationTestContext>();
            Type foot = factory.ImplementationTypeForProperty("FooP");
            Assert.That(foot, Is.EqualTo(typeof(FooToo)));
        }
    }
}
