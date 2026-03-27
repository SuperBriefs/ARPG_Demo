using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private const string INVENTORYPANEL_PATH = "InventoryPanel";

    [Header("UI")]
    public Image image;
    public Text countText;

    [HideInInspector] public ItemSO item;
    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

    /// <summary>
    /// 初始化背包内的某个物体
    /// </summary>
    /// <param name="newItem"></param>
    public void InitialiseItem(ItemSO newItem)
    {
        item = newItem;
        image.sprite = newItem.image;
        RefreshCount();
    }

    /// <summary>
    /// 更新堆叠的物体数量
    /// </summary>
    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 透过当前图片来判断下方有没有可放置物品的区域
        image.raycastTarget = false;
        countText.raycastTarget = false;
        // 将当前父物体存储下来，如果没有放置到别的插槽里，可以返回原位
        parentAfterDrag = transform.parent;
        // 将物品提升到最顶层，避免被父物体的GridLayoutGroup影响
        if(UIManager.GetInstance().GetPanel<InventoryPanel>(INVENTORYPANEL_PATH) != null)
        {
            transform.SetParent(UIManager.GetInstance().GetPanel<InventoryPanel>(INVENTORYPANEL_PATH).transform.root);
        }
        else
        {
            transform.SetParent(transform.root);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        countText.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }
}
