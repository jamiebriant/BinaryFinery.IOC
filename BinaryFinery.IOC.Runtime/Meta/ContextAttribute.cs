using System;

namespace BinaryFinery.IOC.Runtime.Meta
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ContextAttribute : Attribute
    {
        private readonly Type parent;

        public ContextAttribute()
        {
        }

        public ContextAttribute( Type parent)
        {
            this.parent = parent;
        }

        public Type Parent
        {
            get { return parent; }
        }
    }
}
