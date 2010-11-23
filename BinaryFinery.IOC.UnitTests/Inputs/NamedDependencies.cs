using System;
using System.Collections.Generic;
using System.Text;
using BinaryFinery.IOC.Runtime;
using BinaryFinery.IOC.Runtime.Build;
using BinaryFinery.IOC.Runtime.Meta;

namespace BinaryFinery.IOC.UnitTests.Inputs
{
    public interface INamedDependenciesContext : IContext
    {
        IFoo OtherFoo { get;  }
        IFoo Foo { get; }
        IDontCareDeps NeedsFoo { get; }
    }

    #region manual context impl
    public class NamedDependenciesContextBase : BaseContextImpl, INamedDependenciesContext
    {
        public IFoo OtherFoo
        {
            get { return (IFoo) ObjectForProperty("OtherFoo"); }
        }

        public IFoo Foo
        {
            get { return (IFoo) ObjectForProperty("Foo"); }
        }

        public IDontCareDeps NeedsFoo
        {
            get { return (IDontCareDeps)ObjectForProperty("NeedsFoo"); }
        }
    }
    #endregion

    public interface IDontCareDeps
    {
        IFoo Myfoo { get; }
    }

    public class DontCareDeps : IDontCareDeps
    {
        private readonly IFoo myfoo;

        public DontCareDeps(IFoo myfoo)
        {
            this.myfoo = myfoo;
        }

        public IFoo Myfoo
        {
            get { return myfoo; }
        }
    }


    public interface INamedDependenciesContext2 : INamedDependenciesContext
    {
        [Implementation(typeof(OtherFoo))]
        IFoo OtherFoo { get; }

        [Implementation(typeof(Foo))]
        IFoo Foo { get; }

        [Implementation(typeof(DontCareDeps))]
        IDontCareDeps NeedsFoo { get; }
    }

    public interface INamedDependenciesContext2a : INamedDependenciesContext
    {

        [Implementation(typeof(Foo))]
        IFoo Foo { get; }

        [Implementation(typeof(OtherFoo))]
        IFoo OtherFoo { get; }

        [Implementation(typeof(DontCareDeps))]
        IDontCareDeps NeedsFoo { get; }
    }

    public interface INamedDependenciesContext3 : INamedDependenciesContext2
    {

        [Implementation(typeof(OtherFoo))]
        IFoo OtherFoo { get; }

        [Implementation(typeof(Foo))]
        IFoo Foo { get; }

        [Implementation(typeof(DontCareDeps))]
        IDontCareDeps NeedsFoo { get; }
    }


    public class NamedDependenciesContextImpl : NamedDependenciesContextBase, INamedDependenciesContext2, INamedDependenciesContext2a { }
}
