using System;
using System.Collections.Generic;
using System.IO;
using FFrame;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;


namespace GFrame.Service
{
    public enum ELoadType
    {
        AssetDataBase,
        Resource,
        AssetBundle,
    }

    public partial class LoaderService : BaseService
    {
        private Dictionary<ELoadType, ILoader> _loaderDic;
        private SceneLoader _sceneLoader;
        private ELoadType _currentLoad = ELoadType.Resource;

        protected override void OnCreate()
        {
            base.OnCreate();
            _sceneLoader = SceneLoader.Get();
            _loaderDic = new Dictionary<ELoadType, ILoader>
            {
                { ELoadType.Resource, new ResourceLoader() },
                { ELoadType.AssetBundle, new AssetBundleLoader() },
#if UNITY_EDITOR
                { ELoadType.AssetDataBase, new AssetDataBaseLoader() }
#endif
            };
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneLoader.DestroyLoader();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            
        }

        public void SetLoader(ELoadType load)
        {
            _currentLoad = load;
        }

        public GameConfig GetGameConfig()
        {
            var old = _currentLoad;
            _currentLoad = ELoadType.Resource;
            var config = Load<GameConfig>("Config/GameConfig");
            _currentLoad = old;
            return config;
        }

        public T Load<T>(string path) where T : Object
        {
            return GetLoader().Load<T>(path);
        }

        public void LoadAsync<T>(string path, Action<T> callBack) where T : Object
        {
            GetLoader().LoadAsync<T>(path, callBack);
        }

        public T Instantiate<T>(string path) where T : Object
        {
            return GetLoader().Instantiate<T>(path);
        }

        public void InstantiateAsync<T>(string path, Action<T> callBack) where T : Object
        {
            GetLoader().InstantiateAsync(path, callBack);
        }

        public void LoadScene(string sceneName)
        {
            _sceneLoader.LoadScene(sceneName);
        }

        public void LoadSceneAsync(string sceneName, Action onLoadDone, LoadSceneMode mode = LoadSceneMode.Single)
        {
            _sceneLoader.LoadSceneAsync(sceneName, onLoadDone, mode);
        }

        public void Collect(bool unloadRes, bool gcCollect, bool unloadAssetBundle)
        {
            if (unloadRes)
                Resources.UnloadUnusedAssets();
            if (gcCollect)
                GC.Collect();
            if (unloadAssetBundle)
            {
                var old = _currentLoad;
                SetLoader(ELoadType.AssetBundle);
                if (GetLoader() is AssetBundleLoader loader)
                    loader.ReleaseAll();
                SetLoader(old);
                AssetBundle.UnloadAllAssetBundles(true);
            }
        }

        public void UnLoadAsset(Object res)
        {
            Resources.UnloadAsset(res);
        }

        public bool UnLoadAssetBundle(string name, bool unLoadLoaded)
        {
            var loader = ForceGetLoader<AssetBundleLoader>(ELoadType.AssetBundle);
            return loader.UnLoadAssetBundle(name, unLoadLoaded);
        }

        public AsyncOperation UnLoadAssetBundleAsync(string name, bool unLoadLoaded)
        {
            var loader = ForceGetLoader<AssetBundleLoader>(ELoadType.AssetBundle);
            return loader.UnLoadAssetBundleAsync(name, unLoadLoaded);
        }


        public static bool IsResourceExist(string url, bool raiseError = true)
        {
            var pathType = GetResourceFullPath(url);
            return !string.IsNullOrEmpty(pathType);
        }

        public static byte[] LoadAssetsSync(string path)
        {
            string fullPath = GetResourceFullPath(path);
            if (string.IsNullOrEmpty(fullPath))
                return null;

            // if (Application.platform == RuntimePlatform.Android)
            // {
            //     return KEngineAndroidPlugin.GetAssetBytes(path);
            //     //TODO 通过www/webrequest读取
            // }

            return ReadAllBytes(fullPath);
        }

        public static string GetResourceFullPath(string url)
        {
            string fullPath = "";
            return fullPath;
        }

        public static byte[] ReadAllBytes(string resPath)
        {
            byte[] bytes;
            using (FileStream fs = File.Open(resPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
            }

            return bytes;
        }

        private ILoader GetLoader()
        {
            var success = _loaderDic.TryGetValue(_currentLoad, out var loader);
            return success ? loader : null;
        }

        private T ForceGetLoader<T>(ELoadType loadType) where T : class, ILoader
        {
            SetLoader(loadType);
            return GetLoader() as T;
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


    public class AssetBundleLoader : ILoader
    {
        private AssetBundleManifest _manifest;
        private NameMappingTable _nameMappingTable;
        private Dictionary<string, AssetBundleAsset> _cache;

        public AssetBundleLoader()
        {
            Init();
        }

        public void ReleaseAll()
        {
            _cache.Clear();
            _nameMappingTable = null;
            _manifest = null;
        }

        private void Init()
        {
            _cache = new Dictionary<string, AssetBundleAsset>();

            string mainFestName = PathUtil.GetPlatformPath(Application.platform);
            var ab = LoadAb(mainFestName, false);
            var maniFest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            _manifest = maniFest;

            var tableAssetBundle = LoadAb("namemappingtable.0", false);
            _nameMappingTable = tableAssetBundle.LoadAsset<NameMappingTable>("NameMappingTable");
        }

        private AssetBundle LoadAb(string name, bool bNameMapping = true)
        {
            if (_cache.TryGetValue(name, out var asset))
                return asset.Asset;

            var path = PathUtil.GetAssetBundlePath(Application.platform);
            var abName = name;
            if (bNameMapping)
                abName = _nameMappingTable.GetNameMapping(name);

            var abPath = Path.Combine(path, abName);
            if (_manifest)
            {
                var dependencies = _manifest.GetAllDependencies(abName);
                foreach (var abDepend in dependencies)
                    LoadAb(abDepend, false);
            }

            AssetBundleAsset abAsset = new AssetBundleAsset();
            _cache.Add(name, abAsset);
            var ab = abAsset.Load(abPath);
            GameLog.LogWarn($"[Asset Bundle] Load : {abPath}");
            return ab;
        }

        public bool UnLoadAssetBundle(string name, bool unLoadLoaded)
        {
            if (!_cache.TryGetValue(name, out var asset)) return false;
            asset.UnLoad(unLoadLoaded);
            _cache.Remove(name);
            GameLog.Log($"[Asset Bundle] Unload: {name} IsForceUnload: {unLoadLoaded}");
            return true;
        }

        public AsyncOperation UnLoadAssetBundleAsync(string name, bool unLoadLoaded)
        {
            if (!_cache.TryGetValue(name, out var asset)) return null;
            AsyncOperation op = asset.UnLoadAsync(unLoadLoaded);
            return op;
        }

        public T Load<T>(string path) where T : Object
        {
            var ab = LoadAb(path);
            return ab.LoadAsset<T>(path);
        }

        public void LoadAsync<T>(string path, Action<T> callBack) where T : Object
        {
            if (callBack == null) return;

            var ab = LoadAb(path);
            var abReq = ab.LoadAssetAsync<T>(path);
            abReq.completed += op =>
            {
                if (op is AssetBundleRequest req)
                    callBack?.Invoke(req.asset as T);
            };
        }

        T ILoader.Instantiate<T>(string path)
        {
            T asset = Load<T>(path);
            return asset != null ? Object.Instantiate(asset) : null;
        }

        void ILoader.InstantiateAsync<T>(string path, Action<T> callBack)
        {
            LoadAsync<T>(path, asset =>
            {
                if (asset != null)
                    Object.Instantiate(asset);
            });
        }
    }
}