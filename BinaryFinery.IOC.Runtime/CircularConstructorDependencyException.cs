using System;
using System.Collections.Generic;
using System.Text;
using BinaryFinery.IOC.Runtime.Builder;

namespace BinaryFinery.IOC.Runtime
{
    public class CircularConstructorDependencyException : InjectionException
    {
        private readonly IEnumerable<TypeKey> cycle;

        public CircularConstructorDependencyException(IEnumerable<TypeKey> cycle)
        {
            this.cycle = cycle;
        }

        public override string Message
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(GetType().Name);
                sb.Append(":\n");
                foreach (var typeKey in Cycle)
                {
                    sb.AppendFormat("{0}\n", typeKey);
                }
                return sb.ToString();
            }
        }

        public IEnumerable<TypeKey> Cycle
        {
            get { return cycle; }
        }
    }
}