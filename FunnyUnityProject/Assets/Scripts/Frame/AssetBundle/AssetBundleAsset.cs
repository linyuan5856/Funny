using UnityEngine;

namespace FFrame
{
    public class AssetBundleAsset
    {
        private AssetBundle _assetBundle;
        public AssetBundle Asset => _assetBundle;

        public AssetBundle Load(string abPath)
        {
            _assetBundle = AssetBundle.LoadFromFile(abPath);
            return _assetBundle;
        }

        public AssetBundleCreateRequest LoadAsync(string abPath)
        {
            return AssetBundle.LoadFromFileAsync(abPath);
        }

        public void UnLoad(bool forceRelease)
        {
            _assetBundle?.Unload(forceRelease);
        }

        public AsyncOperation UnLoadAsync(bool forceRelease)
        {
            return _assetBundle?.UnloadAsync(forceRelease);
        }
    }
}