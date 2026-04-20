using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIconComponent : IconComponent
{
    public override void ChangeIcon(IconItemConfig iconItemConfig)
    {
        // base.ChangeIcon(iconItemConfig);
        IconManager.Instance.ChangeIcon(iconItemConfig);
    }
}