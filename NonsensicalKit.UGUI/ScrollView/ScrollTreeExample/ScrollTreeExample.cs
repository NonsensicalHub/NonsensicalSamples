using System;
using System.Collections;
using System.Collections.Generic;
using NonsensicalKit.Core;
using NonsensicalKit.UGUI.Table;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NonsensicalKit.UGUI.Samples.Table
{
    public class ScrollTreeExample : MonoBehaviour
    {
        [SerializeField] private ScrollView m_scrollView;
        private RectTransform m_content;

        private List<ScrollTreeNodeInfo> _roots;
        private List<ScrollTreeNodeInfo> _showing;

        private float _minWidth;

        private void Awake()
        {
            m_content = m_scrollView.content;
            _minWidth = m_content.rect.width;
            IOCC.Set<List<ScrollTreeNode>>("crt", new List<ScrollTreeNode>());
        }

        private void Start()
        {
            NonsensicalInstance.Instance.DelayDoIt(0, Init);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var v = _roots[78].Childs;
                var vv = v[v.Count - 2].Childs;
                var vvv = vv[vv.Count - 1].Childs;
                var vvvv = vvv[0];
                Focus(vvvv);
            }
        }

        private void Init()
        {
            // 构造测试数据
            InitData();
            m_scrollView.SetItemSizeFunc((index) =>
            {
                if (_roots.Contains(_showing[index]))
                {
                    return new Vector2(50000, 80);
                }
                else
                {
                    return new Vector2(50000, 50);
                }
            });
            m_scrollView.SetUpdateFunc((index, rectTransform) =>
            {
                rectTransform.GetComponent<ScrollTreeNode>().Init(_showing[index], this);
                UpdateWidth();
            });

            m_scrollView.SetItemCountFunc(() =>
            {
                return _showing.Count;
            });

            Debug.Log(m_scrollView);
            m_scrollView.UpdateData(false);
        }

        private void UpdateWidth()
        {
            var v = IOCC.Get<List<ScrollTreeNode>>("crt");

            float max = _minWidth;
            foreach (var item in v)
            {
                var vv = item.GetWidth();
                if (max < vv)
                {
                    max = vv;
                }
            }
            m_content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, max);
        }

        private void InitData()
        {
            _roots = new List<ScrollTreeNodeInfo>();
            for (int i = 0; i <= 100; ++i)
            {
                var root = new ScrollTreeNodeInfo("I_" + i + "_" + Guid.NewGuid().ToString().Substring(0, 8), 0);
                _roots.Add(root);
                int jLength = Random.Range(2, 7);
                for (int j = 0; j < jLength; j++)
                {
                    var jj = new ScrollTreeNodeInfo("II_" + j + "_" + Guid.NewGuid().ToString().Substring(0, 8), 1);
                    root.Childs.Add(jj);
                    jj.Parent = root;
                    int kLength = Random.Range(1, 6);
                    for (int k = 0; k < kLength; k++)
                    {
                        var kk = new ScrollTreeNodeInfo("III_" + k + "_" + Guid.NewGuid().ToString().Substring(0, 8), 2);
                        jj.Childs.Add(kk);
                        kk.Parent = jj;
                        int lLength = Random.Range(0, 5);
                        for (int l = 0; l < lLength; l++)
                        {
                            var ll = new ScrollTreeNodeInfo("IV_" + l + "_" + Guid.NewGuid().ToString().Substring(0, 8), 3);
                            kk.Childs.Add(ll);
                            ll.Parent = kk;
                        }
                    }
                }
            }
            _showing = new List<ScrollTreeNodeInfo>();
            foreach (var item in _roots)
            {
                _showing.Add(item);
            }
        }

        public void Fold(ScrollTreeNodeInfo info)
        {
            if (_showing.Contains(info) == false)
            {
                Debug.LogError("Error");
                return;
            }
            if (info.IsExpanded == false)
            {
                return;
            }
            info.IsExpanded = false;
            FoldInternal(info.Childs, _showing.IndexOf(info) + 1);

            m_scrollView.UpdateData(false);
        }
        private int FoldInternal(List<ScrollTreeNodeInfo> infos, int index)
        {
            foreach (var item in infos)
            {
                _showing.RemoveAt(index);
                if (item.IsExpanded)
                {
                    index = FoldInternal(item.Childs, index);
                }
            }
            return index;
        }

        public void Unfold(ScrollTreeNodeInfo info)
        {
            if (_showing.Contains(info) == false)
            {
                Debug.LogError("Error");
                return;
            }
            if (info.IsExpanded == true)
            {
                return;
            }
            info.IsExpanded = true;
            UnfoldInternal(info.Childs, _showing.IndexOf(info) + 1);
            m_scrollView.UpdateData(false);
        }

        private int UnfoldInternal(List<ScrollTreeNodeInfo> infos, int index)
        {
            foreach (var item in infos)
            {
                _showing.Insert(index, item);
                index++;
                if (item.IsExpanded)
                {
                    index = UnfoldInternal(item.Childs, index);
                }
            }
            return index;
        }

        private void Focus(ScrollTreeNodeInfo info)
        {
            var parent = info;
            Stack<ScrollTreeNodeInfo> stack = new Stack<ScrollTreeNodeInfo>();

            while (parent.Parent != null)
            {
                stack.Push(parent.Parent);
                parent = parent.Parent;
            }

            while (stack.Count > 0)
            {
                var v = stack.Pop();
                if (v.IsExpanded == false)
                {
                    Unfold(v);
                }
            }

            StartCoroutine(Delay(info));
        }

        private IEnumerator Delay(ScrollTreeNodeInfo info)
        {
            var v = _showing.IndexOf(info);
            if (v > 0)
            {
                v--;
            }

            m_scrollView.ScrollTo(v);

            yield return null;
            IOCC.Publish("scrollTreeNodeFocus", info);
        }
    }
}
