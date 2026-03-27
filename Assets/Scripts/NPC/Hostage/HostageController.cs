using System.Collections.Generic;
using E.Story;
using UnityEngine;

public enum HostageStates { Idle, Saved }

public class HostageController : MonoBehaviour
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private List<EnemyController> enemies;
    [SerializeField] private StoryDataSO currentData;
    [SerializeField] public TipController tipController;

    public StateMachine<HostageController> StateMachine { get; private set; }
    public Dictionary<HostageStates, State<HostageController>> stateDic;

    private Animator animator;

    private bool canTalk = false;
    private bool hasSaved = false;
    private bool hasAddTaskProgress = false;

    public bool NowHasSaved { get => hasSaved; set => hasSaved = value; }

    void Start()
    {
        // 一开始的时候清空对话管理器所有数据
        dialogueController.ResetData();

        EventCenter.GetInstance().AddEventListener<StoryDataSO>("更新对话数据", (nextStory) =>
        {
            if(nextStory != null && canTalk/* 通过当前是否canTalk确定要更新对话数据的对象 */)
            {
                currentData = nextStory;
            }
        });
        EventCenter.GetInstance().AddEventListener("重置对话管理器", () =>
        {
            dialogueController.ResetData();
        });

        animator = GetComponent<Animator>();

        stateDic = new Dictionary<HostageStates, State<HostageController>>();
        stateDic[HostageStates.Idle] = GetComponent<HostageIdleState>();
        stateDic[HostageStates.Saved] = GetComponent<HostageSavedState>();

        StateMachine = new StateMachine<HostageController>(this);
        StateMachine.ChangeState(stateDic[HostageStates.Idle]);
    }

    void Update()
    {
        StateMachine.Execute();
        
        if(canTalk && Input.GetKeyDown(KeyCode.E) && !dialogueController.IsActive())
        {
            // 开启当前对话
            GetNowDialogue();
            dialogueController.StartStory();
        }

        if (!hasAddTaskProgress && HasSaved())
        {
            // 切换动画
            animator.SetBool("HasSaved", true);

            StateMachine.ChangeState(stateDic[HostageStates.Saved]);

            // 任务进度更新
            QuestManager.GetInstance().UpdateQuestProgress("人质", 1);

            hasAddTaskProgress = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (NowHasSaved)
        {
            tipController.Show();
            canTalk = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (NowHasSaved)
        {
            tipController.Hide();
            canTalk = false;
        }
    }

    /// <summary>
    /// 设置当前故事数据
    /// </summary>
    public void GetNowDialogue()
    {
        dialogueController.ChangeCurrentStoryDataSO(currentData);
    }

    /// <summary>
    /// 判断当前是否已经安全
    /// </summary>
    private bool HasSaved()
    {
        foreach(var enemy in enemies)
        {
            if(!enemy.IsInState(EnemyStates.Dead)) return false;
        }

        return true;
    }
}
