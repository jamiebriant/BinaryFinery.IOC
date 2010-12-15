// 
// Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System;
using System.Reflection;
using BinaryFinery.IOC.Runtime.Build;
using BinaryFinery.IOC.Runtime.Meta;
using BinaryFinery.IOC.UnitTests.Inputs;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace BinaryFinery.IOC.UnitTests.Tests.Meta
{
    public class TestErrorsInHierarchy
    {
        [Test]
        [ExpectedException(typeof(ImplementationInterfaceMismatchException))]
        public void ImplementationTypeMustImplementPropertyType()
        {
            ContextManager cf = ContextSystem.ManagerForTesting;
            IContextFactory factory = cf.GetFactory<IDodgyImplContext>();
            Type foot = factory.ImplementationTypeForPropertyForTesting("FooP");
        }

        [Test]
        public void ImplementationExceptionProvidesUsefulInfo()
        {
            try
            {
                ContextManager cf = ContextSystem.ManagerForTesting;
                IContextFactory factory = cf.GetFactory<IDodgyImplContext>();
                Type foot = factory.ImplementationTypeForPropertyForTesting("FooP");
            }
            catch (ImplementationInterfaceMismatchException e)
            {
                Assert.That(e.ImplementationType, Is.EqualTo(typeof(Bar)));
                Assert.That(e.RequiredType, Is.EqualTo(typeof(IFoo)));
                Assert.That(e.ContextType, Is.EqualTo(typeof(IDodgyImplContext)));
                return;
            }
            Assert.Fail("Shouldn't get here.");
        }

        [Test]
        [ExpectedException(typeof(ImplementationsMismatchException))]
        public void ImplementationTypeMustImplementSameOrDerivedOfPreviousImplementation()
        {
            ContextManager cf = ContextSystem.ManagerForTesting;
            IContextFactory factory = cf.GetFactory<IDodgyDerivedImplementationContext>();
            Type foot = factory.ImplementationTypeForPropertyForTesting("FooP");
        }

        #region Quick Assumption Test

        [Test]
        public void TestOurAssumptionOfAttributeInheritance()
        {
            Type t = typeof(IDerivedImplementationTestContext);
            PropertyInfo p = t.GetProperty("FooP");
            object[] attrs = p.GetCustomAttributes(typeof(ImplementationAttribute), true);
            Assert.That(attrs.Length, Is.EqualTo(1));
        }

        [Test]
        public void TestOurAssumptionOfAttributeInheritance2()
        {
            Type t = typeof(IDerivedImplementationTestContext);
            PropertyInfo p = t.GetProperty("FooP");
            object[] attrs = p.GetCustomAttributes(typeof(ImplementationAttribute), false);
            Assert.That(attrs.Length, Is.EqualTo(1));
        }

        [Test]
        public void TestOurAssumptionOfAttributeInheritance3()
        {
            Type t = typeof(IPointlessDerivedImplementationContext);
            PropertyInfo p = t.GetProperty("FooP");
            object[] attrs = p.GetCustomAttributes(typeof(ImplementationAttribute), true);
            Assert.That(attrs.Length, Is.EqualTo(0)); // thought this might be 1. Not tho is it?
        }

        #endregion

        [Test]
        public void ImplementationsMismatchExceptionGivesUsefulInfo()
        {
            try
            {
                ContextManager cf = ContextSystem.ManagerForTesting;
                IContextFactory factory = cf.GetFactory<IDodgyDerivedImplementationContext>();
                Type foot = factory.ImplementationTypeForPropertyForTesting("FooP");
            }
            catch (ImplementationsMismatchException e)
            {
                Console.WriteLine(e);
                Assert.That(e.ImplementationType, Is.EqualTo(typeof(OtherFoo)));
                Assert.That(e.BaseImplementationType, Is.EqualTo(typeof(Foo)));
                Assert.That(e.ContextType, Is.EqualTo(typeof(IDodgyDerivedImplementationContext)));
                Assert.That(e.BaseContext, Is.EqualTo(typeof(IImplementationTestContext)));
                return;
            }
            Assert.Fail("Shouldn't get here.");
        }
    }
}