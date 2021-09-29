using UnityEngine;

namespace GFrame
{
    public class PathUtil
    {
        public static string GetAssetBundlePath()
        {
            int index = Application.dataPath.LastIndexOf('/');
            var newPath = Application.dataPath.Substring(0, index);
            var finalPath = newPath + "/AssetBundle";
            return finalPath;
        }
    }
}