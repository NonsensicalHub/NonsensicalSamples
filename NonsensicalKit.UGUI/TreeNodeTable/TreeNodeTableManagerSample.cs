using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NonsensicalKit.UGUI.Table;
using UnityEngine;
using UnityEngine.UI;

namespace NonsensicalKit.UGUI.Samples.Table
{
    public class TreeNodeTableManagerSample : TreeNodeTableManagerBase<TreeNodeTableElementSample, TreeNodeClassSample>
    {
        [SerializeField] private ScrollRect m_scrollrect;

        protected override void Awake()
        {
            base.Awake();

            Subscribe<List<TreeNodeClassSample>>("treeNodeTableSample", InitTable);
            Subscribe<TreeNodeClassSample>("NodeFocus", Focus);
        }

        private void Focus(TreeNodeClassSample nodeClass)
        {
            var v = FocusNode(nodeClass);

            var childCount = m_group.childCount - (m_childPrefab ? m_prefabs.Length : 0);
            var index = v.transform.GetSiblingIndex() - (m_childPrefab ? m_prefabs.Length : 0);
            StartCoroutine(Wait((1 - (float)index / childCount), nodeClass.Level / _levels.Max()));

        }

        private IEnumerator Wait(float v, float h)
        {
            Debug.Log(v);
            Debug.Log(h);
            yield return null;
            m_scrollrect.verticalNormalizedPosition = v;
            m_scrollrect.horizontalNormalizedPosition = h;
        }
    }
}
