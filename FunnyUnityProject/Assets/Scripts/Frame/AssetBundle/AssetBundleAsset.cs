using UnityEngine;

namespace FFrame
{
    public class AssetBundleAsset : Asset
    {
        private AssetBundle _assetBundle;
        public AssetBundle Asset => _assetBundle;

        public AssetBundle Load(string abPath)
        {
            _assetBundle = AssetBundle.LoadFromFile(abPath);
            OnLoad();
            return _assetBundle;
        }

        public AssetBundleCreateRequest LoadAsync(string abPath)
        {
            var req = AssetBundle.LoadFromFileAsync(abPath);
            req.completed += op => OnLoad();
            return req;
        }


        public void UnLoad(bool forceRelease)
        {
            UnLoad();
            if (IsUnUsed()) _assetBundle?.Unload(forceRelease);
        }

        public AsyncOperation UnLoadAsync(bool forceRelease)
        {
            UnLoad();
            if (!IsUnUsed()) return null;
            var op = _assetBundle?.UnloadAsync(forceRelease);
            return op;
        }
    }
}