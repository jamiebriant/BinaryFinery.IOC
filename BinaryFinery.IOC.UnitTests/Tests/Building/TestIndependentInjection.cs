using BinaryFinery.IOC.Runtime;
using BinaryFinery.IOC.Runtime.Build;
using BinaryFinery.IOC.Runtime.Meta;
using BinaryFinery.IOC.UnitTests.Inputs;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace BinaryFinery.IOC.UnitTests.Tests.Building
{

    public class ContextRequiringFoo : IFoo
    {
        [Inject]
        public IInjectionContext Context { get; set; }
    }

    public interface IInjectionContext : IDependencyTestBaseContext
    {
        [Implementation(typeof(ContextRequiringFoo))]
        IFoo FooP { get; }

        [Implementation(typeof(DepProp))]
        IDeps DepsP { get; }
    }

    public class AnotherDependencyTestContextImpl : DependencyTestContextImpl, IInjectionContext
    {
        
    }


    public class TestIndependentInjection
    {
        private ContextManager CM;

        [SetUp]
        public void Setup()
        {
            CM = ContextSystem.Manager;
            CM.RegisterCustomContextImplementation(typeof(AnotherDependencyTestContextImpl));
        }

        [Test]
        public void TestObjectGetsContextInjectedIfAsked()
        {
            var context = CM.Create<IInjectionContext>();

            IFoo foo = context.FooP;
            ContextRequiringFoo crf = (ContextRequiringFoo) foo;
            Assert.That(crf.Context, Is.EqualTo(context));

        }

        class INeedFoo
        {
            [Inject]
            public IFoo Fooooo { get; set; }
        }

        [Test]
        public void TestContextCanManuallyInjectObjectsIntoOthers()
        {
            var context = CM.Create<IInjectionContext>();
            var inf = new INeedFoo();
            Assert.That(inf.Fooooo,Is.Null);
            context.Inject(inf);
            Assert.That(inf.Fooooo, Is.Not.Null);
        }
    }
}
