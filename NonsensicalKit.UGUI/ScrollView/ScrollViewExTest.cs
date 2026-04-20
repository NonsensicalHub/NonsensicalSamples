using System.Collections.Generic;
using NonsensicalKit.Core;
using NonsensicalKit.UGUI.Table;
using TMPro;
using UnityEngine;

namespace NonsensicalKit.UGUI.Samples.Table
{
    public class ScrollViewExTest : MonoBehaviour
    {
        [SerializeField] private ScrollViewEx m_scrollViewEx;

        private List<string> _test;

        private void Start()
        {
            NonsensicalInstance.Instance.DelayDoIt(0, S);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_scrollViewEx.ScrollTo(Random.Range(0, _test.Count));
            }
        }

        private void S()
        {
            // 构造测试数据
            InitData();

            m_scrollViewEx.SetUpdateFunc((index, rectTransform) =>
            {
                rectTransform.GetComponentInChildren<TextMeshProUGUI>().text = _test[index];
            });

            m_scrollViewEx.SetItemCountFunc(() =>
            {
                return _test.Count;
            });

            m_scrollViewEx.UpdateData(false);
        }

        private void InitData()
        {
            _test = new List<string>();
            for (int i = 1; i <= 654321; ++i)
            {
                _test.Add(i.ToString());
            }
        }
    }
}
