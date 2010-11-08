// 
// Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System;

namespace BinaryFinery.IOC.Runtime.Build
{
    public class BuildException : Exception
    {
        private readonly Type contextType;

        public BuildException(Type contextType)
        {
            this.contextType = contextType;
        }

        public Type ContextType
        {
            get { return contextType; }
        }

        public override string ToString()
        {
            return string.Format("{0}\nDetails:\n ContextType: {1}\n", base.ToString(), contextType);
        }
    }

    public class ImplementationInterfaceMismatchException : BuildException
    {
        private readonly Type implementationTypesContext;
        private readonly Type implementationType;
        private readonly Type requiredType;

        public ImplementationInterfaceMismatchException(Type contextType, Type implementationType, Type requiredType, Type implementationTypesContext)
            : base(contextType)
        {
            this.implementationType = implementationType;
            this.implementationTypesContext = implementationTypesContext;
            this.requiredType = requiredType;
        }

        public Type ImplementationType
        {
            get { return implementationType; }
        }

        public Type RequiredType
        {
            get { return requiredType; }
        }

        public override string ToString()
        {
            return string.Format("{0} ImplementationType: {1},\n RequiredType: {2}\n", base.ToString(),
                                 implementationType, requiredType);
        }
    }

    public class ImplementationsMismatchException : BuildException
    {
        private readonly Type implementationType;
        private readonly Type implementationTypesContext;
        private readonly Type baseImplementationType;
        private readonly Type baseContext;

        public ImplementationsMismatchException(Type contextType, Type implementationType, Type implementationTypesContext, Type baseImplementationType,
                                                Type baseContext)
            : base(contextType)
        {
            this.implementationType = implementationType;
            this.implementationTypesContext = implementationTypesContext;
            this.baseImplementationType = baseImplementationType;
            this.baseContext = baseContext;
        }

        public Type BaseContext
        {
            get { return baseContext; }
        }

        public Type BaseImplementationType
        {
            get { return baseImplementationType; }
        }

        public Type ImplementationType
        {
            get { return implementationType; }
        }

        public Type ImplementationTypesContext
        {
            get { return implementationTypesContext; }
        }

        public override string ToString()
        {
            return string.Format("{0} ImplementationType: {1},\n BaseImplementationType: {2},\n BaseContext: {3}",
                                 base.ToString(), implementationType, baseImplementationType, baseContext);
        }
    }

    public class CyclicDependencyException : BuildException
    {
        public CyclicDependencyException(Type contextType) : base(contextType)
        {
        }
    }
}