using System.Collections.Generic;
using NonsensicalKit.Core;
using NonsensicalKit.Tools;
using NonsensicalKit.UGUI.Table;
using TMPro;
using UnityEngine;

namespace NonsensicalKit.UGUI.Samples.Table
{
    public class ScrollViewTest : MonoBehaviour
    {
        [SerializeField] private ScrollView m_scrollView;

        private List<string> _test;

        private void Start()
        {
            NonsensicalInstance.Instance.DelayDoIt(0, S);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var r = Random.Range(0, _test.Count);
                Debug.Log("滚动至:" + (r+1), gameObject);
                m_scrollView.ScrollTo(r,0);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                //var r = Random.Range(0, _test.Count);
                var r = _test.Count - 1;
               var v= m_scrollView.GetScrollValue(r, 1);
                Debug.Log($"滚动至:{r+1}，值为{v}", gameObject);
                m_scrollView.DoScrollTo(new Vector2(m_scrollView.horizontalNormalizedPosition,v),0.5f);
            }
        }

        private void S()
        {
            // 构造测试数据
            InitData();

            m_scrollView.SetUpdateFunc((index, rectTransform) =>
            {
                rectTransform.GetComponentInChildren<TextMeshProUGUI>().text = _test[index];
            });

            m_scrollView.SetItemCountFunc(() =>
            {
                return _test.Count;
            });

            m_scrollView.UpdateData(false);
        }

        private void InitData()
        {
            _test = new List<string>();
            for (int i = 1; i <= 10000; ++i)
            {
                _test.Add(i.ToString());
            }
        }
    }
}
