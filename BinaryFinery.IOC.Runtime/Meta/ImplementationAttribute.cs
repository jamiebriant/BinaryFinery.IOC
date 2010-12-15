// 
// A modification of FileGenReflector: http://filegenreflector.codeplex.com/
// Copyright (c) 2008 (?) Jason R Bock
// Released under the Microsoft Public License:  http://filegenreflector.codeplex.com/license
// Modifications: Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System;

namespace BinaryFinery.IOC.Runtime.Meta
{
    public enum InstantiationTiming
    {
        Lazy, // Default
        Eager
    }

    public class ImplementationAttribute : Attribute
    {
        private readonly Type type;
        private readonly InstantiationTiming timing;

        public ImplementationAttribute(Type type)
        {
            this.type = type;
            this.timing = InstantiationTiming.Lazy;
        }

        public ImplementationAttribute(Type type, InstantiationTiming timing)
        {
            this.type = type;
            this.timing = timing;
        }

        public InstantiationTiming Timing
        {
            get { return timing; }
        }

        public Type Type
        {
            get { return type; }
        }

        public virtual bool Applies
        {
            get
            {
                return true;
            }
        }
    }
}