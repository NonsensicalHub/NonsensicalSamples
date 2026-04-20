using UnityEngine;

public class MeasureIconComponent : IconComponent 
{
    [SerializeField] private IconItemMeasure m_iconItem;

    public override void ChangeIcon(IconItemConfig iconItemConfig)
    {
       // base.ChangeIcon(iconItemConfig);
       IconManager.Instance.ChangeIcon(iconItemConfig);
    }

    public void SetStatus(IconStatus status)
    {
        if ( IconManager.Instance == null)
        {
            Debug.LogWarning("请先初始化IconManager");
            return;
        }

        m_iconItem ??= GetIconItem();
        m_iconItem?.SetStatus(status, true);
        SetWave(status != IconStatus.Normal);
    }

    public void SetWave(bool wave)
    {
        if (! IconManager.Instance)
        {
            Debug.LogWarning("请先初始化IconManager");
            return;
        }

        m_iconItem ??= GetIconItem();
        m_iconItem.SetWave(wave);
    }
    
    public void SetIconItem(IconItem iconItem)
    {
        m_iconItem = iconItem as IconItemMeasure;
    }
    
    public override void DestroyIcon()
    {
        base.DestroyIcon();
        m_iconItem = null;
    }

    public override void DestroyIcon(string id)
    {
        base.DestroyIcon(id);
        m_iconItem = null;
    }
    
    private IconItemMeasure GetIconItem()
    {
        if (m_iconItem == null)
        {
            if (IconManager.Instance == null)
            {
                Debug.LogWarning("请先初始化IconManager");
                return  null;
            }

            //感觉会存在找到同一个item的可能
            var icon = IconManager.Instance.GetIconItem(m_iconItemConfig);
            if (icon != null && icon is IconItemMeasure measureIcon)
            {
                m_iconItem = measureIcon;
            }
            else
            {
                Debug.LogWarning($"未找到测点ICON或测点ICON类型错误:  测点ID{m_iconItemConfig.m_ID} ");
            }
        }

        return m_iconItem;
    }
    
}
