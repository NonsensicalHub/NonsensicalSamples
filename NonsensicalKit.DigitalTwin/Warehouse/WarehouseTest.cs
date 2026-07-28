    using System.IO;
    using NaughtyAttributes;
    using NonsensicalKit.DigitalTwin.Warehouse;
    using UnityEngine;

public class WarehouseTest : MonoBehaviour
{
    [SerializeField] private string m_dataName = "Test";
    [Button]
    private void Create10x10x10()
    {
        BinDataIO.SaveSync(BinDataIO.CreateTestWarehouse10x10x10 ().Bins,Path.Combine(Application.streamingAssetsPath,"Warehouse",$"{m_dataName}.dat") ); 
    }
    [Button]
    private void Create100x100x100()
    {
        BinDataIO.SaveSync(BinDataIO.CreateTestWarehouse100x100x100 ().Bins,Path.Combine(Application.streamingAssetsPath,"Warehouse",$"{m_dataName}.dat") ); 
    }
}
