// 
// Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
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

        public Deps(Foo myfoo)
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

        public FooDep(IDeps deps)
        {
            this.deps = deps;
        }

        public IDeps Deps
        {
            get { return deps; }
        }
    }

    public interface IDependencyTestBaseContext : IContext
    {
        IFoo FooP { get; }
        IDeps DepsP { get; }
    }


    public interface IDependencyTestContext : IDependencyTestBaseContext
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

    public class DepProp : IDeps
    {
        private Foo foo;

        [Inject]
        public virtual Foo Myfoo
        {
            get { return Foo; }
            set { foo = value; }
        }

        public Foo Foo
        {
            get { return foo; }
        }
    }

    public interface IDependencyTestProperyInjection : IDependencyTestBaseContext
    {
        [Implementation(typeof(Foo))]
        IFoo FooP { get; }

        [Implementation(typeof(DepProp))]
        IDeps DepsP { get; }

    }




    public class DependencyTestContextImpl : BaseContextImpl, IDependencyTestBaseContext
    {
        public IFoo FooP
        {
            get { return (IFoo) ObjectForProperty("FooP"); }
        }

        public IDeps DepsP
        {
            get { return (IDeps) ObjectForProperty("DepsP"); }
        }
    }

    /************************************************************************************************
     * 
     * METHOD INJECTION
     * 
     */


    public class DepProp2 : DepProp
    {
        private Foo foo2;

        /* NOTE: No inject */
        public override Foo Myfoo
        {
            get { return Foo2; }
            set { foo2 = value; }
        }

        public Foo Foo2
        {
            get { return foo2; }
        }
    }

    public interface IDependencyTestProperyInjection2 : IDependencyTestBaseContext
    {
        [Implementation(typeof(Foo))]
        IFoo FooP { get; }

        [Implementation(typeof(DepProp2))]
        IDeps DepsP { get; }

    }

    public class DepMethod : IDeps
    {
        private Foo foo;

        public Foo Myfoo
        {
            get { return Foo; }
        }

        public Foo Foo
        {
            get { return foo; }
        }

        [Inject]
        virtual public bool SetFoo(Foo foo)
        {
            this.foo = foo;
            return false;
        }
    }

    public interface IDependencyTestMethodInjection : IDependencyTestBaseContext
    {
        [Implementation(typeof(Foo))]
        IFoo FooP { get; }

        [Implementation(typeof(DepMethod))]
        IDeps DepsP { get; }

    }

    public class DepMethod2 : DepMethod
    {
        private Foo foo2;

        public Foo Foo2
        {
            get { return foo2; }
        }

        public override bool SetFoo(Foo foo)
        {
            this.foo2 = foo;
            return false;
        }
    }
    public interface IDependencyTestMethodInjection2 : IDependencyTestBaseContext
    {
        [Implementation(typeof(Foo))]
        IFoo FooP { get; }

        [Implementation(typeof(DepMethod2))]
        IDeps DepsP { get; }

    }


    /************************************************************************************************
     * The manual context class.
     */




    public class DependencyTestContextTop : DependencyTestContextImpl, IDependencyTestContextAttributed, IDependencyTestProperyInjection2,
                                            IDependencyTestContext2, IDependencyTestContext2a, IDependencyTestContext,
                                            IDependencyTestCyclic, IDependencyTestProperyInjection, IDependencyTestMethodInjection, IDependencyTestMethodInjection2
    {
    }
}