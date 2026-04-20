using UnityEngine;
using UnityEngine.EventSystems;

// 导入 IPointerUpHandler 接口
public class ResizeTrigger : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [Header("设置")]
    [Tooltip("指定该触发器对应的边缘类型")]
    [SerializeField] private WindowResizeHandler.ResizeEdge m_resizeEdge = WindowResizeHandler.ResizeEdge.Right;

    [Header("光标纹理")]
    [SerializeField, Tooltip("光标热点。建议设为图片中心，如 32x32 图片则为 Vector2(16, 16)")]
    private Vector2 m_cursorHotspot = new Vector2(16, 16);


    private WindowResizeHandler _handler;
    private RectTransform _parentRect; // 主窗口的 RectTransform
    private Vector2 _startMousePos;
    private bool _isDragging = false;
    private bool _isHovering = false;

    private Texture2D _cursorH;
    private Texture2D _cursorV;
    private Texture2D _cursorDL;
    private Texture2D _cursorDR; // 确保大小写一致

    private void Awake()
    {
        _handler = GetComponentInParent<WindowResizeHandler>();
        if (_handler == null)
        {
            Debug.LogError("ResizeTrigger requires a WindowResizeHandler component on a parent object.");
            enabled = false;
            return;
        }

        // 通过 Handler 获取光标纹理，保持触发器的 Inspector 清洁
        var cursors = _handler.GetCursorSprite();
        _cursorH = cursors.Item1;
        _cursorV = cursors.Item2;
        _cursorDL = cursors.Item3;
        _cursorDR = cursors.Item4;

        _parentRect = _handler.GetComponent<RectTransform>();
    }

    // --- 接口实现 ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovering = true;
        SetCursor(m_resizeEdge);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovering = false;
        // 如果不在拖拽状态，立即复位光标
        if (!_isDragging)
        {
            SetCursor(WindowResizeHandler.ResizeEdge.None);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 只响应鼠标左键
        if (eventData.button != PointerEventData.InputButton.Left) return;

        // 记录鼠标在父RectTransform空间中的起始位置
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, eventData.position, eventData.pressEventCamera, out _startMousePos);
        _isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        // 【关键修正】：确保拖拽逻辑只在主鼠标按钮上触发
        if (eventData.button != PointerEventData.InputButton.Left) return;

        // 获取当前鼠标位置
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, eventData.position, eventData.pressEventCamera, out var currentMousePos);

        // 计算鼠标位移量
        Vector2 diff = currentMousePos - _startMousePos;

        // 将尺寸变化量和边缘类型传递给主 Handler 进行处理
        _handler.ApplyResizeDelta(m_resizeEdge, diff);

        // 重新设置起始位置，这是实现持续拖拽的关键！
        _startMousePos = currentMousePos;
    }

    public void OnPointerUp(PointerEventData eventData) // <-- 鼠标抬起时，解除拖拽状态
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        _isDragging = false;

        // 【关键修正】：如果鼠标在抬起时不在任何触发器上，立即复位光标
        if (!_isHovering)
        {
            SetCursor(WindowResizeHandler.ResizeEdge.None);
        }
    }

    // --- 光标系统 ---

    private void SetCursor(WindowResizeHandler.ResizeEdge edge)
    {
        Texture2D tex = null;
        var hotspot = Vector2.zero;

        // 使用 C# 8.0 的 switch 表达式简化逻辑
        tex = edge switch
        {
            WindowResizeHandler.ResizeEdge.Left or WindowResizeHandler.ResizeEdge.Right => _cursorH,
            WindowResizeHandler.ResizeEdge.Top or WindowResizeHandler.ResizeEdge.Bottom => _cursorV,
            WindowResizeHandler.ResizeEdge.TopLeft or WindowResizeHandler.ResizeEdge.BottomRight => _cursorDL,
            WindowResizeHandler.ResizeEdge.TopRight or WindowResizeHandler.ResizeEdge.BottomLeft => _cursorDR,
            _ => null
        };

        if (tex != null) hotspot = m_cursorHotspot;

        Cursor.SetCursor(tex, hotspot, CursorMode.Auto);
    }
}
