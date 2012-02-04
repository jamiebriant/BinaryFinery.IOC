using System;
using BinaryFinery.IOC.Runtime.Builder;

namespace BinaryFinery.IOC.Runtime
{
    public class TypeNotFoundException : InjectionException
    {
        private readonly Context context;
        private readonly Type type;

        public TypeNotFoundException(Context context, Type type)
        {
            this.context = context;
            this.type = type;
        }

        public override string Message
        {
            get
            {
                return string.Format("{0}\nContext: {1}, Type: {2}", GetType(), context, type);
            }
        }

    }
}