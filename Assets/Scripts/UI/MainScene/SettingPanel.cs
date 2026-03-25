using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    private const string Setting_Path = "SettingPanel";
    [SerializeField] private Button closeButton;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;

    void Start()
    {
        // TODO: 初始化音量音效数据
    }

    protected override void OnClick(string btnName)
    {
        if(btnName == closeButton.name)
        {
            UIManager.GetInstance().HidePanel(Setting_Path);
        }
    }
}
