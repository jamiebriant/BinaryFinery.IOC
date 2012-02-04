using System;
using System.Collections.Generic;
using System.Reflection;

namespace BinaryFinery.IOC.Runtime.Builder
{
    internal class BuildTransaction
    {
        private readonly Context context;
        private readonly DefaultInspector inspector;

        public BuildTransaction(Context context, DefaultInspector inspector)
        {
            this.context = context;
            this.inspector = inspector;
        }

        class ResultNode
        {
            public bool IsBuilt
            {
                get { return Node.Value != null; }
            }

            public object Value
            {
                get { return Node.Value; }
            }

            public BuildNode BuildNode { get; set; }

            public Node Node
            {
                get { return node; }
            }

            private Node node;
            private readonly TypeKey tk;
            public ResultNode[] CtorArgs;

            public ResultNode(TypeKey tk, Node node)
            {
                this.node = node;
            }


        }

        Dictionary<TypeKey,ResultNode> nodes = new Dictionary<TypeKey,ResultNode>();
        Dictionary<TypeKey,ResultNode> resolvedNodes = new Dictionary<TypeKey,ResultNode>(); 
        Queue<Node> consider = new Queue<Node>();
        Queue<ResultNode> readyToConstruct = new Queue<ResultNode>();
        Queue<ResultNode> readyToFill = new Queue<ResultNode>();
        Queue<ResultNode> doneHandlers = new Queue<ResultNode>(); 

        public object Get(Type type)
        {
            var typeKey = new TypeKey(type);
            Node n = context.ResolveType(typeKey);
            consider.Enqueue(n);
            Build();
            return n.Value;

        }

        public void Inject(object what)
        {
            var tk = new TypeKey(what.GetType());
            Node n = new Node(tk);
            n.Value = what;
            ResultNode rn = new ResultNode(tk,n);
            var bn = inspector.Inspect(n.Build);
            rn.BuildNode = bn;
            foreach (var op in bn.Injections)
            {
                var cn = context.ResolveType(new TypeKey(op.PropertyType));
                consider.Enqueue(cn);
            }
            readyToFill.Enqueue(rn);
            Build();
        }

        private void Build()
        {
            while(consider.Count>0)
            {
                var node = consider.Dequeue();
                var tried = new Stack<TypeKey>();
                CreateResultNode(tried,node);
            }

            while(readyToConstruct.Count>0)
            {
                var rn = readyToConstruct.Dequeue();
                Construct(rn);
            }
            while(readyToFill.Count>0)
            {
                var rn = readyToFill.Dequeue();
                Fill(rn);
            }
            while(doneHandlers.Count>0)
            {
                var rn = doneHandlers.Dequeue();
                CallInjectionCompleteHandler(rn);
            }

        }

        private void CallInjectionCompleteHandler(ResultNode rn)
        {
            foreach (var methodInfo in rn.BuildNode.Handlers)
            {
                methodInfo.Invoke(rn.Value, null);
            }
        }

        private void Fill(ResultNode rn)
        {
            var bn = rn.BuildNode;
            foreach (var op in bn.Injections)
            {
                var cn = context.ResolveType(new TypeKey(op.PropertyType));
                op.SetValue(rn.Value,cn.Value,null);
            }
            if (rn.BuildNode.HasHandlers)
            {
                doneHandlers.Enqueue(rn);
            }
        }

        private void Construct(ResultNode rn)
        {
            var bn = rn.BuildNode;
            if (bn == null)
            {
                throw new InjectionException("Internal failure");
            }
            ConstructorInfo ctor = bn.Constructor;
            var argts = ctor.GetParameters();
            var args = new object[argts.Length];
            for (int i = 0; i < argts.Length; ++i )
            {
                args[i] = rn.CtorArgs[i].Value;
            }
            rn.Node.Value = Activator.CreateInstance(rn.Node.TypeKey.Type, args);
            resolvedNodes[rn.Node.TypeKey] = rn;
            readyToFill.Enqueue(rn);
        }

        private ResultNode CreateResultNode(Stack<TypeKey> tried, Node n)
        {
            var tk = n.TypeKey;
            if (tried.Contains(tk))
            {
                throw new CircularConstructorDependencyException(tried);
            }
            tried.Push(tk);
            try
            {
                ResultNode rn;
                if (nodes.TryGetValue(n.TypeKey, out rn))
                {
                    return rn;
                }
                rn = new ResultNode(tk, n);
                nodes[tk] = rn;
                if (n.Value != null)
                {
                    resolvedNodes[tk] = rn;
                    return rn;
                }
                else
                {
                    var bn = inspector.Inspect(n.Build);
                    rn.BuildNode = bn;

                    ConstructorInfo ctor = bn.Constructor;
                    if (ctor == null)
                        throw new NoSuiteableInjectionConstructorException(bn.Type);
                    var argts = ctor.GetParameters();

                    if (argts.Length > 0)
                    {
                        var argns = new ResultNode[argts.Length];
                        int i = 0;
                        foreach (var parameterInfo in argts)
                        {
                            var pn = context.ResolveType(new TypeKey(parameterInfo.ParameterType));
                            var arn = CreateResultNode(tried, pn);
                            argns[i] = arn;
                            ++i;
                        }
                        rn.CtorArgs = argns;
                    }
                    readyToConstruct.Enqueue(rn);
                    nodes[tk] = rn;
                    foreach (var op in bn.Injections)
                    {
                        var cn = context.ResolveType(new TypeKey(op.PropertyType));
                        consider.Enqueue(cn);
                    }

                    return rn;
                }
            }
            finally
            {
                tried.Pop();
            }
        }

    }
}