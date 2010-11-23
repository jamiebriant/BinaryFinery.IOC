namespace BinaryFinery.IOC.Runtime.Build
{
    public class BaseContextImpl : IContext
    {
        private ContextFactory factory;

        internal void SetFactory(ContextFactory factory)
        {
            this.factory = factory;
        }

        public void Inject(object injectee)
        {
            factory.Inject(injectee);
        }

        protected object ObjectForProperty(string propertyName)
        {
            return factory.ObjectForProperty(propertyName);
        }
    }
}