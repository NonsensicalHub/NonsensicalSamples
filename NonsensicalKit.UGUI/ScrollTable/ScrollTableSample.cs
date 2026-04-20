using NonsensicalKit.Core;
using UnityEngine;

namespace NonsensicalKit.UGUI.Table.Sample
{
    public class ScrollTableSample : MonoBehaviour
    {
        [SerializeField] private ScrollTable m_table;

        public void Init()
        {
            Array2<string> names = new Array2<string>(100, 100);
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    names[i, j] = $"({i},{j})";
                }
            }

            m_table.SetTableData(names);
        }

        public void AddRow()
        {
            m_table.AddRow();
        }

        public void AddColumn()
        {
            m_table.AddColumn();
        }
    }
}
