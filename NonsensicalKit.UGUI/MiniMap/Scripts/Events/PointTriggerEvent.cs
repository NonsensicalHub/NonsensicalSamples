using System.Collections;
using NaughtyAttributes;
using NonsensicalKit.Core;
using UnityEngine;

public class PointTriggerEvent : MonoBehaviour
{
    [SerializeField] private string m_pointID;
    [SerializeField] private bool m_enableF;
    [SerializeField] private GameObject m_particleTarget;
    [SerializeField] private Transform m_iconTarget;

    [SerializeField, Label("进入和退出动画时间")] private Vector2 m_duration = new Vector2(0.2f, 0.3f);

    [Header("测点模型")]
    [SerializeField] private string m_measurementTag;

    [SerializeField] private Material[] m_materials;
    [SerializeField, Label("测量模型")] private MeshRenderer m_target;

    [Header("配置信息面板")]
    [SerializeField, Label("图表")] private GameObject m_table;

    [SerializeField] private GameObject m_tableContent;


    private Coroutine _scaleCoroutine;

    private bool _isEnter, _showTable;
    private bool _isSetMeasurementModel;

    private bool IsSetMeasurementModel
    {
        get => _isSetMeasurementModel;
        set
        {
            _isSetMeasurementModel = value;
            m_tableContent.SetActive(value);
        }
    }

    private void Awake()
    {
        IOCC.Subscribe<string>("ShowTable", ShowTable);
        IOCC.Subscribe<bool, string>("setMeasurementModel", SetMeasurementModel);
        m_iconTarget.transform.localScale = Vector3.zero;
    }


    private void OnTriggerEnter(Collider other) => SetTarget(true);

    private void OnTriggerExit(Collider other)
    {
        SetTarget(false);
        m_tableContent.SetActive(false);
    }

    private void Update()
    {
        if (_isEnter && Input.GetKeyDown(KeyCode.F) && m_enableF)
        {
            //放置模型
            //IOCC.Publish("ShowTable", m_pointID);
            IOCC.Publish("fPress");
        }
    }

    public void ShowTable(bool isShow)
    {
        m_table.SetActive(isShow);
        _showTable = isShow;
    }

    private void ShowTable(string obj)
    {
        if (obj == m_pointID)
        {
            //m_table.SetActive(true);
            //m_iconTarget.localScale = Vector3.zero;
            m_target.gameObject.SetActive(true);
            IsSetMeasurementModel = true;
            m_target.material = IsSetMeasurementModel ? m_materials[1] : m_materials[0];
        }
        /*else
        {
            Reset();
        }*/
    }

    private void SetTarget(bool isEnter)
    {
        _isEnter = isEnter;
        IOCC.Set<PointTriggerEvent>("nowPoint", isEnter ? this : null);
        if (_scaleCoroutine != null) StopCoroutine(_scaleCoroutine);

        m_particleTarget.SetActive(!isEnter);
        if (IsSetMeasurementModel)
        {
            m_target?.gameObject.SetActive(true);
            m_tableContent.SetActive(true);
        }
        else
        {
            if (m_enableF == false)
                m_target?.gameObject.SetActive(isEnter);
        }

        _scaleCoroutine = StartCoroutine(ScaleToTarget(isEnter ? Vector3.one : Vector3.zero, isEnter ? m_duration.x : m_duration.y));
        m_table.SetActive(_showTable);
    }

    private void SetMeasurementModel(bool isTrue, string obj)
    {
        if (m_target == null || _isEnter == false) return;
        IsSetMeasurementModel = obj == m_measurementTag && isTrue;
        m_target.material = IsSetMeasurementModel ? m_materials[1] : m_materials[0];
    }


    private void Reset()
    {
        if (_showTable == false)
            m_table.SetActive(false);
        SetTarget(false);
    }

    private IEnumerator ScaleToTarget(Vector3 targetScale, float duration)
    {
        Vector3 startScale = m_iconTarget.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            m_iconTarget.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            yield return null;
        }

        m_iconTarget.localScale = targetScale; // 确保最终值准确
    }
}
