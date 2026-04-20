using System;
using System.Collections;
using System.Collections.Generic;
using NonsensicalKit.Core;
using NonsensicalKit.Tools.PlayerController;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapInteraction : NonsensicalMono
{
    [SerializeField] private List<MinoMapMoveArea> m_moveAreas;
    private Dictionary<DeviceArea, MinoMapMoveArea> _moveAreaPosition;
    private readonly Dictionary<string, MinoMapMoveArea> _areaIDToMoveAreaPosition=new ();

    private void Awake()
    {
        _moveAreaPosition = new Dictionary<DeviceArea, MinoMapMoveArea>();
        foreach (var moveArea in m_moveAreas)
        {
            _moveAreaPosition.Add(moveArea.MoveArea, moveArea);
            if (string.IsNullOrEmpty(moveArea.AreaID) == false)
            {
                _areaIDToMoveAreaPosition.Add(moveArea.AreaID, moveArea);
            }
        }

        Subscribe<DeviceArea, PointerEventData>("minimapMoveAreaClick", OnMiniMapClick);
        AddHandler<string, MinoMapMoveArea>("getMinimapMoveArea", GetMinimapMoveArea);
    }

    private MinoMapMoveArea GetMinimapMoveArea(string id)
    {
        if (_areaIDToMoveAreaPosition.ContainsKey(id))
        {
            return _areaIDToMoveAreaPosition[id];
        }
        return null;
    }

    private void OnMiniMapClick(DeviceArea arg1, PointerEventData arg2)
    {
        if (_moveAreaPosition.TryGetValue(arg1, out var moveArea))
        {
            Publish("miniMapDrivePlayerMove", moveArea);
        }
    }
}

[System.Serializable]
public class MinoMapMoveArea
{
    public string AreaID="";
    public DeviceArea MoveArea;
    public Vector3 Position;
    public Vector3 LookAtPoint;
}
