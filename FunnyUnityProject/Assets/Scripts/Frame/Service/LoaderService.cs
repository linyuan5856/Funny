using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace GFrame.Service
{
    public enum ELoadType
    {
        AssetDataBase,
        Resource,
        Addressable,
    }

    public class LoaderService : BaseService
    {
        private Dictionary<ELoadType, ILoader> _loaderDic;
        private SceneLoader _sceneLoader;

        protected override void OnCreate()
        {
            base.OnCreate();
            _sceneLoader = SceneLoader.Get();
            _loaderDic = new Dictionary<ELoadType, ILoader>
            {
                {ELoadType.Resource, new ResourceLoader()},
                {ELoadType.AssetDataBase, new AssetDataBaseLoader()}
            };
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneLoader.DestroyLoader();
        }


        private ILoader GetLoader()
        {
            return _loaderDic[ELoadType.AssetDataBase];
        }

        public T Load<T>(string path) where T : Object
        {
            return GetLoader().Load<T>(path);
        }

        public T Instantiate<T>(string path) where T : Object
        {
            return GetLoader().Instantiate<T>(path);
        }

        public void InstantiateAsync<T>(string path, Action<T> callBack) where T : Object
        {
            GetLoader().InstantiateAsync(path, callBack);
        }

        public void LoadAsync<T>(string path, Action<T> callBack) where T : Object
        {
            GetLoader().LoadAsync<T>(path, callBack);
        }

        public void LoadScene(string sceneName)
        {
            _sceneLoader.LoadScene(sceneName);
        }

        public void LoadSceneAsync(string sceneName, Action onLoadDone, LoadSceneMode mode = LoadSceneMode.Single)
        {
            _sceneLoader.LoadSceneAsync(sceneName, onLoadDone, mode);
        }
    }


    public class ResourceLoader : ILoader
    {
        public T Load<T>(string path) where T : Object
        {
            return InternalLoad<T>(path);
        }

        public void LoadAsync<T>(string path, Action<T> callBack) where T : Object
        {
            GameLog.Assert(callBack != null, "load async ,but callBack is null");

            var async = Resources.LoadAsync<T>(path);
            async.completed += result =>
            {
                if (result is ResourceRequest req)
                    callBack?.Invoke(req.asset as T);
            };
        }

        public T Instantiate<T>(string path) where T : Object
        {
            T asset = InternalLoad<T>(path);
            GameLog.Assert(asset != null, $"Instantiate {path} Failed");
            T ins = Object.Instantiate<T>(asset);
            return ins;
        }

        public void InstantiateAsync<T>(string path, Action<T> callBack) where T : Object
        {
            LoadAsync<T>(path, asset =>
            {
                GameLog.Assert(asset != null, $"Instantiate {path} Failed");
                var ins = Object.Instantiate(asset);
                callBack?.Invoke(ins);
            });
        }


        private T InternalLoad<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }
    }

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