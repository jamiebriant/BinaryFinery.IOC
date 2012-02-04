using System;

namespace BinaryFinery.IOC.Runtime
{
    public class InjectionException : Exception
    {
        public InjectionException(string internalFailure) : base(internalFailure)
        {
        }

        public InjectionException()
        {
        }
    }
}