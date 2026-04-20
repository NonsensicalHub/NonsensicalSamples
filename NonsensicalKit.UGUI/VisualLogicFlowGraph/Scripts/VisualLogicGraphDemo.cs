using NonsensicalKit.UGUI.VisualLogicGraph;
using UnityEngine;

namespace NonsensicalKit.UGUI.Samples.VisualLogicGraph
{
    public class VisualLogicGraphDemo : MonoBehaviour
    {
        [SerializeField] private NonsensicalKit.UGUI.VisualLogicGraph.VisualLogicGraph m_graph;

        private void Awake()
        {
            m_graph.Init((str) => { return new BasicVisualLogicNodeInfo() { Name = "New Node" }; }, (str) => { return new BasicVisualLogicPointInfo(); });
        }

        public void New()
        {
            m_graph.Clear();
            m_graph.AddNewNode("event");
            m_graph.AddNewNode("event");
            m_graph.AddNewNode("event");
        }

        public void Save()
        {
            var data = m_graph.Save<BasicVisualSaveData, BasicVisualLogicNodeInfo, BasicVisualLogicPointInfo>();

            if (data == null)
            {
                Debug.Log("bug");
                return;
            }
            PlayerPrefs.SetString("VisualLogicFlowGraphDemo_Savedata", NonsensicalKit.Tools.JsonTool.SerializeObject(data));
        }

        public void Load()
        {
            string savedataString = PlayerPrefs.GetString("VisualLogicFlowGraphDemo_Savedata", null);
            if (savedataString != null)
            {
                var data = NonsensicalKit.Tools.JsonTool.DeserializeObject<BasicVisualSaveData>(savedataString);
                m_graph.Load<BasicVisualSaveData, BasicVisualLogicNodeInfo, BasicVisualLogicPointInfo>(data);
            }
        }
    }
}
