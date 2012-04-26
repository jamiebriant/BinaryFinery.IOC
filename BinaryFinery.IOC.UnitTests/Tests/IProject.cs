using BinaryFinery.IOC.Attributes;
using BinaryFinery.IOC.Runtime;
using NUnit.Framework;

namespace BinaryFinery.IOC.UnitTests.Tests.Services
{
    interface IProject
    {
    }

    public interface IDocument
    {
    }

    public interface IComponent
    {
    }

    public class Document : IDocument
    {
        private bool injectionCompleteHandlerCalled = false;

        public Document(IComponentC c)
        {
            this.C = c;
        }

        [Inject]
        public IComponentA A { get; set; }

        [Inject]
        public IComponentB B { get; set; }

        public IComponentC C { get; set; }

        public bool InjectionCompleteHandlerCalled
        {
            get { return injectionCompleteHandlerCalled; }
        }

        [InjectionCompleteHandler]
        public void InjectedThanks()
        {
            Assert.IsNotNull(A);
            Assert.IsNotNull(B);
            ComponentB b = (ComponentB) B;
            Assert.IsNotNull(b.A);
            injectionCompleteHandlerCalled = true;
        }

    }

    public interface IComponentA : IComponent { }
    public interface IComponentB : IComponent {
        Document Document { get; }
    }
    public interface IComponentC : IComponent { }
    public interface IComponentD : IComponent { }

    public class ComponentA : IComponentA
    {
    }

    public class ComponentB : IComponentB
    {
        private readonly Document document;
        private bool injectionCompleteHandlerCalled;

        public ComponentB(Document document)
        {
            this.document = document;
        }

        [Inject]
        public IComponentA A { get; set; }

        public Document Document
        {
            get { return document; }
        }

        public bool InjectionCompleteHandlerCalled
        {
            get { return injectionCompleteHandlerCalled; }
        }

        [InjectionCompleteHandler]
        public void InjectedThanks()
        {
            Assert.IsNotNull(Document);
            Assert.IsNotNull(Document.B);
            injectionCompleteHandlerCalled = true;
        }

    }

    public interface ISomeInterface
    {
    }

    public class SomeImplementation : ISomeInterface
    {
    
    }

    public class ComponentC : IComponentC
    {
        private bool injectionCompleteHandlerCalled;

        [Inject]
        public ISomeInterface Something
        {
            get;
            set;
        }

        public bool InjectionCompleteHandlerCalled
        {
            get { return injectionCompleteHandlerCalled; }
        }

        [InjectionCompleteHandler]
        public void InjectedThanks()
        {
            Assert.IsNotNull(Something);
            injectionCompleteHandlerCalled = true;
        }
    }


    public class CyclicComponentC : IComponentC
    {
        private readonly IDocument d;

        public CyclicComponentC(IDocument d)
        {
            this.d = d;
        }

        public ISomeInterface Shared { get; set; }
    }

    public class ComponentD : IComponentD
    {
        public ISomeInterface Shared { get; set; }
    }

    public class NeedsContext
    {
        [Inject]
        public IContext Context { get; set; }
    }

    public class ClassWithPrivateConstructor
    {
        public static ClassWithPrivateConstructor Make()
        {
            return new ClassWithPrivateConstructor();
        }

        private ClassWithPrivateConstructor()
        {
        }
        [Inject]
        public IContext Context { get; set; }
    }

    public class Meep
    {
        private readonly ClassWithPrivateConstructor other;

        public Meep(ClassWithPrivateConstructor other)
        {
            this.other = other;
        }

        public ClassWithPrivateConstructor Other
        {
            get { return other; }
        }
    }

}
