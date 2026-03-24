using UnityEngine;
using UnityEngine.UI;
using E.Story;
using System.Collections.Generic;
using UnityEngine.Events;

public class QuestPanel : BasePanel
{
    [Header("主要组件")]
    public GameObject background;

    private bool isOpen;

    [Header("任务名称")]
    public RectTransform questListTransform;
    public QuestNameButton questNameButton;

    [Header("任务信息")]
    public Text questContentText;

    [Header("任务需求")]
    public RectTransform requireTransform;
    public QuestRequirement requirement;

    [Header("奖励面板")]
    public RectTransform rewardTransform;
    // 这里标识奖励的UI元素（根据游戏内容决定）
    public Image rewardUI;

    private UnityAction<QuestSO> showRequireAction;
    private UnityAction<List<Sprite>> showRewardAction;

    void Start()
    {
        questContentText.text = "";
        SetupQuestList();

        showRequireAction = (currentData) =>
        {
            SetupRequireList(currentData);
        };
        showRewardAction = (rewards) =>
        {
            SetRewardItem(rewards);
        };

        EventCenter.GetInstance().AddEventListener("显示任务需求列表", showRequireAction);
        EventCenter.GetInstance().AddEventListener("任务奖励显示", showRewardAction);
    }

    /// <summary>
    /// 显示任务按钮的列表
    /// </summary>
    public void SetupQuestList()
    {
        // 清空任务列表
        foreach(Transform item in questListTransform)
        {
            Destroy(item.gameObject);
        }

        // 清空需求列表
        foreach(Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }

        // 清空任务奖励列表
        foreach(Transform item in rewardTransform)
        {
            Destroy(item.gameObject);
        }

        // 显示任务的按钮列表
        foreach(QuestManager.QuestTask task in QuestManager.GetInstance().tasks)
        {
            QuestNameButton newTask = Instantiate(questNameButton, questListTransform);
            newTask.SetupNameButton(task.questData);
            newTask.questContentText = questContentText;
        }
    }

    /// <summary>
    /// 显示任务需求列表
    /// </summary>
    /// <param name="questData"></param>
    public void SetupRequireList(QuestSO questData)
    {
        // 清空需求列表（实时更新）
        foreach(Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }

        // 显示任务需求列表
        foreach(QuestSO.QuestRequire require in questData.questRequires)
        {
            QuestRequirement q = Instantiate(requirement, requireTransform);
            q.SetupRequirement(require.name, require.requireAmount, require.currentAmount);
        }
    }

    /// <summary>
    /// 设置任务奖励显示
    /// </summary>
    /// <param name="reward"></param>
    public void SetRewardItem(List<Sprite> rewardData)
    {
        // 清空任务奖励列表（实时更新）
        foreach (Transform item in rewardTransform)
        {
            Destroy(item.gameObject);
        }

        // 显示任务奖励列表
        foreach (Sprite reward in rewardData)
        {
            Image item = Instantiate(rewardUI, rewardTransform);
            item.sprite = reward;
        }
    }

    void OnDestroy()
    {
        EventCenter.GetInstance().RemoveEventListener("显示任务需求列表", showRequireAction);
        EventCenter.GetInstance().RemoveEventListener("任务奖励显示", showRewardAction);
    }
}
