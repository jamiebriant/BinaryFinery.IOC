using System;
using System.Collections.Generic;
using System.Text;
using BinaryFinery.IOC.Runtime.Build;
using BinaryFinery.IOC.UnitTests.Inputs;
using NUnit.Framework;

namespace BinaryFinery.IOC.UnitTests.Tests.Building
{
    public class TestContextManagement
    {
        [Test]
        public void RegisteringAContextProxyShouldRegisterAllItsInterfaces()
        {
            ContextManager cm = ContextSystem.ManagerForTesting;

            cm.RegisterCustomContextImplementation(typeof(DependencyTestContextTop));

            var context = cm.Create<IDependencyTestMethodInjection2>();
            Assert.IsNotNull(context);
        }
    }
}
