using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

        public static string GetAssetBundlePath(RuntimePlatform platform)
        {
            var abPath = GetAssetBundlePath();
            string platformPath = GetPlatformPath(platform);
            abPath = Path.Combine(abPath, platformPath);
            return abPath;
        }

        public static string GetPlatformPath(RuntimePlatform platform)
        {
            string platformPath = string.Empty;
            switch (platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    platformPath = "Windows";
                    break;
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.OSXEditor:
                    platformPath = "IOS";
                    break;
                case RuntimePlatform.Android:
                    platformPath = "Android";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
            }

            return platformPath;
        }

#if UNITY_EDITOR
        public static string GetAssetBundlePath(BuildTarget buildTarget)
        {
            var abPath = GetAssetBundlePath();
            string platformPath = string.Empty;
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    platformPath = "Windows";
                    break;
                case BuildTarget.iOS:
                    platformPath = "IOS";
                    break;
                case BuildTarget.Android:
                    platformPath = "Android";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildTarget), buildTarget, null);
            }

            abPath = abPath + "/" + platformPath;
            return abPath;
        }
#endif
    }
}