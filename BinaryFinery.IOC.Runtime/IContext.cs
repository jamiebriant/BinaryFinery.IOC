using System.Text;

namespace BinaryFinery.IOC.Runtime
{
    public interface IContext : IInjector
    {
        T Get<T>() where T : class;
        void RegisterSingleton<TBuilt, TRequested>();
        void RegisterSingleton<TBuilt>();
    }

    public interface IInjector
    {
        T Inject<T>(T what);
    }
}
