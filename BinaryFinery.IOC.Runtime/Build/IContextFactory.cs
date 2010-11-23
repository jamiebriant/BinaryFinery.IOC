using System;

namespace BinaryFinery.IOC.Runtime.Build
{
    public interface IContextFactory
    {
        Type ContextType { get; }
        Type TypeForProperty(string foop);
        Type ImplementationTypeForPropertyForTesting(string property);
        object ObjectForProperty(string propertyName);
   } 
}