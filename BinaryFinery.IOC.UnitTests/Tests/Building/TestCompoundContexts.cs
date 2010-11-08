using BinaryFinery.IOC.Runtime.Build;
using BinaryFinery.IOC.UnitTests.Inputs;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace BinaryFinery.IOC.UnitTests.Tests.Building
{
    public class TestCompoundContexts
    {
        private ContextManager CM;

        [SetUp]
        public void Setup()
        {
            CM = ContextSystem.Manager;
            CM.RegisterCustomContextImplementation(typeof(CompoundContextImp), typeof(ICompoundContext));
        }

        [Test]
        public void TestContextsThatDontActuallyDefineAnythingStillWork()
        {
            var context = CM.Create<ICompoundContext>();
            var a = context.A;
            Assert.That(a, Is.Not.Null);
        }


        [Test]
        // To resolve conflicts between props of the same type, a prop should look up using the prop declared along side itself, i.e. in its initial context
        // In this case, Deps was declared in the context with B, so B should provide the IFoo. To do this we need to scan up to find where B was originally declared.
        public void DepsFooShouldBeOtherFoo()
        {
            var context = CM.Create<ICompoundContext>();
            var d = context.Deps;
            Assert.IsInstanceOfType(typeof(OtherFoo),d.Myfoo);
        }

    }
}
