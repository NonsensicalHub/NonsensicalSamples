using System;
using System.Collections.Generic;
using NaughtyAttributes;
using NonsensicalKit.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum MiniMapState
{
    Min,
    Max,
    Normal
}

public enum MaskType
{
    None,
    Circle,
    Rectangle
}

public class MiniMapManager : NonsensicalMono
{
    [SerializeField] private bool m_useMapController = true;
    [SerializeField] private MiniMapState m_currentState = MiniMapState.Normal;
    [SerializeField] private MaskType m_maskType;
    [SerializeField] private MapController m_mapController;
    [SerializeField, BoxGroup("Rect")] private CanvasGroup m_mapCanvas;
    [SerializeField, BoxGroup("Rect")] private RectTransform m_containerRectTransform;
    [SerializeField, BoxGroup("Rect")] private RectTransform m_maskRectTransform;
    [SerializeField, BoxGroup("Rect")] private RectTransform m_mapRectTransform;

    [SerializeField, BoxGroup("Button")] private Button m_minButton;
    [SerializeField, BoxGroup("Button")] private Button m_maxButton;
    [SerializeField, BoxGroup("Button")] private Image m_maxButtonImage;
    [SerializeField, BoxGroup("Button")] private Sprite m_minButtonSprite;
    [SerializeField, BoxGroup("Button")] private Sprite m_maxButtonSprite;

    [Header("Mask")]
    [SerializeField] private GameObject[] m_circleMaskGameObjects;

    [SerializeField] private GameObject[] m_rectangleMaskGameObjects;
    [SerializeField] private Sprite m_circleMask;
    [SerializeField] private Sprite m_rectangleMask;
    [SerializeField] private Image m_maskImage;

    [Header("SizeConfig")]
    [SerializeField, Label("启用MapController窗口大小配置")]
    private List<SizeData> m_sizeConfigs;

    [SerializeField, Label("禁用MapController窗口大小配置")]
    private List<SizeData> m_sizeStaticConfigs;

    private readonly Dictionary<MiniMapState, SizeData> _sizeDataDict = new Dictionary<MiniMapState, SizeData>();
    private readonly Dictionary<MiniMapState, SizeData> _staticSizeDataDict = new Dictionary<MiniMapState, SizeData>();

    public Dictionary<MiniMapState, SizeData> SizeDataDict => _sizeDataDict;
    public Dictionary<MiniMapState, SizeData> SizeStaticDataDict => _staticSizeDataDict;

    private void Awake()
    {
        //m_minButton.onClick.AddListener(OnSwitchMinEvent);
        m_maxButton.onClick.AddListener(OnSwitchMaxEvent);
        foreach (var data in m_sizeConfigs)
        {
            _sizeDataDict.Add(data.m_MiniMapStateState, data);
        }

        foreach (var data in m_sizeStaticConfigs)
        {
            _staticSizeDataDict.Add(data.m_MiniMapStateState, data);
        }

        SetMaskKind(m_maskType);

        Subscribe<Vector2>("iconItemPositionUpdate", "Player", OnIconItemPositionUpdate);
        Subscribe("hideMiniMap", HideMiniMap);

        m_mapController.enabled = m_useMapController;
        m_maxButtonImage.sprite = m_minButtonSprite;
    }

    private void HideMiniMap()
    {
        m_currentState = MiniMapState.Min;
        SwitchState(m_currentState);
    }

    private void OnIconItemPositionUpdate(Vector2 obj)
    {
        m_mapController.SetPlayerVector2(obj);
    }

    public void OnSwitchMinEvent()
    {
        m_currentState = m_currentState == MiniMapState.Min ? MiniMapState.Normal : MiniMapState.Min;
        SwitchState(m_currentState);
    }

    public void OnSwitchMaxEvent()
    {
        m_currentState = m_currentState == MiniMapState.Max ? MiniMapState.Normal : MiniMapState.Max;
        SwitchState(m_currentState);
    }


    public void OnWindowSizeUpdate(Vector2 size, Vector2 pos, Vector2 delta)
    {
        if (m_useMapController) return;
        m_containerRectTransform.sizeDelta = size;
        m_maskRectTransform.sizeDelta = size;
        m_mapRectTransform.sizeDelta = size;
        _staticSizeDataDict[m_currentState].m_ContainerSize = size;
        _staticSizeDataDict[m_currentState].m_MaskSize = size;
        _staticSizeDataDict[m_currentState].m_MapSize = size;
    }

    public void OnWindowPositionUpdate(Vector2 pos)
    {
        var dic = m_useMapController ? _sizeDataDict : _staticSizeDataDict;
        dic[m_currentState].m_ContainerPosition = pos;
    }


