using System.Collections.Generic;
using NonsensicalKit.Tools;
using NonsensicalKit.UGUI.Table;
using TMPro;
using UnityEngine;

namespace NonsensicalKit.UGUI.Samples.Table
{
    public class ScrollView_MK2Test : MonoBehaviour
    {
        [SerializeField] private ScrollView_MK2 m_scrollView_MK2;

        private List<string> _test;

        private void Start()
        {
            S();
            //NonsensicalInstance.Instance.DelayDoIt(0, S);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_scrollView_MK2.ScrollTo(Random.Range(0, _test.Count));
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                InitData(1);
                m_scrollView_MK2.UpdateData(false);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                var r = Random.Range(0, _test.Count);
                var v = m_scrollView_MK2.GetScrollValue(r, 0);
                Debug.Log($"获取滚动到{r+1}的值为{v}");
                m_scrollView_MK2.DoScrollTo(new Vector2(v, m_scrollView_MK2.verticalNormalizedPosition), 0.5f);
            }
        }

        private void S()
        {
            // 构造测试数据
            InitData(1);

            m_scrollView_MK2.SetUpdateFunc((index, rectTransform) =>
            {
                var t = rectTransform.GetComponentInChildren<TextMeshProUGUI>().text;// += "_"+_test[index];
                if (string.IsNullOrEmpty(t))
                {
                    rectTransform.GetComponentInChildren<TextMeshProUGUI>().text = _test[index] + "_" + 1;
                }
                else
                {
                    var sp = t.Split('_');
                    var countString = sp[1];
                    int count = int.Parse(countString);
                    count++;
                    rectTransform.GetComponentInChildren<TextMeshProUGUI>().text = _test[index] + "_" + count;
                }
            });

            m_scrollView_MK2.SetItemCountFunc(() =>
            {
                return _test.Count;
            });

            m_scrollView_MK2.UpdateData(false);
        }

        private void InitData(int count)
        {
            _test = new List<string>();
            for (int i = 1; i <= count * 100000; ++i)
            {
                _test.Add(i.ToString() );
            }
        }
    }
}
