// 
// Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using BinaryFinery.IOC.Runtime.Build;
using BinaryFinery.IOC.UnitTests.Inputs;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace BinaryFinery.IOC.UnitTests.Tests.Building
{
    public class TestDependencyConstruction
    {
        private ContextManager CM;

        [SetUp]
        public void Setup()
        {
            CM = ContextSystem.Manager;
            CM.RegisterCustomContextImplementation(typeof(DependencyTestContextTop), typeof(IDependencyTestContext));
            CM.RegisterCustomContextImplementation(typeof(DependencyTestContextTop), typeof(IDependencyTestContext2));
            CM.RegisterCustomContextImplementation(typeof(DependencyTestContextTop), typeof(IDependencyTestContext2a));
            CM.RegisterCustomContextImplementation(typeof(DependencyTestContextTop),
                                                   typeof(IDependencyTestContextAttributed));
            CM.RegisterCustomContextImplementation(typeof(DependencyTestContextTop), typeof(IDependencyTestCyclic));
            CM.RegisterCustomContextImplementation(typeof(DependencyTestContextTop), typeof(IDependencyTestProperyInjection));
        }

        [Test]
        public void DependenciesInjectedViaConstructor()
        {
            var context = CM.Create<IDependencyTestContext>();
            var result = context.DepsP;
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void DependenciesReallyInjectedViaConstructor()
        {
            var context = CM.Create<IDependencyTestContext>();
            var result = context.DepsP;
            var foo = context.FooP;
            Assert.That(result.Myfoo, Is.SameAs(foo));
        }

        [Test]
        public void FirstPublicConstructorIsCalledByDefault()
        {
            var context = CM.Create<IDependencyTestContext2>();
            var result = context.DepsP;
            Assert.That(result.Myfoo, Is.Null);
        }

        [Test]
        public void FirstPublicConstructorIsCalledByDefault2a()
        {
            var context = CM.Create<IDependencyTestContext2a>();
            var result = context.DepsP;
            var foo = context.FooP;
            Assert.That(result.Myfoo, Is.SameAs(foo));
            Assert.IsInstanceOfType(typeof(Deps2a), result);
        }

        [Test]
        public void AttributedInjectorIsUsed()
        {
            var context = CM.Create<IDependencyTestContextAttributed>();
            var result = context.DepsP;
            var foo = context.FooP;
            Assert.That(result.Myfoo, Is.Not.Null);
            Assert.That(result.Myfoo, Is.Not.SameAs(foo));
            Assert.IsInstanceOfType(typeof(DepsAttributed), result);
        }

        [Test]
        [ExpectedException(typeof(CyclicDependencyException))]
        public void CyclicDepsThrow()
        {
            var context = CM.Create<IDependencyTestCyclic>();
            var result = context.DepsP;
            var foo = context.FooP;
        }

        [Test]
        public void InjectionViaPropertyWorks()
        {
            var context = CM.Create<IDependencyTestProperyInjection>();
            var result = context.DepsP;
            var foo = context.FooP;
            Assert.That(result.Myfoo, Is.SameAs(foo));
        }

    }
}