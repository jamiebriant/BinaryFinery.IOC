using BinaryFinery.IOC.Runtime;
using BinaryFinery.IOC.Runtime.Meta;
using BinaryFinery.IOC.UnitTests.Tests.Meta;

namespace BinaryFinery.IOC.UnitTests.Inputs
{
    public interface IFoo
    {

    }

    public class Foo : IFoo
    {

    }

    public class FooToo : Foo
    {

    }

    public class OtherFoo : IFoo
    {

    }


    public interface IRootContext : IContext
    {
        Foo FooP { get;  }
    }

    public class Bar
    {

    }

    public interface IDodgyImplContext : IContext
    {
        [Implementation(typeof(Bar))]
        IFoo FooP { get; }
    }


    public interface IDodgyDerivedImplementationContext : IImplementationTestContext
    {
        [Implementation(typeof(OtherFoo))]
        IFoo FooP { get; }
    }

    public interface IPointlessDerivedImplementationContext : IImplementationTestContext
    {
        IFoo FooP { get; }
    }
}