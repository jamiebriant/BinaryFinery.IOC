using System;
using System.Collections.Generic;
using System.Text;
using BinaryFinery.IOC.Runtime;
using BinaryFinery.IOC.Runtime.Build;
using BinaryFinery.IOC.Runtime.Meta;

namespace BinaryFinery.IOC.UnitTests.Inputs
{
    public interface ILocationDependencyContext : IContext
    {
        IFoo A { get; }
    }

    public interface IAnotherLocationDependencyContext : IContext
    {
        IFoo B { get; }
        IDontCareDeps Deps { get; }
    }

    /*************************************************/

    public interface ILocationDependencyContext2 : ILocationDependencyContext
    {
        [Implementation(typeof(Foo))]
        IFoo A { get; }
    }

    public interface IAnotherLocationDependencyContext2 : IAnotherLocationDependencyContext
    {
        [Implementation(typeof(OtherFoo))]
        IFoo B { get; }

        [Implementation(typeof(DontCareDeps))]
        IDontCareDeps Deps { get; }
    }

    /***************************************************/

    public interface ICompoundContext : ILocationDependencyContext2, IAnotherLocationDependencyContext2
    {
        
    }

    public class CompoundContextImp : BaseContextImpl, ICompoundContext
    {
        public IFoo A
        {
            get { return (IFoo) ObjectForProperty("A"); }
        }

        public IFoo B
        {
            get { return (IFoo) ObjectForProperty("B"); }
        }

        public IDontCareDeps Deps
        {
            get { return (IDontCareDeps) ObjectForProperty("Deps"); }
        }
    }

    /***********************************************************************************/

    public interface IFragmentedContext : IContext
    {
        [Implementation(typeof(DontCareDeps))]
        IDontCareDeps Deps { get; }
    }

    public interface IMissingPieceContext : IContext
    {
        [Implementation(typeof(OtherFoo))]
        IFoo B { get; }
    }

    public interface IJoinContext : IFragmentedContext, IMissingPieceContext
    {
        
    }

    public class JoinContextImp : BaseContextImpl, IJoinContext
    {
        public IFoo B
        {
            get { return (IFoo)ObjectForProperty("B"); }
        }

        public IDontCareDeps Deps
        {
            get { return (IDontCareDeps)ObjectForProperty("Deps"); }
        }
    }

    /**********************************************************************/

    public interface IWorkingJoinContext : IFragmentedContext, IMissingPieceContext
    {
        [Implementation(typeof(DontCareDeps))]
        IDontCareDeps Deps { get; }
    }

    public class WorkingJoinContextImp : BaseContextImpl, IWorkingJoinContext
    {
        public IFoo B
        {
            get { return (IFoo)ObjectForProperty("B"); }
        }

        public IDontCareDeps Deps
        {
            get { return (IDontCareDeps)ObjectForProperty("Deps"); }
        }
    }


}
