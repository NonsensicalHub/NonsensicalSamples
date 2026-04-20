using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using NonsensicalKit.Core;
using UnityEngine;

public class CreateTest : NonsensicalMono
{
    [SerializeField] private IconItemConfig config;

    [SerializeField] private IconItem _iconItem;
    [SerializeField,Label("带返回")] private IconItem _iconItemTemp;
    

    [Button]
    private void CreateIconWithThisConfig()
    {
        IOCC.Publish("createIcon", config);
    }
    [Button]
    private void CreateIconWithThisConfigWithCallback()
    {
        _iconItemTemp = Execute<IconItemConfig, IconItem>("createIcon", config);
    }
    
    [Button] 
    private void ChangeIconItemWithThisConfig()
    {
        _iconItem.ChangeConfig(config);
    }
}