using Unity;

namespace ETW.Provider
{
    public interface IEventProvider
    {
        void Subscribe(IUnityContainer container);
        void Unsubscribe(IUnityContainer container);
    }
}