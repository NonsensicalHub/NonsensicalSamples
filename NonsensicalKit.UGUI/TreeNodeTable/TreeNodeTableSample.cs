using NonsensicalKit.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NonsensicalKit.UGUI.Samples.Table
{
    public class TreeNodeTableSample : MonoBehaviour
    {
        [SerializeField] private Button m_btn_test;
        private TreeNodeClassSample _epsilon;

        private void Awake()
        {
            m_btn_test.onClick.AddListener(Focus);
        }

        private void Start()
        {
            TreeNodeClassSample root1 = new TreeNodeClassSample("root1");
            for (int i = 0; i < 10; i++)
            {
                root1.AddChild(new TreeNodeClassSample("alpha" + i));
            }

            TreeNodeClassSample gamma = new TreeNodeClassSample("gamma");
            root1.Children[0].AddChild(gamma);

            TreeNodeClassSample root2 = new TreeNodeClassSample("root2");
            for (int i = 0; i < 120; i++)
            {
                root2.AddChild(new TreeNodeClassSample("beta" + i));
            }

            TreeNodeClassSample root3 = new TreeNodeClassSample("root3");
            TreeNodeClassSample crtNode = root3;
            for (int i = 0; i < 30; i++)
            {
                var v = new TreeNodeClassSample("delta" + i);
                crtNode.AddChild(v);
                crtNode = v;
            }
            _epsilon = new TreeNodeClassSample("epsilon");
            crtNode.AddChild(_epsilon);

            //IOCC.Publish<List<TreeNodeClassSample>>("treeNodeTableSample", new List<TreeNodeClassSample>() { root1,  root2, root3 });
            IOCC.Publish<List<TreeNodeClassSample>>("treeNodeTableSample", new List<TreeNodeClassSample>() { root1, root3, root2 });
        }

        private void Focus()
        {
            IOCC.Publish("NodeFocus", _epsilon);
        }
    }
}
