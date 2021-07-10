using System;
using Object = UnityEngine.Object;

namespace GFrame.Service
{
    public interface ILoader
    {
        T Load<T>(string path) where T : Object;

        void LoadAsync<T>(string path, Action<T> callBack) where T : Object;

        T Instantiate<T>(string path) where T : Object;

        void InstantiateAsync<T>(string path, Action<T> callBack) where T : Object;
    }
}