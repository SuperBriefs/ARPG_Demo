using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : BasePanel
{
    /// <summary>
    /// 更新背包面板挂载的物品
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetInventoryPanelSlots()
    {
        // 获取所有挂载 InventoryItem 脚本的子物体
        InventoryItem[] items = GetComponentsInChildren<InventoryItem>();
        List<GameObject> inventoryPanelSlots = new List<GameObject>();

        foreach(InventoryItem item in items)
        {
            inventoryPanelSlots.Add(item.gameObject);
        }

        return inventoryPanelSlots;
    }

    /// <summary>
    /// 更新背包面板挂载的物品数据
    /// </summary>
    /// <returns></returns>
    public List<InventoryItem> GetInventoryPanelItems()
    {
        // 获取所有挂载 InventoryItem 脚本的子物体
        InventoryItem[] items = GetComponentsInChildren<InventoryItem>();
        List<InventoryItem> inventoryPanelItems = new List<InventoryItem>();

        foreach(InventoryItem item in items)
        {
            inventoryPanelItems.Add(item);
        }

        return inventoryPanelItems;
    }
}
