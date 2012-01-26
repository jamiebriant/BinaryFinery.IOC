// 
// Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System;
using System.Collections.Generic;
using System.Reflection;
using BinaryFinery.IOC.Runtime.Meta;

namespace BinaryFinery.IOC.Runtime.Build
{
    internal class ContextFactory : IContextFactory
    {
        private readonly Type contextType;
        private readonly Type custom;
        readonly Dictionary<string,ConstructionNode> nodeMap = new Dictionary<string, ConstructionNode>();

        readonly Dictionary<string, object> singletons = new Dictionary<string, object>();
        readonly Dictionary<Type, object> singletonsByType = new Dictionary<Type, object>();

        private IContext externalContext; // what the world sees.

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
        
        public Type ImplementationTypeForPropertyForTesting(string property)
        {
            var cn = ImplementationTypeForPropertyBuilder(property);
            return cn == null ? null : cn.ImplementationType;
        }


        private ConstructionNode ImplementationTypeForProperty(string property)
        {
            // find the most recent declaration.
            ConstructionNode node = null;
            nodeMap.TryGetValue(property, out node);
            return node;
        }

        private ConstructionNode ImplementationTypeForPropertyBuilder(string property)
        {
            var ti = contextType;
            Type[] ifaces = contextType.GetInterfaces();
            int i = 0;
            Type imp = null;
            PropertyInfo impPropertyInfo = null;
            Type impContext = null;
            InstantiationTiming timing = InstantiationTiming.Lazy;
            // This is combining the search with the check.

            while (true)
            {
                var info = ti.GetProperty(property);
                if (info != null)
                {
                    Type pt = info.PropertyType;
                    Type timp = null;
                    InstantiationTiming ttiming = InstantiationTiming.Lazy;
                    var attrs2 = info.GetCustomAttributes(typeof(ImplementationAttribute), true);
                    if (attrs2.Length > 0)
                    {
                        Type current = null;
                        foreach (ImplementationAttribute attr2 in attrs2)
                        {
                            if (attr2.Applies)
                            {
                                if (timp != null)
                                {
                                    if (timp.IsAssignableFrom(attr2.Type))
                                    {
                                        timp = attr2.Type;
                                    }
                                    else if (!attr2.Type.IsAssignableFrom(timp))
                                    {
                                        throw new ImplementationsMismatchException(contextType, timp, ti, attr2.Type, ti);
                                    }
                                    if (ttiming != InstantiationTiming.Eager)
                                        ttiming = attr2.Timing;
                                }
                                else
                                {
                                    timp = attr2.Type;
                                    ttiming = attr2.Timing;
                                }
                            }

                        }
                        //var attr2 = (ImplementationAttribute)attrs2[0];
                    }
                    else if (!info.PropertyType.IsInterface)
                    {
                        timp = pt;
                    }
                    if (timp != null)
                    {
                        if (imp == null)
                        {
                            imp = timp;
                            impContext = ti;
                            impPropertyInfo = info;
                            timing = ttiming;
                        }
                        else
                        {
                            if (!timp.IsAssignableFrom(imp))
                                throw new ImplementationsMismatchException(contextType, imp, impContext, timp, ti);
                        }
                        if ( info.PropertyType.IsInterface &&  !pt.IsAssignableFrom(imp))
                        {
                            throw new ImplementationInterfaceMismatchException(contextType, imp, pt, ti);
                        }
                    }
                }
                if (i >= ifaces.Length)
                    break;
                ti = ifaces[i];
                ++i;
            }
            return new ConstructionNode(impPropertyInfo,impContext,imp,timing);
        }

        class State
        {
            public readonly Stack<String> ConstructorStack = new Stack<String>();
            public readonly Queue<ConstructionNode> ConstructedObjectsWaitingPropertyInjection = new Queue<ConstructionNode>();
            public Queue<ConstructionNode> ConstructedObjectsWaitingBuildFinalized = new Queue<ConstructionNode>();
        }

        class ConstructionNode
        {
            public ConstructionNode(PropertyInfo implementationProperty, Type implementationContext, Type imp, InstantiationTiming timing)
            {
                ImplementationProperty = implementationProperty;
                ImplementationContext = implementationContext;
                ImplementationType = imp;
                InstantiationTiming = timing;
            }


