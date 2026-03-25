using UnityEngine;

/// <summary>
/// 管理通用的事件中心事件
/// </summary>
public class EventManager : SingletonAutoMono<EventManager>
{
    private const string TIP_PATH = "TipPanel";

    void Start()
    {
        EventCenter.GetInstance().AddEventListener("锁定玩家", () =>
        {
            PlayerController.Instance.SetControl(false);
        });
        EventCenter.GetInstance().AddEventListener("开启玩家", () =>
        {
            PlayerController.Instance.SetControl(true);
        });

        EventCenter.GetInstance().AddEventListener("锁定鼠标", () =>
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        });
        EventCenter.GetInstance().AddEventListener("开启鼠标", () =>
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        });

        EventCenter.GetInstance().AddEventListener("玩家死亡", () =>
        {
            // 显示提示面板
            UIManager.GetInstance().ShowPanel<TipPanel>(TIP_PATH, E_UI_Layer.System);
        });
    }

    void Update()
    {
        if (UIManager.GetInstance().HasAnyPanel())
        {
            EventCenter.GetInstance().EventTrigger("开启鼠标");
        }
        else
        {
            EventCenter.GetInstance().EventTrigger("锁定鼠标");
        }
    }
}
