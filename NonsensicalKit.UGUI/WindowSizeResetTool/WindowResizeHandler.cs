using System;
using UnityEngine;
using UnityEngine.Events;

public class WindowResizeHandler : MonoBehaviour
{
    [Header("设置")]
    [SerializeField] private RectTransform m_resizeRect;

    [SerializeField, Tooltip("最小窗口尺寸")] private Vector2 m_minSize = new Vector2(150, 100);
    //[SerializeField, Tooltip("是否限制在屏幕范围内 (功能未实现)")] private bool m_clampToScreen = true;

    [SerializeField] private Texture2D m_cursorH;
    [SerializeField] private Texture2D m_cursorV;
    [SerializeField] private Texture2D m_cursorDL;
    [SerializeField] private Texture2D m_cursorDR;

    public UnityEvent<Vector2, Vector2, Vector2> m_OnResize = new();

    // 枚举与原始脚本保持一致
    public enum ResizeEdge { None, Right, Left, Top, Bottom, TopRight, TopLeft, BottomRight, BottomLeft }

    private void Awake()
    {
        if (!m_resizeRect)
        {
            Debug.LogWarning("ResizeRect is missing. Please assign a RectTransform to the ResizeRect field.");
            this.enabled = false;
        }
    }


    /// <summary>
    /// 应用从拖拽边框接收到的尺寸变化量。
    /// </summary>
    /// <param name="edge">当前正在拖拽的边缘类型。</param>
    /// <param name="delta">鼠标从开始拖拽位置到当前位置的差值。</param>
    public void ApplyResizeDelta(ResizeEdge edge, Vector2 delta)
    {
        var startSize = m_resizeRect.sizeDelta;
        var startPos = m_resizeRect.anchoredPosition;
        var newSize = startSize;
        var newPos = startPos;

        switch (edge)
        {
            // X轴处理
            case ResizeEdge.Right or ResizeEdge.TopRight or ResizeEdge.BottomRight:
                newSize.x = Mathf.Max(m_minSize.x, startSize.x + delta.x);
                // 计算新的锚点位置，向右侧偏移变化量的 1/2
                newPos.x = startPos.x + (newSize.x - startSize.x) * 0.5f;
                break;
            case ResizeEdge.Left or ResizeEdge.TopLeft or ResizeEdge.BottomLeft:
                newSize.x = Mathf.Max(m_minSize.x, startSize.x - delta.x);
                // 计算新的锚点位置，向左侧偏移变化量的 1/2
                newPos.x = startPos.x - (newSize.x - startSize.x) * 0.5f;
                break;
        }

        switch (edge)
        {
            // Y轴处理
            case ResizeEdge.Top or ResizeEdge.TopLeft or ResizeEdge.TopRight:
                newSize.y = Mathf.Max(m_minSize.y, startSize.y + delta.y);
                // 计算新的锚点位置，向上侧偏移变化量的 1/2
                newPos.y = startPos.y + (newSize.y - startSize.y) * 0.5f;
                break;
            case ResizeEdge.Bottom or ResizeEdge.BottomLeft or ResizeEdge.BottomRight:
                newSize.y = Mathf.Max(m_minSize.y, startSize.y - delta.y);
                // 计算新的锚点位置，向下侧偏移变化量的 1/2
                newPos.y = startPos.y - (newSize.y - startSize.y) * 0.5f;
                break;
        }

        // 应用新尺寸和位置
        m_resizeRect.sizeDelta = newSize;
        m_OnResize.Invoke(newSize, newPos, delta);
    }

    public (Texture2D, Texture2D, Texture2D, Texture2D) GetCursorSprite()
    {
        return (m_cursorH, m_cursorV, m_cursorDL, m_cursorDR);
    }
}
