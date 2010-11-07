// 
// Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System;
using System.Collections.Generic;
using System.Reflection;
using BinaryFinery.IOC.Runtime.Meta;

namespace BinaryFinery.IOC.Runtime.Build
{
    public interface IContextFactory
    {
        Type ContextType { get; }
        Type TypeForProperty(string foop);
        Type ImplementationTypeForProperty(string property);
        object ObjectForProperty(string propertyName);
        TContext Create<TContext>() where TContext : class, IContext;
    }


    public class BaseContextImpl : IContext
    {
        private IContextFactory factory;

        internal void SetFactory(IContextFactory factory)
        {
            this.factory = factory;
        }

        protected IContextFactory Factory
        {
            get { return factory; }
        }
    }

    internal class ContextFactory : IContextFactory
    {
        private readonly Type contextType;
        private readonly Type custom;
        internal readonly Dictionary<string, object> singletons = new Dictionary<string, object>();
        internal readonly Dictionary<Type, object> singletonsByType = new Dictionary<Type, object>();

        private Queue<Type> currentRun = null;

        public ContextFactory(Type contextType, Type custom)
        {
            this.contextType = contextType;
            this.custom = custom;
        }

        public ContextFactory(Type contextType)
        {
            this.contextType = contextType;
        }

        public Type ContextType
        {
            get { return contextType; }
        }

        public Type TypeForProperty(string foop)
        {
            return contextType.GetProperty(foop).PropertyType;
        }

        public Type ImplementationTypeForProperty(string property)
        {
            var info = contextType.GetProperty(property);
            var attrs = info.GetCustomAttributes(typeof(ImplementationAttribute), true);
            Type propertyType = info.PropertyType;
            if (attrs.Length == 0)
                return propertyType;
            ImplementationAttribute attr = (ImplementationAttribute) attrs[0];

            Type type = attr.Type;
            if (!propertyType.IsAssignableFrom(type))
            {
                throw new ImplementationInterfaceMismatchException(this.contextType, type, propertyType);
            }
            Type[] ifaces = contextType.GetInterfaces();
            foreach (var iface in ifaces)
            {
                var test = iface.GetProperty(property);
                if (test != null)
                {
                    var attrs2 = test.GetCustomAttributes(typeof(ImplementationAttribute), false);
                    if (attrs2.Length == 0)
                        continue;
                    ImplementationAttribute attr2 = (ImplementationAttribute) attrs2[0];
                    if (!attr2.Type.IsAssignableFrom(type))
                    {
                        throw new ImplementationsMismatchException(contextType, type, attr2.Type, iface);
                    }
                }
            }
            return type;
        }

        public object ObjectForProperty(string propertyName)
        {
            if ( currentRun == null)
            {
                currentRun = new Queue<Type>();
            }
            object rv;
            if (singletons.TryGetValue(propertyName, out rv))
            {
                return rv;
            }
            Type type = ImplementationTypeForProperty(propertyName);
            try
            {
                currentRun.Enqueue(type);
                // find constructor
                ConstructorInfo ctor = GetCtor(type);
                var parameters = ctor.GetParameters();
                object[] args = new object[parameters.Length];
                for (int i = 0; i < args.Length; ++i)
                {
                    args[i] = ObjectForType(parameters[i].ParameterType);
                }

                rv = Activator.CreateInstance(type, args);
                singletons[propertyName] = rv;
                singletonsByType[TypeForProperty(propertyName)] = type;
                return rv;
            }
            finally
            {
                currentRun.Dequeue();
                if (currentRun.Count == 0) currentRun = null;
            }
        }

        internal ConstructorInfo GetCtor(Type type)
        {
            var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            ConstructorInfo ctor = null;
            foreach (var constructorInfo in ctors)
            {
                var inject = constructorInfo.GetCustomAttributes(typeof(InjectAttribute), false);
                if (inject.Length > 0)
                {
                    ctor = constructorInfo;
                    break;
                }
                if (ctor == null)
                {
                    ctor = constructorInfo;
                }
            }
            return ctor;
        }

        private object ObjectForType(Type parameterType)
        {
            object rv;
            if (singletonsByType.TryGetValue(parameterType, out rv))
            {
                return rv;
            }
            if (currentRun.Contains(parameterType))
            {
                throw new CyclicDependencyException(this.contextType);
            }
            currentRun.Enqueue(parameterType);
            try
            {
                var props =
                    contextType.GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy |
                                              BindingFlags.Instance);
                foreach (var propertyInfo in props)
                {
                    if (propertyInfo.PropertyType == parameterType ||
                        parameterType.IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        return ObjectForProperty(propertyInfo.Name);
                    }
                    Type impType = ImplementationTypeForProperty(propertyInfo.Name);
                    if (impType == parameterType || parameterType.IsAssignableFrom(impType))
                    {
                        return ObjectForProperty(propertyInfo.Name);
                    }
                }
                return null;
            }
            finally
            {
                currentRun.Dequeue();
            }
        }

        public TContext Create<TContext>()
            where TContext : class, IContext
        {
            object result = Activator.CreateInstance(custom);
            BaseContextImpl impl = (BaseContextImpl) result;
            impl.SetFactory(this);
            TContext rv = (TContext) result;
            return rv;
        }
    }
}