// 
// Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System;
using System.Collections.Generic;

namespace BinaryFinery.IOC.Runtime.Build
{
    public class ContextSystem
    {
        private static readonly ContextManager manager = new ContextManager();

        public static ContextManager Manager
        {
            get { return manager; }
        }

        public static ContextManager ManagerForTesting
        {
            get { return new ContextManager(); }
        }

    }


    public class ContextManager
    {
        private readonly Dictionary<Type, Type> customImplementations = new Dictionary<Type, Type>();

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

        public void RegisterCustomContextImplementation(Type implementation)
        {
            var ifaces = implementation.GetInterfaces();
            foreach (var iface in ifaces)
            {
                customImplementations[iface] = implementation;
            }
        }
    }
}