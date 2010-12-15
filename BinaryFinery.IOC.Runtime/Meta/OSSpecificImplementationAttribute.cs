using System;

namespace BinaryFinery.IOC.Runtime.Meta
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class OSSpecificImplementationAttribute : ImplementationAttribute
    {
        private readonly PlatformID platformId;

        public OSSpecificImplementationAttribute(Type type, PlatformID platformId) : base(type)
        {
            this.platformId = platformId;
        }

        public OSSpecificImplementationAttribute(Type type, InstantiationTiming timing, PlatformID platformId) : base(type, timing)
        {
            this.platformId = platformId;
        }

        public override bool Applies
        {
            get { return Environment.OSVersion.Platform == platformId; }
        }
    }
}