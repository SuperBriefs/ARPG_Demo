using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private const string ITEMTOOLTIP_PATH = "ItemTooltip";
    
    public Image image;
    public Color selectedColor, notSelectedColor;

    void Awake()
    {
        Deselect();
    }

    /// <summary>
    /// 选中物品栏时的颜色
    /// </summary>
    public void Select()
    {
        image.color = selectedColor;
    }

    /// <summary>
    /// 没有选中物品栏时的颜色
    /// </summary>
    public void Deselect()
    {
        image.color = notSelectedColor;
    }

    /// <summary>
    /// OnDrop事件,是在拖拽结束并释放鼠标时调用
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        // 如果当前物品栏为空，则将拖拽的物品放入当前物品栏
        if(transform.childCount == 0)
        {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            if(inventoryItem != null)
            {
                inventoryItem.parentAfterDrag = transform;
            }
        }
    }

    /// <summary>
    /// OnPointerClick事件,是在同一对象的按下再松开时候调用
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 右键点击物品栏，将物品丢弃
        if(transform.childCount > 0 && eventData.button == PointerEventData.InputButton.Right)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// OnPointerEnter事件,是在鼠标进入对象时调用,在这里用来显示ItemTooltip
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(transform.childCount > 0)
        {
            // 显示ItemTooltip
            UIManager.GetInstance().ShowPanel<ItemTooltip>(ITEMTOOLTIP_PATH, E_UI_Layer.Top, (itemTooltip) =>
            {
                itemTooltip.SetupTooltip(transform.GetChild(0).GetComponent<InventoryItem>().item);
            });
        }
    }

    /// <summary>
    /// OnPointerExit事件,是在鼠标离开对象时调用,在这里用来隐藏ItemTooltip
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        // 隐藏ItemTooltip
        UIManager.GetInstance().HidePanel(ITEMTOOLTIP_PATH);
    }

    /// <summary>
    /// 背包关闭时，也要隐藏ItemTooltip
    /// </summary>
    void OnDisable()
    {
        // 隐藏ItemTooltip
        UIManager.GetInstance().HidePanel(ITEMTOOLTIP_PATH);
    }
}
