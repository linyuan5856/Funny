using System;
using System.Collections.Generic;
using GFrame.Service;

namespace GFrame.System
{
    public class SystemFactory
    {
        private static SystemFactory _factory;

        public static SystemFactory Get(IServiceLocate locate)
        {
            return _factory ??= new SystemFactory(locate);
        }

        public static void Release()
        {
            _factory = null;
        }

        private readonly IServiceLocate _locate;
        private readonly Dictionary<Type, ISystem> _systemDic;

        private SystemFactory(IServiceLocate locate)
        {
            _locate = locate;
            _systemDic = new Dictionary<Type, ISystem>();
        }

        public void Update()
        {
            foreach (var map in _systemDic)
                map.Value.Update();
        }

        public void CreateSystem<T>() where T : ISystem
        {
            Type key = typeof(T);
            if (_systemDic.ContainsKey(key))
                return;
            T t = Activator.CreateInstance<T>();
            t.Create(_locate);
            _systemDic.Add(key, t);
        }

        public T GetSystem<T>() where T : ISystem
        {
            Type key = typeof(T);
            _systemDic.TryGetValue(key, out var system);
            return (T) system;
        }
    }
}