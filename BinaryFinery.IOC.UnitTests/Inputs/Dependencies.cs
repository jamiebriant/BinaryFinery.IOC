using BinaryFinery.IOC.Runtime;
using BinaryFinery.IOC.Runtime.Build;
using BinaryFinery.IOC.Runtime.Meta;

namespace BinaryFinery.IOC.UnitTests.Inputs
{

    public interface IDeps
    {
        Foo Myfoo { get; }
    }

    public class Deps : IDeps
    {
        private readonly Foo myfoo;

        public Deps( Foo myfoo)
        {
            this.myfoo = myfoo;
        }

        public Foo Myfoo
        {
            get { return myfoo; }
        }
    }

    public class FooDep : Foo
    {
        private readonly IDeps deps;

        public FooDep( IDeps deps)
        {
            this.deps = deps;
        }

        public IDeps Deps
        {
            get { return deps; }
        }
    }

    public interface IDependencyTestContext : IContext
    {
        [Implementation(typeof(Foo))]
        IFoo FooP { get; }
        [Implementation(typeof(Deps))]
        IDeps DepsP { get; }
    }

    public class Deps2 : Deps
    {
        private Deps2(Foo myfoo) : base(myfoo)
        {
        }

        public Deps2() : base(null)
        {
        }
    }

    public class Deps2a : Deps
    {
        public Deps2a(Foo myfoo)
            : base(myfoo)
        {
        }

        public Deps2a()
            : base(null)
        {
        }
    }

    public class DepsAttributed : Deps
    {
        public DepsAttributed(Foo myfoo)
            : base(myfoo)
        {
        }

        [Inject]
        public DepsAttributed()
            : base(new Foo())
        {
        }
    }

    public interface IDependencyTestContext2 : IDependencyTestContext
    {
        [Implementation(typeof(Foo))]
        IFoo FooP { get; }
        [Implementation(typeof(Deps2))]
        IDeps DepsP { get; }
    }

    public interface IDependencyTestContext2a : IDependencyTestContext
    {
        [Implementation(typeof(Foo))]
        IFoo FooP { get; }
        [Implementation(typeof(Deps2a))]
        IDeps DepsP { get; }
    }

    public interface IDependencyTestContextAttributed : IDependencyTestContext
    {
        [Implementation(typeof(Foo))]
        IFoo FooP { get; }
        [Implementation(typeof(DepsAttributed))]
        IDeps DepsP { get; }
    }

    public interface IDependencyTestCyclic : IDependencyTestContext
    {
        [Implementation(typeof(FooDep))]
        IFoo FooP { get; }
        [Implementation(typeof(Deps))]
        IDeps DepsP { get; }
    }


    public class DependencyTestContextImpl : BaseContextImpl
    {


        public IFoo FooP
        {
            get { return (IFoo) Factory.ObjectForProperty("FooP"); }
        }

        public IDeps DepsP
        {
            get { return (IDeps) Factory.ObjectForProperty("DepsP"); ; }
        }
    }

    public class DependencyTestContextTop : DependencyTestContextImpl, IDependencyTestContextAttributed, IDependencyTestContext2, IDependencyTestContext2a, IDependencyTestContext, IDependencyTestCyclic
    {
    }

}
