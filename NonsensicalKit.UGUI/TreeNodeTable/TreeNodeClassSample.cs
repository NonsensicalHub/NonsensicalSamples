using System.Collections.Generic;
using NonsensicalKit.UGUI.Table;
using UnityEngine;

namespace NonsensicalKit.UGUI.Samples.Table
{
    public class TreeNodeClassSample : ITreeNodeClass<TreeNodeClassSample>
    {
        public string NodeName { get; set; }

        public bool IsFold { get; set; }
        public bool IsVisible { get; set; }

        public int Level { get; set; }
        public TreeNodeClassSample Parent { get; set; }

        private List<TreeNodeClassSample> _children;

        public TreeNodeClassSample(string NodeName)
        {
            this.NodeName = NodeName;
            this._children = new List<TreeNodeClassSample>();
            IsFold = true;
            IsVisible = true;
        }

        public TreeNodeClassSample(string NodeName, List<TreeNodeClassSample> children)
        {
            this.NodeName = NodeName;
            this._children = children;
            IsFold = true;
            IsVisible = true;
        }

        public List<TreeNodeClassSample> Children { get { return _children; } }

        public GameObject Belong { get; set; }

        public void AddChild(TreeNodeClassSample newNode)
        {
            _children.Add(newNode);
            newNode.Parent = this;
            newNode.Level = Level + 1;
        }

        public void UpdateInfo()
        {
            Queue<TreeNodeClassSample> queues = new Queue<TreeNodeClassSample>();
            queues.Enqueue(this);

            while (queues.Count > 0)
            {
                var v = queues.Dequeue();
                foreach (var item in v._children)
                {
                    item.Parent = v;
                    item.Level = v.Level + 1;
                    queues.Enqueue(item);
                }
            }
        }
    }
}
