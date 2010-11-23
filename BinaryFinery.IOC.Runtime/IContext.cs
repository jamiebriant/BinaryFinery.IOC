// 
// Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
namespace BinaryFinery.IOC.Runtime
{
    public interface IContext : IInjector
    {
    }

    public interface IInjector
    {
        void Inject(object injectee);
    }
}