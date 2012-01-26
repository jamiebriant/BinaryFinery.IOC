namespace BinaryFinery.IOC.Runtime.Build
{
    public abstract class BaseContextImpl : IContext
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

        public T Get<T>(string propertyName)
        {
            return (T)factory.ObjectForProperty(propertyName);
        }
    }

    public class BasicContextImpl : BaseContextImpl
    {
        
    }
}