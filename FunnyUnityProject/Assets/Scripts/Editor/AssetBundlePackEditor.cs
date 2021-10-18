using System.IO;
using UnityEditor;
using UnityEngine;

namespace GFrame
{
    public class AssetBundlePackEditor
    {
        [MenuItem("Tools/AssetBundle/Clear AssetBundle Names")]
        public static void ClearAssetBundleName()
        {
            ClearAllAssetBundleNames();
        }

        [MenuItem("Tools/AssetBundle/BuildStandaloneWindows64")]
        public static void BuildWindowBundle()
        {
            BuildBundle(BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Tools/AssetBundle/BuildAndroid")]
        public static void BuildAndroidBundle()
        {
            BuildBundle(BuildTarget.Android);
        }

        [MenuItem("Tools/AssetBundle/BuildIOS")]
        public static void BuildIOSBundle()
        {
            BuildBundle(BuildTarget.iOS);
        }


        private static void ClearAllAssetBundleNames()
        {
            var names = AssetDatabase.GetAllAssetBundleNames();
            foreach (var name in names)
                AssetDatabase.RemoveAssetBundleName(name, true);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            var abNames = AssetDatabase.GetAllAssetBundleNames();
            UnityEngine.Debug.Log("ab name clean finished count:" + abNames.Length);
        }

        private static void BuildBundle(BuildTarget target)
        {
            var abPath = PathUtil.GetAssetBundlePath(target);
            if (Directory.Exists(abPath))
                Directory.Delete(abPath, true);
            Directory.CreateDirectory(abPath);

            var option = BuildAssetBundleOptions.ChunkBasedCompression |
                         BuildAssetBundleOptions.IgnoreTypeTreeChanges;
            BuildPipeline.BuildAssetBundles(abPath, option, target);
            Debug.Log($"build AssetBundle Done: {target}   path:{abPath} ");
        }
    }
}