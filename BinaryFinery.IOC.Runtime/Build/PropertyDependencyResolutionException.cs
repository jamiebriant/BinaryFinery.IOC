using System;
using System.Reflection;

namespace BinaryFinery.IOC.Runtime.Build
{
    public class PropertyDependencyResolutionException : BuildException
    {
        private readonly PropertyInfo dependentProperty;
        private readonly Type parameterType;

        public PropertyDependencyResolutionException(Type contextType, PropertyInfo dependentProperty, Type parameterType)
            : base(contextType)
        {
            this.dependentProperty = dependentProperty;
            this.parameterType = parameterType;
        }

        public override string ToString()
        {
            return
                string.Format(
                    "The system attempted to find a property that provides type {2} that appears at or below an implementation attribute for type {1}. The error follows:\n{0} DependentProperty: {1}\n ParameterType: {2}",
                    base.ToString(), dependentProperty, parameterType);
        }
    }
}