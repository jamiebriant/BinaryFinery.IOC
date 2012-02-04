using System;

namespace BinaryFinery.IOC.Runtime
{
    public class NoSuiteableInjectionConstructorException : InjectionException
    {
        private readonly Type type;

        public NoSuiteableInjectionConstructorException(Type type)
        {
            this.type = type;
        }
    }
}