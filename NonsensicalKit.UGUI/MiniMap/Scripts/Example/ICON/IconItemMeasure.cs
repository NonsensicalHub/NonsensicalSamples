using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class IconItemMeasure : IconItem
{
    [ShowNonSerializedField] private IconStatus _iconStatus;

    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
    private static readonly int OutLineColor = Shader.PropertyToID("_LineColor");
    private static readonly int OutlineBaseWidth = Shader.PropertyToID("_BaseOutlineWidth");
    private static readonly int ExpandRangeOut = Shader.PropertyToID("_ExpandRangeOut");
    private static readonly int ExpandRangeIn = Shader.PropertyToID("_ExpandRangeIn");
    private static readonly int PulseSpeed = Shader.PropertyToID("_PulseSpeed");
    private Material _uiMaterial;

    private void Awake()
    {
        _uiMaterial = new Material(m_icon.material);
        m_icon.material = _uiMaterial;
    }

    protected override void SetConfig(IconItemConfig config)
    {
        base.SetConfig(config);

        if (config.m_Visible == false) return;
        if (m_icon.material != null)
        {
            _uiMaterial.SetColor(BaseColor, config.m_BaseColor);
        }
    }

    public void SetStatus(IconStatus status, bool setBaseColor = false)
    {
        _iconStatus = status;
        if (m_icon.material != null)
        {
            _uiMaterial.SetColor(OutLineColor, status switch
            {
                IconStatus.Normal => Color.green,
                IconStatus.Warning => Color.yellow,
                IconStatus.Danger => Color.red,
                _ => Color.white
            });
            if (setBaseColor)
            {
                _uiMaterial.SetColor(BaseColor, status switch
                {
                    IconStatus.Normal => Color.green,
                    IconStatus.Warning => Color.yellow,
                    IconStatus.Danger => Color.red,
                    _ => Color.white
                });
            }

            RefreshIconInMask();    
        }
    }

    public void SetWave(bool wave)
    {
        if (wave)
        {
            _uiMaterial.SetFloat(OutlineBaseWidth, 3);
            _uiMaterial.SetFloat(ExpandRangeOut, 10);
            _uiMaterial.SetFloat(ExpandRangeIn, 60);
            _uiMaterial.SetFloat(PulseSpeed, 4.5f);
        }
        else
        {
            _uiMaterial.SetFloat(OutlineBaseWidth, 0);
            _uiMaterial.SetFloat(ExpandRangeOut, 0);
            _uiMaterial.SetFloat(ExpandRangeIn, 0);
            _uiMaterial.SetFloat(PulseSpeed, 0);
        }
    }
}
