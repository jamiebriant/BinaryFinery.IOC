using System;
using System.Collections.Generic;
using System.Text;
using BinaryFinery.IOC.Runtime.Build;
using BinaryFinery.IOC.UnitTests.Inputs;
using NUnit.Framework;

namespace BinaryFinery.IOC.UnitTests.Tests.Building
{
    public class TestNamedDependencies
    {
        private ContextManager CM;

        [SetUp]
        public void Setup()
        {
            CM = ContextSystem.Manager;
            CM.RegisterCustomContextImplementation(typeof(NamedDependenciesContextImpl));
        }

        [Test]
        public void VerifyAssumptionsThatFirstDeclaredPropertyReturningTypeWillBeUsedForDependencyResolution()
        {
            var context = CM.Create<INamedDependenciesContext2>();

            var dep = context.NeedsFoo;

            Assert.IsInstanceOfType(typeof(OtherFoo), dep.Myfoo);
        }

        [Test]
        [Description("So, the order depends upon the types position in the highest declared interface. This is not really reliable is it.")]
        public void VerifyAssumptionsThatFirstDeclaredPropertyReturningTypeWillBeUsedForDependencyResolution2()
        {
            var context = CM.Create<INamedDependenciesContext2a>();

            var dep = context.NeedsFoo;

            Assert.IsInstanceOfType(typeof(Foo), dep.Myfoo);
        }


    }
}
