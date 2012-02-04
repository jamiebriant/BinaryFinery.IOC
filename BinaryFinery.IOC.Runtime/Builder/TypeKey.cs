using System;

namespace BinaryFinery.IOC.Runtime.Builder
{
    public struct TypeKey
    {
        private readonly Type type;
        private readonly string code;

        public TypeKey(Type type, string code)
        {
            this.type = type;
            this.code = code;
        }

        public TypeKey(Type type) : this()
        {
            this.type = type;
        }

        public Type Type
        {
            get { return type; }
        }

        public bool Equals(TypeKey other)
        {
            return Equals(other.Type, Type) && Equals(other.code, code);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(TypeKey)) return false;
            return Equals((TypeKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0)*397) ^ (code != null ? code.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Type: {0}, Code: {1}", type, code);
        }
    }
}