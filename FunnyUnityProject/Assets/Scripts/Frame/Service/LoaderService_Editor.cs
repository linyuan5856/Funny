#if UNITY_EDITOR
using UnityEditor;
using System;
using Object = UnityEngine.Object;

namespace GFrame.Service
{
    public partial class LoaderService : BaseService
    {
        public class AssetDataBaseLoader : ILoader
        {
            private const string Prefix = "Assets/";

            public T Load<T>(string path) where T : Object
            {
                return AssetDatabase.LoadAssetAtPath<T>(Prefix + path);
            }

            public void LoadAsync<T>(string path, Action<T> callBack) where T : Object
            {
                throw new NotImplementedException();
            }

            public T Instantiate<T>(string path) where T : Object
            {
                T asset = Load<T>(path);
                return Object.Instantiate(asset);
            }

            public void InstantiateAsync<T>(string path, Action<T> callBack) where T : Object
            {
                throw new NotImplementedException();
            }
        }
    }
}

#endif