using UnityEngine;
using UnityEngine.UI;

public class QuitPanel : BasePanel
{
    private const string QuitPanel_Path = "QuitPanel";

    [SerializeField] private Button sureButton;
    [SerializeField] private Button cancleButton;

    protected override void OnClick(string btnName)
    {
        if(btnName == sureButton.name)
        {
            // 清空所有面板
            UIManager.GetInstance().HideAllPanel();

            // 跳转场景
            SceneMgr.GetInstance().LoadSceneAsync(SceneMgr.Scene.LoadingScene.ToString(), () =>
            {
                SceneMgr.GetInstance().LoadSceneAsync(SceneMgr.Scene.MainScene.ToString(), () =>
                {
                    
                });
            });
        }
        else if(btnName == cancleButton.name)
        {
            // 隐藏面板
            UIManager.GetInstance().HidePanel(QuitPanel_Path);
            EventCenter.GetInstance().EventTrigger("开启玩家");
        }
    }
}
