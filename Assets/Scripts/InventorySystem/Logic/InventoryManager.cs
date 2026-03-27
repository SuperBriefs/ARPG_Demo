using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonAutoMono<InventoryManager>
{
    private const string INVENTORYPANEL_PATH = "InventoryPanel";

    public InventorySlotList toolbar;
    public InventorySlotList inventoryPanel;
    public GameObject inventoryItemPrefab;

    // 当前选中的物品栏
    private int selectedSlot = -1;
    // 记录上一次选择的格子
    private int lastSelectedSlot;

    private bool isOpen = false;

    // 预存背包面板中的数据
    private List<InventoryItem> inventoryPanelItems;
    private List<GameObject> inventoryPanelSlots;

    // 测试
    public ItemSO testItem;

    void Start()
    {
        inventoryPanelItems = new List<InventoryItem>();
        inventoryPanelSlots = new List<GameObject>();
        ChangeSelectedSlot(0);
    }

    void Update()
    {
        // 用键盘来更改选择的物品栏
        if(Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number < 8)
            {
                if(number - 1 == selectedSlot)
                {
                    // 连续选中一个格子将使用其中的物品
                    GetSelectedItem(true);
                }
                ChangeSelectedSlot(number - 1);
                lastSelectedSlot = number - 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!isOpen)
            {
                UIManager.GetInstance().ShowPanel<InventoryPanel>(INVENTORYPANEL_PATH, E_UI_Layer.Mid, (inventoryPanel) =>
                {
                    // 显示背包面板数据
                    ShowInventoryItems(inventoryPanel);
                });
            }
            else
            {
                // 回收数据
                UpdataInventoryItemsData();
                RecoverInventoryItems();
                UIManager.GetInstance().HidePanel(INVENTORYPANEL_PATH);
            }
            isOpen = !isOpen;
        }

        // 测试
        if (Input.GetKeyDown(KeyCode.B))
        {
            AddItem(testItem);

            // 开启背包界面时要实时更新显示面板
            if (UIManager.GetInstance().GetPanel<InventoryPanel>(INVENTORYPANEL_PATH))
            {
                // 先回收 再显示
                RecoverInventoryItems();
                ShowInventoryItems(UIManager.GetInstance().GetPanel<InventoryPanel>(INVENTORYPANEL_PATH));
            }
        }
    }

    /// <summary>
    /// 切换选中的物品栏时，颜色的变化
    /// </summary>
    /// <param name="newValue"></param>
    private void ChangeSelectedSlot(int newValue)
    {
        //先删除之前选中的物品栏颜色
        if (selectedSlot >= 0)
        {
            toolbar.inventorySlots[selectedSlot].Deselect();
        }
        //再更改新物品栏的颜色
        toolbar.inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }

    /// <summary>
    /// 从游戏中获取物品（场景内，商店中）并添加到背包 并且 搜索可以用来添加物体的插槽
    /// </summary>
    /// <param name="item"></param>
    public bool AddItem(ItemSO item)
    {
        // 开启背包时要添加新物品得先更新最新数据 避免在于 toolbar 交换物品后直接更新会报错
        UpdataInventoryItemsData();

        // 检查 toolbar 是否有相同的物品并且可以堆叠
        foreach (InventorySlot slot in toolbar.inventorySlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < item.maxStack && itemInSlot.item.stackable)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
        }
        // 查询 toolbar 所有空闲的插槽
        foreach (InventorySlot slot in toolbar.inventorySlots)
        {
            //如果插槽为空，则将物品添加到插槽中
            if (slot.transform.childCount == 0)
            {
                //产生新物体
                SpawnNewItem(item, slot);
                return true;
            }
        }
        
        // 检查 inventoryPanel 是否有相同的物品并且可以堆叠
        foreach (InventoryItem inventoryItem in inventoryPanelItems)
        {
            if (inventoryItem.item == item && inventoryItem.count < item.maxStack && inventoryItem.item.stackable)
            {
                inventoryItem.count++;
                inventoryItem.RefreshCount();
                return true;
            }
        }
        // 查询 inventoryPanel 所有空闲的插槽
        if(inventoryPanelItems.Count < inventoryPanel.inventorySlots.Count)
        {
            GameObject newItem = Instantiate(inventoryItemPrefab);
            newItem.SetActive(false); // 先隐藏产生的物件
            inventoryPanelSlots.Add(newItem);
            InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
            inventoryItem.InitialiseItem(item);
            inventoryPanelItems.Add(inventoryItem);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 产生新物体
    /// </summary>
    /// <param name="item"></param>
    private void SpawnNewItem(ItemSO item, InventorySlot slot)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
    }

    /// <summary>
    /// 给外部用来使用当前选中的物体
    /// </summary>
    /// <returns></returns>
    public ItemSO GetSelectedItem(bool use)
    {
        InventorySlot slot = toolbar.inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            ItemSO item = itemInSlot.item;
            if(use == true && item.UseItem())
            {
                itemInSlot.count--;
                if(itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
            }

            return item;
        }
        return null;
    }

    /// <summary>
    /// 更新背包面板上的实时数据
    /// </summary>
    public void UpdataInventoryItemsData()
    {
        if (UIManager.GetInstance().GetPanel<InventoryPanel>(INVENTORYPANEL_PATH))
        {
            InventoryPanel nowInventoryPanel = UIManager.GetInstance().GetPanel<InventoryPanel>(INVENTORYPANEL_PATH);
            inventoryPanelItems = nowInventoryPanel.GetInventoryPanelItems();
            inventoryPanelSlots = nowInventoryPanel.GetInventoryPanelSlots();
        }
    }

    /// <summary>
    /// 回收背包面板上的所有物品
    /// </summary>
    public void RecoverInventoryItems()
    {
        if(inventoryPanelItems != null)
        {
            foreach(GameObject slot in inventoryPanelSlots)
            {
                slot.SetActive(false);
                slot.transform.SetParent(null);
            }
        }
    }

    /// <summary>
    /// 显示背包面板上的所有物品
    /// </summary>
    public void ShowInventoryItems(InventoryPanel inventoryPanel)
    {
        if(inventoryPanelItems != null)
        {
            int index = 0;
            List<InventorySlot> inventorySlotList = inventoryPanel.GetComponent<InventorySlotList>().inventorySlots;
            foreach(GameObject slot in inventoryPanelSlots)
            {
                slot.SetActive(true);
                slot.transform.SetParent(inventorySlotList[index++].transform);
            }
        }
    }
}
