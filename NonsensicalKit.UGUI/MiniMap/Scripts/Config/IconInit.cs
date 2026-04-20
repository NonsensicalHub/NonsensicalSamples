using System;
using NonsensicalKit.Core.Service.Config;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "IconInit", menuName = "ScriptableObjects/IconInit")]
public class IconInit : ConfigObject,ISerializationCallbackReceiver
{
    public IconInitData data;

    public override ConfigData GetData()
    {
        foreach (var icon in data.Icons)
        {
            icon.Prefab = Resources.Load<GameObject>(icon.Path);
        }
        return data;
    }

    public override void SetData(ConfigData cd)
    {
        data = cd as IconInitData;
    }

    public override void BeforeSetData()
    {
        base.BeforeSetData();
        foreach (var icon in data.Icons)
        {
            icon.Path = "Icons/" + icon.Prefab.name ;
        }
    }

    public void OnBeforeSerialize()
    {
        foreach (var icon in data.Icons)
        {
            if (icon.Prefab == null)
                continue;
            icon.Path = "ICON/" + icon.Prefab.name ;
        }
    }

    public void OnAfterDeserialize()
    {
        
    }
}


[System.Serializable]
public class IconInitData : ConfigData 
{
    public List<Icon> Icons = new List<Icon>();
}
