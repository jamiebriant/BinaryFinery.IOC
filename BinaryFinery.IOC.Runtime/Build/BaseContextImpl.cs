namespace BinaryFinery.IOC.Runtime.Build
{
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
}