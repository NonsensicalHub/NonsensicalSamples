using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using NonsensicalKit.Core;
using NonsensicalKit.Core.Service;
using NonsensicalKit.Core.Service.Config;
using UnityEngine;

public class IconManager : IconManagerBase<IconInitData>
{
    private ConfigService _configService;
    private bool _isInit = false;

    private readonly Queue<IconItemConfig> _iconItemConfigs = new Queue<IconItemConfig>();

    //private readonly Dictionary<IconItemType,Queue<IconItem>> _iconItemQueuePoolDic = new Dictionary<IconItemType, Queue<IconItem>>();

    //public static IconManager Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;

        ServiceCore.SafeGet<ConfigService>(GetConfig);
    }


    private void GetConfig(ConfigService obj)
    {
        _configService = obj;
        _configService.TryGetConfig<IconInitData>(out _iconInitData);
        Debug.Log($"IconDataCount : {_iconInitData.Icons.Count}");

        _isInit = true;

        while (_iconItemConfigs.Count > 0)
        {
            CreateIcon(_iconItemConfigs.Dequeue());
        }
    }


    #region 回收图标

    public override void StoreIcon(string id)
    {
        var foundKey = _iconItemDic.Keys.FirstOrDefault(key => key.m_ID == id);
        if (foundKey != null)
        {
            _iconItemDic[foundKey].Recycling();
            _iconItemDic.Remove(foundKey);
        }
    }

    public override void StoreIcon(IconItemConfig iconItem)
    {
        if (_iconItemDic.TryGetValue(iconItem, out var value))
        {
            value.Recycling();
            _iconItemDic.Remove(iconItem);
        }
    }

    #endregion

    #region 获取图标

    public override IconItem GetIconItem(IconItemConfig iconItemConfig)
    {
        return _iconItemDic.GetValueOrDefault(iconItemConfig);
    }

    public override IconItem GetIconItem(string id)
    {
        var foundKey = _iconItemDic.Keys.FirstOrDefault(key => key.m_ID == id);
        return _iconItemDic.GetValueOrDefault(foundKey);
    }

    #endregion

    public override void ChangeIcon(IconItemConfig iconItemConfig)
    {
        var iconItem = GetIconItem(iconItemConfig);
        if (iconItem != null)
        {
            iconItem.ChangeConfig(iconItemConfig);
        }
    }

    public void CreateIcon(string id = "", string iconName = "", string type = "Other", Color color = default)
    {
        var iconItemConfig = new IconItemConfig
        {
            m_ID = id,
            m_Name = iconName,
            m_Type = type,
            m_BaseColor = color
        };
        CreateIcon(iconItemConfig);
    }

    #region 创建图标

    protected override void CreateIconCor(IconItemConfig iconItemConfig)
    {
        if (m_needWaitService && _isInit == false)
        {
            _iconItemConfigs.Enqueue(iconItemConfig);
        }
        else
        {
            CreateIcon(iconItemConfig);
        }
    }

    protected override IconItem CreateIcon(IconItemConfig iconItemConfig)
    {
        var icon = _iconInitData.Icons.Find(x => x.Type == iconItemConfig.m_Type);

        if (icon != null)
        {
            var obj = icon.Prefab;
            if (obj == null)
            {
                obj = Resources.Load<GameObject>(icon.Path);
            }

            var iconItem = Instantiate(obj, m_iconPool).GetComponent<IconItem>();
            iconItem?.Init(iconItemConfig);
            if (_iconItemDic.ContainsKey(iconItemConfig))
            {
                Debug.LogWarning($"相同的配置图标已存在!,是否特殊操作?");
            }
            else
            {
                _iconItemDic.Add(iconItemConfig, iconItem);
            }

            return iconItem;
        }

        return null;
    }

    #endregion
}