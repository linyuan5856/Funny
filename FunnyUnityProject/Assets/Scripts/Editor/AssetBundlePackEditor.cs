using System.IO;
using System.Text.RegularExpressions;
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

        [MenuItem("Tools/AssetBundle/Make AssetBundle Names")]
        public static void MakeAssetBundleName()
        {
            MakeAllAssetBundleNames();
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

        private static void MakeAllAssetBundleNames()
        {
            AssetBundleSetting setting =
                AssetDatabase.LoadAssetAtPath<AssetBundleSetting>(AppDefine.AssetBundleSettingName);
            if (setting == null)
                return;
            NameMappingTable nameTable =
                AssetDatabase.LoadAssetAtPath<NameMappingTable>(AppDefine.NameMappingTableName);
            if (nameTable == null)
            {
                nameTable = ScriptableObject.CreateInstance<NameMappingTable>();
                AssetDatabase.CreateAsset(nameTable, AppDefine.NameMappingTableName);
            }

            foreach (var info in setting.Directory)
                ProcessSingleDirectoryBundle(nameTable, info);
        }

        private static void BuildBundle(BuildTarget target)
        {
            InternalBuildBundle(target);
        }

        private static void ProcessSingleDirectoryBundle(NameMappingTable nameTable, AssetBundleSettingInfo info)
        {
            if (info == null || info.file == null) return;
            string fileName = AssetDatabase.GetAssetPath(info.file); //  Assets/Content/DynamicAssets/Prefabs/Character 
            if (Directory.Exists(fileName))
            {
                string[] filenames = Directory.GetFiles(fileName, "*.*", SearchOption.AllDirectories);
                foreach (var t in filenames)
                {
                    string f = t.Replace("\\", "/");
                    if (string.IsNullOrEmpty(f) || f.Contains(".meta") || !f.Contains(".")) continue;
                    if (!IsValidFile(f)) continue;
                    CheckChineseName(fileName);
                    var assetImporter = AssetImporter.GetAtPath(f);
                    assetImporter.assetBundleName = nameTable.GetNameMapping(f);
                }
            }
            else
                GameLog.LogError($"{info.file} is not exist");
        }

        private static bool IsValidFile(string fileName)
        {
            return true;
        }

        private static void CheckChineseName(string fileName)
        {
            bool bChinese = Regex.IsMatch(fileName, @"[\u4e00-\u9fa5]");
            if (bChinese)
                GameLog.LogError("文件名：" + fileName + "包含中文");
        }

        private static void InternalBuildBundle(BuildTarget target)
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