using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AssetBundleSettingInfo
{
    public UnityEngine.Object file;
}

[CreateAssetMenu(fileName = "AssetBundleSetting", menuName = "AssetBundlePack/AssetBundleSetting")]
public class AssetBundleSetting : ScriptableObject
{
    public List<AssetBundleSettingInfo> Directory;
}