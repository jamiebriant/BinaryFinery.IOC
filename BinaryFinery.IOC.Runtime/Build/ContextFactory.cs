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

        private Queue<string> currentRun = null;

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

        public Type TypeForProperty(string property)
        {
            var ti = contextType;
            Type[] ifaces = contextType.GetInterfaces();
            int i = 0;
            while (i < ifaces.Length)
            {
                var info = ti.GetProperty(property);
                if (info != null)
                    return info.PropertyType;
                ++i;
                ti = ifaces[i];
            }
            return null;
        }

        public Type ImplementationTypeForProperty(string property)
        {

            // find the most recent declaration.

            var ti = contextType;
            Type[] ifaces = contextType.GetInterfaces();
            int i = 0;
            Type guess = null;
            Type imp = null;
            Type impContext = null;
            while ( i < ifaces.Length)
            {
                var info = ti.GetProperty(property);
                if (info != null )
                {
                    if (guess == null)
                    {
                        guess = info.PropertyType;
                    }
                    var attrs2 = info.GetCustomAttributes(typeof(ImplementationAttribute), true);
                    if (attrs2.Length > 0)
                    {
                        var attr2 = (ImplementationAttribute) attrs2[0];
                        Type timp = attr2.Type;
                        if (imp == null)
                        {
                            imp = timp;
                            impContext = ti;
                            if (!guess.IsAssignableFrom(imp))
                            {
                                throw new ImplementationInterfaceMismatchException(contextType, imp, guess,ti);
                            }
                        }
                        else
                        {
                            if ( !timp.IsAssignableFrom(imp))
                                throw new ImplementationsMismatchException(contextType, imp, impContext, timp, ti);
                        }
                    }
                }
                ti = ifaces[i];
                ++i;
            }
            return imp ?? guess;
        }

        public object ObjectForProperty(string propertyName)
        {
            if ( currentRun == null)
            {
                currentRun = new Queue<String>();
            }
            if (currentRun.Contains(propertyName))
            {
                throw new CyclicDependencyException(this.contextType);
            }

            object rv;
            if (singletons.TryGetValue(propertyName, out rv))
            {
                return rv;
            }
            Type type = ImplementationTypeForProperty(propertyName);
            try
            {
                currentRun.Enqueue(propertyName);
                // find constructor
                ConstructorInfo ctor = GetCtor(type);
                var parameters = ctor.GetParameters();
                object[] args = new object[parameters.Length];
                for (int i = 0; i < args.Length; ++i)
                {
                    PropertyInfo pi = PropertyForType(parameters[i].ParameterType,propertyName);

                    args[i] = ObjectForProperty(pi.Name);
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

        private PropertyInfo PropertyForType(Type parameterType, string dependentProperty)
        {
            var ti = contextType;
            Type[] ifaces = contextType.GetInterfaces();
            int i = ifaces.Length;
            while (i >= 0)
            {
                --i;
                Type iface = i >=0 ? ifaces[i] : contextType;
                var pi = iface.GetProperty(dependentProperty);
                if (pi != null)
                {
                    if (!pi.PropertyType.IsInterface || pi.GetCustomAttributes(typeof(ImplementationAttribute), false).Length>0)
                    {
                        // found a possible match.
                        var dt = PropertyForType(parameterType, iface);
                        if (dt != null)
                            return dt;
                    }
                }
            }
            return null;
        }
        private PropertyInfo PropertyForType(Type parameterType, Type startingContext)
        {

        var props =
                startingContext.GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy |
                                          BindingFlags.Instance);
            foreach (var propertyInfo in props)
            {
                if (propertyInfo.PropertyType == parameterType ||
                    parameterType.IsAssignableFrom(propertyInfo.PropertyType))
                {
                    return propertyInfo;
                }
                Type impType = ImplementationTypeForProperty(propertyInfo.Name);
                if (impType == parameterType || parameterType.IsAssignableFrom(impType))
                {
                    return propertyInfo;
                }
            }
            return null;
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