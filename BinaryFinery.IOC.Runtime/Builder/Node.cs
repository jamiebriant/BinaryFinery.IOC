using System;

namespace BinaryFinery.IOC.Runtime.Builder
{
    class Node
    {
        private readonly TypeKey typeKey;

        public Node(object what, Type type)
        {
            this.typeKey = new TypeKey(type);
            this.Value = what;
        }

        public Node(object what)
        {
            this.typeKey = new TypeKey(what.GetType());
            this.Value = what;
        }

        public Node(TypeKey typeKey)
        {
            this.typeKey = typeKey;
        }

        public Type Build
        {
            get { return TypeKey.Type; }
        }

        public object Value { get; set; }

        public TypeKey TypeKey
        {
            get { return typeKey; }
        }
    }
}