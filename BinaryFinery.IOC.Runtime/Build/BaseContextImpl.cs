namespace BinaryFinery.IOC.Runtime.Build
{
    public class BaseContextImpl : IContext, IInjector
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

        public void Inject(object injectee)
        {
        }
    }
}