// 
// Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System;
using System.Reflection;
using BinaryFinery.IOC.Runtime.Build;
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