            public object Built;
            public readonly PropertyInfo ImplementationProperty; // may be just a property with a concrete type (which is bad, ok)
            public readonly Type ImplementationType;
            public readonly Type ImplementationContext; // where we found the implementation.
            public readonly InstantiationTiming InstantiationTiming;

            // Used for manual injection
            public ConstructionNode(Type contextType, object injectee)
            {
                Built = injectee;
                ImplementationType = injectee.GetType();
                ImplementationContext = contextType;
                InstantiationTiming = InstantiationTiming.Lazy;
            }
        }

        private State currentState = new State();

        public object ObjectForProperty(string propertyName)
        {
//            if (currentState == null)
//            {
//                currentState = new State();
//            }
            object rv = InternalObjectForProperty(propertyName);
            // Now we must go thru all the created objects, and initialize their properties.
            CompleteBuild();


            return rv;
        }

        public void Inject(object injectee)
        {
//            if (currentState == null)
//            {
//                currentState = new State();
//            }
            currentState.ConstructedObjectsWaitingPropertyInjection.Enqueue(new ConstructionNode(contextType, injectee));
            // Now we must go thru all the created objects, and initialize their properties.);
            CompleteBuild();
        }
        private void CompleteBuild()
        {
            while (currentState.ConstructedObjectsWaitingPropertyInjection.Count > 0)
            {
                ConstructionNode cn = currentState.ConstructedObjectsWaitingPropertyInjection.Dequeue();
                ResolvePropertyDependencies(cn);
                ResolveMethodDependencies(cn);
            }

//            Queue<ConstructionNode> constructedObjectsWaitingBuildFinalized = currentState.ConstructedObjectsWaitingBuildFinalized;
//            currentState = null;
            // This causes anything built as part of the finalization to be in a new build group.
            while (currentState.ConstructedObjectsWaitingBuildFinalized.Count > 0)
            {
                ConstructionNode cn = currentState.ConstructedObjectsWaitingBuildFinalized.Dequeue();
                CallCompletionMethods(cn);
            }
        }

        private void CallCompletionMethods(ConstructionNode cn)
        {
            Type t = cn.ImplementationType;
            var methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var methodInfo in methods)
            {
                var parameters = methodInfo.GetParameters();

                if (parameters.Length == 0)
                {
                    object[] customAttributes = Attribute.GetCustomAttributes(methodInfo, typeof(InjectionCompleteHandlerAttribute), true);
                    if (customAttributes.Length > 0)
                    {
                        methodInfo.Invoke(cn.Built, new object[0]);
                    }
                }
            }

        }


        private void ResolveMethodDependencies(ConstructionNode cn)
        {
            Type t = cn.ImplementationType;
            var methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            bool foundBuildComplete = false;
            foreach (var methodInfo in methods)
            {
                var parameters = methodInfo.GetParameters();

                if (parameters.Length>0)
                {
                    object[] customAttributes = Attribute.GetCustomAttributes(methodInfo, typeof(InjectAttribute), true);
                    if (customAttributes.Length > 0)
                    {
                        object[] args = new object[parameters.Length];
                        for (int i = 0; i < args.Length; ++i)
                        {
                            args[i] = ResolveObjectForType(parameters[i].ParameterType, cn);
                        }
                        methodInfo.Invoke(cn.Built, args);
                    }
                }
                else if (!foundBuildComplete)
                {
                    object[] customAttributes = Attribute.GetCustomAttributes(methodInfo, typeof(InjectionCompleteHandlerAttribute), true);
                    if (customAttributes.Length > 0)
                    {
                        foundBuildComplete = true;
                        currentState.ConstructedObjectsWaitingBuildFinalized.Enqueue(cn);
                    }
                    
                }
            }
        }

        private void ResolvePropertyDependencies(ConstructionNode cn)
        {
            Type t = cn.ImplementationType;
            //var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in props)
            {
                /* 
                 * http://msdn.microsoft.com/en-US/library/system.reflection.propertyinfo(v=VS.80).aspx
                 * "Calling ICustomAttributeProvider.GetCustomAttributes on PropertyInfo when the inherit parameter
                 * of GetCustomAttributes is true does not walk the type hierarchy. Use System.Attribute to inherit
                 * custom attributes."
                 * 
                 * So this doesnt work:
                 * 
                 * object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(InjectAttribute), true);
                 */

                object[] customAttributes = Attribute.GetCustomAttributes(propertyInfo,typeof(InjectAttribute), true);
                if ( customAttributes.Length>0)
                {
                    object o = ResolveObjectForType(propertyInfo.PropertyType, cn);
                    propertyInfo.SetValue(cn.Built, o, null);
                }
            }
        }

