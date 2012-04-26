using System;
using System.Collections.Generic;
using System.Reflection;
using BinaryFinery.IOC.Attributes;

namespace BinaryFinery.IOC.Runtime.Builder
{
    internal class DefaultInspector
    {
        public BuildNode Inspect(Type t)
        {
            var bn = new BuildNode(t);
            AddInjectors(t, bn);
            bn.Constructor = ChooseConstructor(t, bn);
            AddHandlers(t, bn);
            return bn;
        }

        private void AddHandlers(Type type, BuildNode bn)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            var injectionCompleteHandlers = new List<MethodInfo>();
            foreach (var methodInfo in methods)
            {
                if (methodInfo.GetParameters().Length==0)
                {
                    var attrs = methodInfo.GetCustomAttributes(typeof(InjectionCompleteHandlerAttribute), true);
                    if(attrs.Length>0)
                    {
                        injectionCompleteHandlers.Add(methodInfo);
                    }
                }
            }
            bn.Handlers = injectionCompleteHandlers;
        }

        private ConstructorInfo ChooseConstructor(Type type, BuildNode bn)
        {
            var ctors= type.GetConstructors(BindingFlags.Public|BindingFlags.Instance);
            ConstructorInfo dfault = null;
            foreach (var constructorInfo in ctors)
            {
                var parameterInfos = constructorInfo.GetParameters();
                if (parameterInfos.Length == 0)
                {
                    dfault = constructorInfo;
                }
                var attrs = constructorInfo.GetCustomAttributes(typeof(InjectAttribute), true);
                if (attrs.Length>0)
                {
                    return constructorInfo;
                }
            }
            if (dfault != null)
                return dfault;
            if (ctors.Length > 0)
                return ctors[0];
            return null;
        }

        private void AddInjectors(Type type, BuildNode bn)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<PropertyInfo> pi = new List<PropertyInfo>();
            foreach (var propertyInfo in props)
            {
                var attrs = propertyInfo.GetCustomAttributes(typeof(InjectAttribute), true);
                if (attrs.Length>0)
                {
                    pi.Add(propertyInfo);
                }
            }
            bn.Injections = pi;
        }
    }
}