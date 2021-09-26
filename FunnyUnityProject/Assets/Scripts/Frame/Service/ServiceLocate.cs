using System;
using System.Collections.Generic;

namespace GFrame.Service
{
    public interface IServiceLocate : ILocate
    {
        T GetService<T>() where T : IService;

        bool TryGetService<T>(out T service) where T : IService;
    }

    public class ServiceLocate : IServiceLocate
    {
        private readonly Dictionary<Type, IService> _serviceDic;

        private static ServiceLocate _locate;

        public static ServiceLocate Get()
        {
            return _locate ??= new ServiceLocate();
        }

        public static void Release()
        {
            _locate = null;
        }

        private ServiceLocate()
        {
            _serviceDic = new Dictionary<Type, IService>();
        }

        void ILocate.OnUpdate()
        {
            foreach (var kv in _serviceDic)
                kv.Value.Update();
        }

        public void ResetAllService()
        {
            foreach (var kv in _serviceDic)
                kv.Value.Reset();
        }

        public void Destroy()
        {
            foreach (var kv in _serviceDic)
                kv.Value.Destroy();
            _serviceDic.Clear();
        }

        public void RegisterService<T>() where T : IService
        {
            IService service = Activator.CreateInstance<T>();
            InternalRegisterService(service);
        }

        public void RegisterService(IService service)
        {
            InternalRegisterService(service);
        }


        public void UnRegisterService<T>() where T : IService
        {
            InternalUnRegisterService<T>();
        }

        public T GetService<T>() where T : IService
        {
            var key = typeof(T);
            _serviceDic.TryGetValue(key, out var service);
            return (T) service;
        }

        public bool TryGetService<T>(out T service) where T : IService
        {
            return InternalTryGetService<T>(out service);
        }


        public void ResetService<T>() where T : IService
        {
            if (!InternalTryGetService<T>(out var service)) return;
            service.Reset();
        }

        private void InternalRegisterService(IService service)
        {
            var key = service.GetType();
            if (_serviceDic.ContainsKey(key))
            {
                GameLog.LogError($"[RegisterService] {key} has register.");
                return;
            }

            service.Create();
            _serviceDic.Add(key, service);
        }

        private void InternalUnRegisterService<T>() where T : IService
        {
            var key = typeof(T);
            if (!_serviceDic.TryGetValue(key, out var service))
            {
                GameLog.LogError($"[UnRegisterService] but {key} has not register.");
                return;
            }

            service.Destroy();
            _serviceDic.Remove(key);
        }

        private bool InternalTryGetService<T>(out T service) where T : IService
        {
            var key = typeof(T);
            bool success = _serviceDic.TryGetValue(key, out var temp);
            service = (T) temp;
            return success;
        }
    }
}