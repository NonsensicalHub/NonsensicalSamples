using NonsensicalKit.Core;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NonsensicalKit.UGUI.Samples.Table
{
    public class ScrollTreeNode : NonsensicalMono
    {
        [SerializeField] private Button m_btn_expand;
        [SerializeField] private Button m_btn_collapse;
        [SerializeField] private TextMeshProUGUI m_txt_name;
        [SerializeField] private RectTransform m_box;

        private ScrollTreeNodeInfo _nodeInfo;
        private ScrollTreeExample _tree;

        private void Awake()
        {
            m_btn_expand.onClick.AddListener(OnExpandClick);
            m_btn_collapse.onClick.AddListener(OnCollapseClick);

            Subscribe<ScrollTreeNodeInfo>("scrollTreeNodeFocus", OnFocus);
        }

        private void OnEnable()
        {
            IOCC.Get<List<ScrollTreeNode>>("crt").Add(this);
        }

        private void OnDisable()
        {
            IOCC.Get<List<ScrollTreeNode>>("crt").Remove(this);
        }

        public void Init(ScrollTreeNodeInfo info, ScrollTreeExample tree)
        {
            if (_nodeInfo == info)
            {
                return;
            }
            _nodeInfo = info;
            _tree = tree;
            m_txt_name.text = info.Name;
            if (info.Childs.Count == 0)
            {
                m_btn_expand.gameObject.SetActive(false);
                m_btn_collapse.gameObject.SetActive(false);
            }
            else
            {
                m_btn_expand.gameObject.SetActive(!info.IsExpanded);
                m_btn_collapse.gameObject.SetActive(info.IsExpanded);
            }
            m_box.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, info.Level * 30, 100);
        }

        public float GetWidth()
        {
            m_txt_name.ForceMeshUpdate();
            var width = 100 + _nodeInfo.Level * 30 + m_txt_name.bounds.size.x;
            GetComponent<RectTransform>().sizeDelta = new Vector2(width, GetComponent<RectTransform>().sizeDelta.y);
            return width;
        }

        private void OnExpandClick()
        {
            _tree.Unfold(_nodeInfo);
            m_btn_expand.gameObject.SetActive(false);
            m_btn_collapse.gameObject.SetActive(true);
        }

        private void OnCollapseClick()
        {
            _tree.Fold(_nodeInfo);
            m_btn_expand.gameObject.SetActive(true);
            m_btn_collapse.gameObject.SetActive(false);
        }

        private void OnFocus(ScrollTreeNodeInfo info)
        {
            if (info == _nodeInfo)
            {
                m_txt_name.color = Color.red;
            }
            else
            {
                m_txt_name.color = Color.black;
            }
        }
    }

    public class ScrollTreeNodeInfo
    {
        public List<ScrollTreeNodeInfo> Childs = new List<ScrollTreeNodeInfo>();
        public ScrollTreeNodeInfo Parent;
        public bool IsExpanded;

        public string Name;
        public int Level;

        public ScrollTreeNodeInfo(string name, int level)
        {
            Name = name;
            Level = level;
        }
    }
}
