using UnityEngine;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    [SerializeField] private Text tipText;
    [SerializeField] private Button quitButton;

    protected override void OnClick(string btnName)
    {
        if(btnName == quitButton.name)
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
    }

    /// <summary>
    /// 修改提示内容
    /// </summary>
    /// <param name="tip"></param>
    public void ChangeTipText(string tip)
    {
        tipText.text = tip;
    }
}
