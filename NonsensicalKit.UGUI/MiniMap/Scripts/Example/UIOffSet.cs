using UnityEngine;
using NaughtyAttributes;

public enum AxisControl
{
    /// <summary>同时控制X轴和Y轴的偏移</summary>
    Both,

    /// <summary>只控制X轴的偏移</summary>
    X_Only,

    /// <summary>只控制Y轴的偏移</summary>
    Y_Only,
}

/// <summary>
/// 控制rect偏移,适用于某个父级尺寸变化时，保证本节点的位置相对不变
/// </summary>
public class UIOffSet : MonoBehaviour
{
    [Header("RectTransforms")]
    [SerializeField] private RectTransform m_fatherRectTransform;

    [SerializeField] private RectTransform m_thisRectTransform;
    [SerializeField] private Vector2 m_offset;

    [Header("控制选项")]
    [SerializeField, Label("要控制的轴向")]
    private AxisControl m_axisControl = AxisControl.Both;

    private Vector2 _lastFatherSize;

    private void Start()
    {
        if (m_offset == Vector2.zero)
        {
            m_offset = m_thisRectTransform.sizeDelta / 2;
        }
        // 初始设置
        if (m_fatherRectTransform != null)
        {
            _lastFatherSize = m_fatherRectTransform.sizeDelta;
            ApplyOffset(); // 首次执行
        }

    }

    private void LateUpdate()
    {
        if (m_fatherRectTransform == null || m_thisRectTransform == null) return;

        // 仅在父级尺寸变化时执行，提高性能
        if (_lastFatherSize != m_fatherRectTransform.sizeDelta)
        {
            _lastFatherSize = m_fatherRectTransform.sizeDelta;
            ApplyOffset();
        }
    }

    private void ApplyOffset()
    {
        // 1. 获取当前位置作为基础
        Vector2 currentPosition = m_thisRectTransform.anchoredPosition;

        float targetX = m_fatherRectTransform.sizeDelta.x / 2f + m_offset.x;
        float targetY = m_fatherRectTransform.sizeDelta.y / 2f + m_offset.y;

        Vector2 newPosition = currentPosition;

        // 3. 根据 AxisControl 应用新的位置
        switch (m_axisControl)
        {
            case AxisControl.Both:
                newPosition = new Vector2(targetX, targetY);
                break;

            case AxisControl.X_Only:
                // 只更新 X 轴，保留 Y 轴的原始值
                newPosition.x = targetX;
                break;

            case AxisControl.Y_Only:
                // 只更新 Y 轴，保留 X 轴的原始值
                newPosition.y = targetY;
                break;
        }

        m_thisRectTransform.anchoredPosition = newPosition;
    }
}
