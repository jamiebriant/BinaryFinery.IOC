using System;
using System.Collections.Generic;
using System.Reflection;

namespace BinaryFinery.IOC.Runtime.Builder
{
    internal class BuildNode
    {
        private readonly Type type;

        public BuildNode(Type type)
        {
            this.type = type;
            Injections = new List<PropertyInfo>();
        }

        public Type Type
        {
            get { return type; }
        }

        public IEnumerable<PropertyInfo> Injections { get; internal set; }

        public ConstructorInfo Constructor { get; set; }

        public List<MethodInfo> Handlers { get; internal set; }

        public bool HasHandlers
        {
            get { return Handlers.Count>0; }
        }
    }
}