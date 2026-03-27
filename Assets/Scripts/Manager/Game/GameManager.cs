using UnityEngine;

public class GameManager : SingletonAutoMono<GameManager>
{
    // 退出面板
    private const string QuitPanel_Path = "QuitPanel";

    void Start()
    {
        // 进入游戏场景 开启背景音乐
        MusicMgr.GetInstance().PlayBKMusic("Night Ambient");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 退出界面如果开启了就直接退出
            if (UIManager.GetInstance().GetPanel<QuitPanel>(QuitPanel_Path))
            {
                return;
            }

            // 显示退出面板
            UIManager.GetInstance().ShowPanel<QuitPanel>(QuitPanel_Path, E_UI_Layer.Top);
            EventCenter.GetInstance().EventTrigger("锁定玩家");
        }
    }
}
