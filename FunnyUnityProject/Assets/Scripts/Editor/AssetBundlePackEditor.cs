using System.IO;
using UnityEditor;
using UnityEngine;

namespace GFrame
{
    public class AssetBundlePackEditor
    {
        [MenuItem("Tools/AssetBundle/Build")]
        public static void BuildBundle()
        {
            var abPath = PathUtil.GetAssetBundlePath();
            if (!Directory.Exists(abPath))
                Directory.CreateDirectory(abPath);
            else
                Directory.Delete(abPath, true);

            var option = BuildAssetBundleOptions.ChunkBasedCompression |
                         BuildAssetBundleOptions.IgnoreTypeTreeChanges;
            var manifest = BuildPipeline.BuildAssetBundles(abPath, option, BuildTarget.StandaloneWindows64);
            Debug.Log("build done:  " + manifest);
        }
    }
}