    private void SwitchState(MiniMapState state)
    {
        switch (state)
        {
            case MiniMapState.Min:
                ShowMap(false);
                SwitchMask(m_maskType);
                break;
            case MiniMapState.Max:
                ShowMap(true);
                m_maxButtonImage.sprite = m_maxButtonSprite;
                SwitchMask(MaskType.Rectangle, true);
                SetWindowSize(MiniMapState.Max);
                break;
            case MiniMapState.Normal:
                ShowMap(true);
                m_maxButtonImage.sprite = m_minButtonSprite;
                SwitchMask(m_maskType);
                SetWindowSize(MiniMapState.Normal);
                break;
            default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void SetWindowSize(MiniMapState state)
    {
        var sizeDataDict = m_useMapController ? _sizeDataDict : _staticSizeDataDict;
        m_containerRectTransform.sizeDelta = sizeDataDict[state].m_ContainerSize;
        m_maskRectTransform.sizeDelta = sizeDataDict[state].m_MaskSize;

        m_containerRectTransform.anchoredPosition = sizeDataDict[state].m_ContainerPosition;
        m_maskRectTransform.anchoredPosition = sizeDataDict[state].m_MaskPosition;


        if (sizeDataDict[state].m_UseMapSize)
        {
            m_mapRectTransform.sizeDelta = sizeDataDict[state].m_MapSize;
            if (m_useMapController)
            {
                m_mapController.HandleZoom(0f); // 更新缩放比
            }
            else
            {
                m_mapRectTransform.anchoredPosition = sizeDataDict[state].m_MapPosition;
            }
        }
        else
        {
            if (m_useMapController)
            {
                m_mapController.HandleZoomWithPercentageValue(sizeDataDict[state].m_ScalePercentage);
            }
            else
            {
                Debug.LogError("请使用直接控制窗口大小进行该配置");
            }
        }

        foreach (var showGameObject in sizeDataDict[state].m_ShowGameObjects)
        {
            showGameObject.SetActive(true);
        }

        foreach (var hideGameObject in sizeDataDict[state].m_HideGameObjects)
        {
            hideGameObject.SetActive(false);
        }
    }


    [Button]
    private void SwitchMask()
    {
        m_maskType = m_maskType == MaskType.Circle ? MaskType.Rectangle : MaskType.Circle;
        SetMaskKind(m_maskType);
    }

    private void SwitchMask(MaskType type, bool force = false)
    {
        if (!m_useMapController) return;
        if (force == true)
        {
            SetMaskKind(type);
        }
        else
        {
            m_maskType = type;
            SetMaskKind(m_maskType);
        }
    }

    private void SetMaskKind(MaskType type)
    {
        switch (type)
        {
            case MaskType.Circle:
                SetMask(true);
                m_maskImage.sprite = m_circleMask;
                foreach (var circleMaskGameObject in m_circleMaskGameObjects)
                {
                    circleMaskGameObject.SetActive(true);
                }

                foreach (var rectangleMaskGameObject in m_rectangleMaskGameObjects)
                {
                    rectangleMaskGameObject.SetActive(false);
                }

                break;
            case MaskType.Rectangle:
                SetMask(true);
                m_maskImage.sprite = m_rectangleMask;
                foreach (var circleMaskGameObject in m_circleMaskGameObjects)
                {
                    circleMaskGameObject.SetActive(false);
                }

                foreach (var rectangleMaskGameObject in m_rectangleMaskGameObjects)
                {
                    rectangleMaskGameObject.SetActive(true);
                }

                break;
            case MaskType.None:
                SetMask(false);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);

                void SetMask(bool a)
                {
                    m_maskImage.enabled = a;
                    var mask = m_maskImage.GetComponent<Mask>();
                    if (mask != null)
                    {
                        mask.enabled = a;
                    }
                }
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(this.gameObject);
#endif
    }

    private void ShowMap(bool show)
    {
        m_mapCanvas.interactable = show;
        m_mapCanvas.blocksRaycasts = show;
        m_mapCanvas.alpha = show ? 1 : 0;
    }
}

[Serializable]
public class SizeData
{
    public bool m_UseMapSize;
    public Vector2 m_ContainerSize;
    public Vector2 m_ContainerPosition;
    public Vector2 m_MaskSize;
    public Vector2 m_MaskPosition;
    public Vector2 m_MapSize;
    public Vector2 m_MapPosition;
    public float m_ScalePercentage;
    public MiniMapState m_MiniMapStateState;
    public GameObject[] m_ShowGameObjects;
    public GameObject[] m_HideGameObjects;
}
