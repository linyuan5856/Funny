using GFrame.Service;

namespace GFrame.System
{
    public class BaseSystem : ISystem
    {
        private IServiceLocate _serviceLocate;
        private ISystemLocate _systemLocate;

        void ISystem.Create(IGameLocate gameLocate)
        {
            _serviceLocate = gameLocate.GetLocate(GameDefine.SERVICE_LOCATE) as ServiceLocate;
            _systemLocate = gameLocate.GetLocate(GameDefine.SYSTEM_LOCATE) as SystemFactory;
            OnCreate();
        }

        public void Create()
        {
        }

        public void Update()
        {
            OnUpdate();
        }

        public void Reset()
        {
        }

        public void Destroy()
        {
        }

        protected virtual void OnCreate()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected T GetService<T>() where T : IService
        {
            return _serviceLocate.GetService<T>();
        }

        protected bool TryGetService<T>(out T service) where T : IService
        {
            return _serviceLocate.TryGetService<T>(out service);
        }

        protected T GetSystem<T>() where T : ISystem
        {
            return _systemLocate.GetSystem<T>();
        }
    }
}