        private object InternalObjectForProperty(string propertyName)
        {
            if (currentState.ConstructorStack.Contains(propertyName))
            {
                throw new CyclicDependencyException(this.contextType);
            }

            object rv;
            if (singletons.TryGetValue(propertyName, out rv))
            {
                return rv;
            }
            ConstructionNode node = ImplementationTypeForProperty(propertyName);
            if (node == null)
            {
                throw new MissingDefinitionException(propertyName, this.contextType);
            }
            if (singletonsByType.TryGetValue(node.ImplementationType, out rv))
            {
                return rv;
            }
            try
            {
                currentState.ConstructorStack.Push(propertyName);
                // find constructor
                ConstructorInfo ctor = GetCtor(node.ImplementationType);
                var parameters = ctor.GetParameters();
                object[] args = new object[parameters.Length];
                for (int i = 0; i < args.Length; ++i)
                {
                    args[i] = ResolveObjectForType(parameters[i].ParameterType, node);
                }

                rv = Activator.CreateInstance(node.ImplementationType, args);
                singletons[propertyName] = rv;
                singletonsByType[node.ImplementationType] = rv;
                node.Built = rv;
                currentState.ConstructedObjectsWaitingPropertyInjection.Enqueue(node);
                return rv;
            }
            finally
            {
                currentState.ConstructorStack.Pop();
            }
        }

        private object ResolveObjectForType(Type type, ConstructionNode node)
        {
            if (type.IsAssignableFrom(contextType))
            {
                return externalContext;
            }
            PropertyInfo pi = PropertyForType(type, node);
            return InternalObjectForProperty(pi.Name);
        }

        private PropertyInfo PropertyForType(Type parameterType, ConstructionNode node)
        {
            PropertyInfo pi = PropertyForType(parameterType, node.ImplementationContext);
            if (pi==null)
                throw new PropertyDependencyResolutionException(contextType, node.ImplementationProperty, parameterType);
            return pi;

        }

        private PropertyInfo PropertyForType(Type parameterType, Type startingContext)
        {
            var ti = startingContext;
            Type[] ifaces = startingContext.GetInterfaces();
            int i = 0;
            while (true)
            {

                var props =
                    ti.GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy |
                                                  BindingFlags.Instance);
                foreach (var propertyInfo in props)
                {
                    if (propertyInfo.PropertyType == parameterType ||
                        parameterType.IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        return propertyInfo;
                    }
                    ConstructionNode node = ImplementationTypeForProperty(propertyInfo.Name);
                    if (node.ImplementationType == parameterType || parameterType.IsAssignableFrom(node.ImplementationType))
                    {
                        return propertyInfo;
                    }
                }
                if (i >= ifaces.Length)
                    break;

                ti = ifaces[i];
                ++i;
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

        internal TContext Create<TContext>()
            where TContext : class, IContext
        {
            if (externalContext == null)
            {
                object result = Activator.CreateInstance(custom);
                BaseContextImpl impl = (BaseContextImpl) result;
                impl.SetFactory(this);
                // now have to check for eager implementations
                CreateEagerImplementations(custom, impl);
                externalContext = (TContext) result;
            }
            return (TContext) externalContext;
        }

        internal IContext CreateBasic<TContext>()
            where TContext : class, IContext
        {
            if (externalContext == null)
            {
                object result = Activator.CreateInstance(custom);
                BaseContextImpl impl = (BaseContextImpl)result;
                impl.SetFactory(this);
                // now have to check for eager implementations
                CreateEagerImplementations(typeof(TContext), impl);
                externalContext = (IContext)result;
            }
            return externalContext;
        }

        private void CreateEagerImplementations(Type type, BaseContextImpl impl)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var info in props)
            {
                var node = ImplementationTypeForPropertyBuilder(info.Name);
                nodeMap.Add(info.Name,node);
            }

            // Now build all the nodes.
            foreach (var constructionNode in nodeMap)
            {
                if (constructionNode.Value.InstantiationTiming == InstantiationTiming.Eager)
                {
                    InternalObjectForProperty(constructionNode.Value.ImplementationProperty.Name);
                }
            }
            CompleteBuild();
        }
    }
}