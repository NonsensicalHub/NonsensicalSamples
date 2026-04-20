using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NonsensicalKit.Core;
using UnityEngine;
using UnityEngine.Serialization;

public class MeasureAreaIconComponent : NonsensicalMono
{
    [SerializeField] private bool m_initShow;
    [FormerlySerializedAs("_areaIcons")] [SerializeField] private List<MeasureAreaIconItem> m_areaIcons = new();

    private void Awake()
    {
        Subscribe("SceneMeasurePointCountChange", SceneMeasurePointCountChange);
        foreach (var item in m_areaIcons)
        {
          item.m_GameObject.SetActive(m_initShow);      
        }
    }

    private void SceneMeasurePointCountChange()
    {
        foreach (var area in m_areaIcons)
        {
            //var existingIcon = DataCenter.Instance.GetAllModelToMeasurePoint().FirstOrDefault(x => x.m_PointID == area.m_ID);
            //area.m_GameObject.SetActive(existingIcon != null);
        }
    }
}

[System.Serializable]
public class MeasureAreaIconItem
{
    public string m_ID;
    public GameObject m_GameObject;
}
