using FGame;
using GFrame.Service;

namespace GFrame.System
{
    public class BaseSystem : ISystem
    {
        private IServiceLocate _serviceLocate;

        void ISystem.Create(IServiceLocate locate)
        {
            _serviceLocate = locate;
            OnCreate();
        }

        public void Update()
        {
            OnUpdate();
        }

        protected virtual void OnCreate()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        T GetService<T>() where T : IService
        {
            return _serviceLocate.GetService<T>();
        }

        bool TryGetService<T>(out T service) where T : IService
        {
            return _serviceLocate.TryGetService<T>(out service);
        }
    }
}