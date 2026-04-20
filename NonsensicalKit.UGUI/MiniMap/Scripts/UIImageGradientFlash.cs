using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIImageGradientFlash : MonoBehaviour
{
    [SerializeField] private bool m_useBlink = true;
    public Gradient m_Gradient;


    [Header("闪烁速度")]
    public float m_Speed = 2f;

    private Graphic _target;
    private float _time;

    private void Awake()
    {
        _target = GetComponent<Graphic>();

        if (_target == null)
            Debug.LogError("UIImageGradientFlash 需要挂在含有 Graphic（Image/Text）的对象上！");
    }

    private void Update()
    {
        if (m_useBlink == false)
        {
            if (_time != 0)
            {
                _time = 0;
                _target.color = m_Gradient.Evaluate(_time);
            }

            return;
        }

        _time = (_time + Time.deltaTime * m_Speed) % 1;

        // 从渐变颜色条获取颜色
        _target.color = m_Gradient.Evaluate(_time);
    }
}
