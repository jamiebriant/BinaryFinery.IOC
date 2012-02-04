using System;
using System.Collections.Generic;

namespace BinaryFinery.IOC.Runtime.Builder
{
    public class Context : IContext
    {
        DefaultInspector inspector = new DefaultInspector();

        public Context()
        {
            RegisterSingleton<IContext>(this);
            RegisterSingleton<IInjector>(this);
        }

        public T Get<T>()
            where T : class
        {
            var type = typeof(T);
            return (T) Get(type);
        }

        public object Get(Type type)
        {
            var bt = new BuildTransaction(this, inspector);
            return bt.Get(type);
        }

        public void RegisterSingleton<TBuilt, TRequested>()
        {
            var from = new TypeKey(typeof(TRequested));
            var to = new TypeKey(typeof(TBuilt));
            var node = new Node(to);
            rules[from] = node;
            rules[to] = node;
        }

        public void RegisterSingleton<TBuilt>()
        {
            var to = new TypeKey(typeof(TBuilt));
            var node = new Node(to);
            rules[to] = node;
        }

        public void RegisterSingleton<T>(T context)
        {
            var node = new Node(context);
            if (context.GetType() != typeof(T))
            {
                rules[new TypeKey(typeof(T))] = node;
            }
            rules[node.TypeKey] = node;
        }

        internal Dictionary<TypeKey, Node> rules = new Dictionary<TypeKey, Node>();

        internal Node ResolveType(TypeKey tk)
        {
            Node n;
            if (!rules.TryGetValue(tk, out n))
            {
                throw new TypeNotFoundException(this, tk.Type);
            }
            return n;
        }

        public T Inject<T>(T what)
        {
            var bt = new BuildTransaction(this, inspector);
            bt.Inject(what);
            return what;
        }
    }
}