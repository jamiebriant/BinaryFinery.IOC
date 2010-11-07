// 
// Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System;

namespace BinaryFinery.IOC.Runtime.Meta
{
    public class ImplementationAttribute : Attribute
    {
        private readonly Type type;

        public ImplementationAttribute(Type type)
        {
            this.type = type;
        }

        public Type Type
        {
            get { return type; }
        }
    }
}