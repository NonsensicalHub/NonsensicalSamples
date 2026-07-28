using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using NonsensicalKit.Core;
using NonsensicalKit.DigitalTwin.Warehouse;
using UnityEngine;

public class DemoWarehouseRandom : NonsensicalMono
{
    [SerializeField] private  string CargoStatusKey="cargoStatus";
    [SerializeField] private WarehouseManager m_warehouseManager;

    private void Awake()
    {
        StartCoroutine(SetTestState());
        
        AddHandler<int,Int4[][]>("searchInventoryCargoLocation",SearchInventoryCargoLocation);
    }

    
    private IEnumerator SetTestState()
    {
        while (m_warehouseManager.Inited == false)
        {
            yield return null;
        }

        var i4 = new Int4[10 * 10 * 10];
        var b4 = new bool[10 * 10 * 10];
        int index = 0;
        for (int row = 0; row < 10; row++)
        {
            for (int column = 0; column < 10; column++)
            {
                for (int level = 0; level < 10; level++)
                {
                    i4[index] = new Int4(level, column, row, 0);

                    b4[index] = Random.Range(0, 2) == 1;
                    index++;
                }
            }
        }

        m_warehouseManager.SetCargoState(i4, b4, true);
        IOCC.Set(CargoStatusKey, (i4, b4));
    }

    private Int4[][] SearchInventoryCargoLocation(int arg)
    {
        var a = IOCC.Get<(Int4[], bool[])>("cargoStatus");
        return BuildBatchLocations(a.Item1, a.Item2, arg,0.5f);
    }
    /// <summary>
    /// 随机创建高亮批格位 Int4[]。
    /// </summary>
    private Int4[][] BuildBatchLocations(
        Int4[] locations,
        bool[] cargoStates,
        int batchCount,
        float pickChance)
    {
        var perBatchLocs = new List<Int4>[batchCount];
        for (int b = 0; b < batchCount; b++)
        {
            perBatchLocs[b] = new List<Int4>();
        }

        int cursor = 0;
        for (int i = 0; i < cargoStates.Length; i++)
        {
            if (!cargoStates[i] || Random.value > pickChance)
            {
                continue;
            }

            int batchIndex = cursor % batchCount;
            perBatchLocs[batchIndex].Add(locations[i]);
            cursor++;
        }

        var result = new Int4[batchCount][];
        for (int b = 0; b < batchCount; b++)
        {
            result[b] = perBatchLocs[b].ToArray();
        }

        return result;
    }
}
