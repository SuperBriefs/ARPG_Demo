using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : BasePanel
{
    public Text ItemNameText;
    public Text ItemInfoText;

    private RectTransform rectTransform;

    protected override void Awake()
    {
        base.Awake();

        rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        // 一开始就更新坐标，避免显示时闪烁
        UpdatePosition();
    }

    protected override void Update()
    {
        base.Update();

        UpdatePosition();
    }

    /// <summary>
    /// 更新当前鼠标UI显示位置
    /// </summary>
    public void UpdatePosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        float width = corners[3].x - corners[0].x;
        float height = corners[1].y - corners[0].y;

        // *0.6f是为了避免鼠标位置和ui显示位置一致，导致ui与鼠标相互遮挡导致闪烁
        if(mousePosition.y < height)
        {
            rectTransform.position = mousePosition + Vector3.up * height * 0.6f;
        }
        else if(Screen.width - mousePosition.x > width)
        {
            rectTransform.position = mousePosition + Vector3.right * width * 0.6f;
        }
        else
        {
            rectTransform.position = mousePosition + Vector3.left * width * 0.6f;
        }
    }

    /// <summary>
    /// 更新面板数据
    /// </summary>
    /// <param name="item"></param>
    public void SetupTooltip(ItemSO item)
    {
        if(item == null)
        {
            Debug.LogWarning("物体为空");
            return;
        }

        ItemNameText.text = item.itemName;
        ItemInfoText.text = item.description;
    }
}
