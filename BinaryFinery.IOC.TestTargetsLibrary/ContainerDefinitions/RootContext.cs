// 
// Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System;
using System.Reflection;
using BinaryFinery.IOC.Runtime.Meta;

namespace BinaryFinery.IOC.TestTargetsLibrary.ContainerDefinitions
{
    [Context]
    public interface IRootContext
    {
        ILog Logger { get; }
        IThreadServices ThreadServices { get; }
        IFileServices FileServices { get; }
    }

    [Context]
    public interface IOurRootContext : IRootContext
    {
        [Implementation(typeof(MyLogger))]
        ILog Logger { get; }

        IThreadServices ThreadServices { get; }
    }

    [Context(typeof(IOurRootContext))]
    internal interface IAppContext
    {
    }

    internal class MyRootContext : IOurRootContext
    {
        private readonly IContextFactory factory;

        public MyRootContext(IContextFactory factory)
        {
            this.factory = factory;
        }

        private ILog _Logger;
        private static IFactoryNode _Logger_Node;

        public ILog Logger
        {
            get
            {
                if (_Logger_Node == null)
                {
                    _Logger_Node = factory.CreateNode(typeof(IOurRootContext),
                                                      typeof(IOurRootContext).GetProperty("Logger"));
                }
                return _Logger_Node.Create<ILog>(factory);
            }
        }

        public IThreadServices ThreadServices
        {
            get { return null; }
        }

        public IFileServices FileServices
        {
            get { return null; }
        }
    }

    internal interface IFactoryNode
    {
        T Create<T>(IContextFactory factory);
    }

    internal interface IContextFactory
    {
        IFactoryNode CreateNode(Type type, PropertyInfo getProperty);
    }


    public class MyLogger : ILog
    {
    }

    public interface IFileServices
    {
    }

    public interface IThreadServices
    {
    }

    public interface ILog
    {
    }
}