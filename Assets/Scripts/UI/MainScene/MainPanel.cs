using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    private const string Setting_Path = "SettingPanel";

    [SerializeField] private Button beginButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button quitButton;

    void Start()
    {
        beginButton.onClick.AddListener(() =>
        {
            // 跳转场景
            // 先显示LoadingScene场景
            SceneMgr.GetInstance().LoadSceneAsync(SceneMgr.Scene.LoadingScene.ToString(), () =>
            {
                // LoadingScene加载完成后再显示GameScene
                SceneMgr.GetInstance().LoadSceneAsync(SceneMgr.Scene.GameScene.ToString(), () =>
                {
                    
                });
            });
        });

        settingButton.onClick.AddListener(() =>
        {
            // 显示设置界面
            UIManager.GetInstance().ShowPanel<SettingPanel>(Setting_Path, E_UI_Layer.System);
        });

        quitButton.onClick.AddListener(() =>
        {
            // 退出游戏
            Application.Quit();
        });

        // 进入主菜单时 开启背景音乐
        MusicMgr.GetInstance().PlayBKMusic("Night Ambient");
    }
}
