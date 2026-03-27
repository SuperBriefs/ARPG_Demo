using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/ItemSO")]
public class ItemSO : ScriptableObject
{
    [Header("UI")]
    // 是否可以堆叠
    public bool stackable = true;
    public int maxStack;

    [Header("通用属性")]
    // 物体名字
    public string itemName;
    // 物体图标
    public Sprite image;
    // 物体描述
    [TextArea]
    public string description;
    // 物体用途
    public ActionType actionType;
    
    /// <summary>
    /// 使用道具
    /// </summary>
    public bool UseItem()
    {
        switch (actionType)
        {
            case ActionType.AddHealth:
                return PlayerController.Instance.AddHealth(5); // 回血：5
        }
        return false;
    }
}

/// <summary>
/// 当前物体的用途
/// </summary>
public enum ActionType
{
    // 回血
    AddHealth,
}
