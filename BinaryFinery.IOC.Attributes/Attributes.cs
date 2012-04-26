using System;

namespace BinaryFinery.IOC.Attributes
{
    public class InjectAttribute : Attribute
    {
        public InjectAttribute()
        {
        }

        public bool Optional { get; set; }
    }

    public class InjectionCompleteHandlerAttribute : Attribute
    {
    }
}
