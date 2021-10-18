using System;
using System.Collections.Generic;
using System.IO;
using GFrame;
using UnityEngine;

[Serializable]
public class NameMappingInfo
{
    public string name;
    public uint id;
}

public class NameMappingTable : ScriptableObject
{
    public List<NameMappingInfo> mapTable;

    public string CreateAndGetNameMapping(string fileName)
    {
        mapTable ??= new List<NameMappingInfo>();

        string path = Path.GetDirectoryName(fileName);
        string name = Path.GetFileNameWithoutExtension(fileName).Replace(" ", "");
        Debug.Log($"Create Name Mapping [fileName:{fileName}]  [path: {path}]  [name: {name}] ");
        if (string.IsNullOrEmpty(path))
            return (name + AppDefine.ExtName).ToLower();

        var mapInfo = mapTable.Find(t => t.name == path);
        if (mapInfo == null)
        {
            mapInfo = new NameMappingInfo
            {
                name = path,
                id = (uint)path.GetHashCode()
            };
            if (mapTable.Exists(it => it.id == mapInfo.id))
                Debug.Log("name map hash code conflict detected, pls change path name:" + mapInfo.name);
            else
                mapTable.Add(mapInfo);
        }

        return (name + "." + mapInfo.id).ToLower();
    }
}