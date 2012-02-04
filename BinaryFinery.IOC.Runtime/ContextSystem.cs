using BinaryFinery.IOC.Runtime.Builder;

namespace BinaryFinery.IOC.Runtime
{
    public static class ContextSystem
    {
        public static Context CreateContextForTesting()
        {
            return new Context();
        }

        public static Context RootContext
        {
            get { return rootContext; }
        }

        private static readonly Context rootContext = new Context();
    }
}
