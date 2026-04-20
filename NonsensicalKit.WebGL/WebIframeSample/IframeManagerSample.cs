using NonsensicalKit.UGUI;
using UnityEngine;
using UnityEngine.UI;

namespace NonsensicalKit.WebGL.Samples
{
    public class IframeManagerSample : MonoBehaviour
    {
        [SerializeField] private RectTransform m_rect_1;
        [SerializeField] private RectTransform m_rect_2;
        [SerializeField] private Button m_btn_change;
        [SerializeField] private Button m_btn_move;
        [SerializeField] private Button m_btn_setUrl;
        [SerializeField] private Button m_btn_close;
        [SerializeField] private Button m_btn_info;
        [SerializeField] private Button m_btn_closeAll;

     private   Vector3[] _vs = new Vector3[2];

        private void Awake()
        {
            m_btn_change.onClick.AddListener(Change);
            m_btn_move.onClick.AddListener(Move);
            m_btn_setUrl.onClick.AddListener(SetUrl);
            m_btn_close.onClick.AddListener(Close);
            m_btn_info.onClick.AddListener(Info);
            m_btn_closeAll.onClick.AddListener(CloseAll);
        }

        private void Change()
        {
            m_rect_1.GetWorldMinMax(ref _vs);
            var min = _vs[0];
            var max = _vs[1];
            WebIframe.Instance.Change(min.x / Screen.width, min.y / Screen.height, max.x / Screen.width, max.y / Screen.height,"https://www.baidu.com");
        }

        private void Move()
        {
            m_rect_2.GetWorldMinMax(ref _vs);
            var min = _vs[0];
            var max = _vs[1];
            WebIframe.Instance.Move(min.x / Screen.width, min.y / Screen.height, max.x / Screen.width, max.y / Screen.height);
        }

        private void SetUrl()
        {
            WebIframe.Instance.SetUrl("https://www.google.com");
        }

        private void Info()
        {
            WebIframe.Instance.Info();
        }

        private void Close()
        {
            WebIframe.Instance.Close();
        }

        private void CloseAll()
        {
            WebIframe.Instance.CloseAll();
        }
    }
}