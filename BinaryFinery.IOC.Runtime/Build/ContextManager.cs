using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryFinery.IOC.Runtime.Build
{
    public class ContextSystem
    {
        private static readonly ContextManager manager = new ContextManager();

        public static ContextManager Manager
        {
            get { return manager; }
        }
    }

    public class ContextManager
    {

        Dictionary<Type,Type> customImplementations = new Dictionary<Type, Type>();

        public T Create<T>()
            where T : class, IContext
        {
            IContextFactory factory = GetFactory<T>();
            return factory.Create<T>();
        }

        public IContextFactory GetFactory<T>()
        {
            Type custom;
            if (customImplementations.TryGetValue(typeof(T), out custom))
            {
                return new ContextFactory(typeof(T), custom); // Shit you can't do in java.
            }
            else return new ContextFactory(typeof(T));
        }

        public void RegisterCustomContextImplementation(Type implementation, Type context)
        {
//            Type current;
//            if (customImplementations.TryGetValue(context, out current))
//            {
//            }
            customImplementations[context] = implementation;
        }
    }
}
