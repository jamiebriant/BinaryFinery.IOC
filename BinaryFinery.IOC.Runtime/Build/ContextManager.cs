// 
// Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System;
using System.Collections.Generic;

namespace BinaryFinery.IOC.Runtime.Build
{
    public class ContextSystem
    {
        private static readonly ContextManager manager = new ContextManager(false);

        public static ContextManager Manager
        {
            get { return manager; }
        }

        public static ContextManager ManagerForTesting
        {
            get { return new ContextManager(true); }
        }

    }


    public class ContextManager
    {
        private readonly bool testing;
        private readonly Dictionary<Type, Type> customImplementations = new Dictionary<Type, Type>();


        internal ContextManager(bool testing)
        {
            this.testing = testing;
        }

        public T Create<T>()
            where T : class, IContext
        {
            ContextFactory factory = IntlGetFactory<T>();
            return factory.Create<T>();
        }

        public IContextFactory GetFactory<T>()
        {
            return IntlGetFactory<T>();
        }

        private ContextFactory IntlGetFactory<T>()
        {
            Type custom;
            if (customImplementations.TryGetValue(typeof(T), out custom))
            {
                return new ContextFactory(typeof(T), custom); // Shit you can't do in java.
            }
            else if (testing) 
                return new ContextFactory(typeof(T));
            else
            {
                throw new BuildException(typeof(T));
            }
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