using System.Collections.Generic;
using E.Story;
using UnityEngine;
using UnityEngine.UI;

public class QuestNameButton : MonoBehaviour
{
    public Text questNameText;
    public QuestSO currentData;
    public Text questContentText;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(UpdateQuestContent);
    }
    
    /// <summary>
    /// 更新任务描述内容
    /// </summary>
    private void UpdateQuestContent()
    {
        questContentText.text = currentData.description;
        //TODO: 显示任务需求列表
        EventCenter.GetInstance().EventTrigger<QuestSO>("显示任务需求列表", currentData);
        //TODO: 设置任务奖励显示
        EventCenter.GetInstance().EventTrigger<List<Sprite>>("任务奖励显示", currentData.rewards);
    }

    /// <summary>
    /// 让外部设置任务名称按钮显示的任务数据
    /// </summary>
    /// <param name="questDate"></param>
    public void SetupNameButton(QuestSO questDate)
    {
        currentData = questDate;

        if (questDate.isComplete)
        {
            questNameText.text = questDate.questName + "(已完成)";
        }
        else
        {
            questNameText.text = questDate.questName;
        }
    }
}
