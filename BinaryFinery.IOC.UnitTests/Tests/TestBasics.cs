using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryFinery.IOC.Runtime;
using BinaryFinery.IOC.Runtime.Builder;
using BinaryFinery.IOC.UnitTests.Tests.Services;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace BinaryFinery.IOC.UnitTests.Tests
{

    public class TestBasics
    {
        private Context C;

        [SetUp]
        public void Setup()
        {
            C = ContextSystem.CreateContextForTesting();
            C.RegisterSingleton<Document, IDocument>();
            C.RegisterSingleton<ComponentA, IComponentA>();
            C.RegisterSingleton<ComponentB, IComponentB>();
            C.RegisterSingleton<ComponentC, IComponentC>();
            C.RegisterSingleton<SomeImplementation, ISomeInterface>();
            
        }
        [Test]
        public void TestBasic()
        {
            var d = C.Get<IDocument>();
            Assert.That(d, Is.InstanceOfType(typeof(Document)));
        }
        [Test]
        public void TestSingletonIs()
        {
            var d = C.Get<IDocument>();
            var e = C.Get<IDocument>();
            Assert.IsTrue(Object.ReferenceEquals(d,e));
        }
        [Test]
        public void TestInjectorCalled()
        {
            var id = C.Get<IDocument>();
            var d = (Document) id;

            Assert.IsInstanceOfType(typeof(ComponentA),d.A);
            Assert.IsInstanceOfType(typeof(ComponentB), d.B);
        }
        [Test]
        public void TestConstructorInjectorCalled()
        {
            var id = C.Get<IDocument>();
            var d = (Document)id;
            var b = (ComponentB)d.B;
            Assert.AreSame(d,b.Document);
        }
        [Test]
        [ExpectedException(typeof(CircularConstructorDependencyException))]
        public void TestCyclicsDeterminedBeforeCreation()
        {
            C = ContextSystem.CreateContextForTesting();
            C.RegisterSingleton<Document, IDocument>();
            C.RegisterSingleton<ComponentA, IComponentA>();
            C.RegisterSingleton<ComponentB, IComponentB>();
            C.RegisterSingleton<CyclicComponentC, IComponentC>();
            var id = C.Get<IDocument>();
        }
        [Test]
        public void TestInjectionComplete()
        {
            var id = C.Get<IDocument>();
            var d = (Document)id;
            var b = (ComponentB)d.B;

            Assert.IsTrue(d.InjectionCompleteHandlerCalled);
            Assert.IsTrue(b.InjectionCompleteHandlerCalled);
        }

        [Test]
        public void TestManualInject()
        {
            C.RegisterSingleton<SomeImplementation,ISomeInterface>();
            var c = new ComponentC();
            Assert.IsFalse(c.InjectionCompleteHandlerCalled);
            C.Inject(c);
            Assert.IsTrue(c.InjectionCompleteHandlerCalled);
            Assert.IsNotNull(c.Something);
        }

        [Test]
        public void TestInjectorRegistered()
        {
            var i = C.Get<IInjector>();
            Assert.IsNotNull(i);
        }
        [Test]
        public void TestContextInjected()
        {
            C.RegisterSingleton<NeedsContext>();
            var t = C.Get<NeedsContext>();
            Assert.AreSame(C,t.Context);
        }
        [Test]
        public void TestCanInjectObjectsThatCannotBeCreatedBySystem()
        {
            var p = ClassWithPrivateConstructor.Make();
            C.Inject(p);
            Assert.AreSame(C, p.Context);
        }
        [Test]
        [ExpectedException(typeof(NoSuiteableInjectionConstructorException))]
        public void TestObjectsThatCannotBeCreatedBySystemThrow()
        {
            C.RegisterSingleton<ClassWithPrivateConstructor>();
            C.Get<ClassWithPrivateConstructor>();
        }

        [Test]
        public void CanRegisterExistingObjectAsSingleton()
        {
            var p = ClassWithPrivateConstructor.Make();
            C.RegisterSingleton<Meep>();
            C.RegisterSingleton(p);

            Meep m = C.Get<Meep>();
            Assert.AreSame(p,m.Other);

        }

    }
}
