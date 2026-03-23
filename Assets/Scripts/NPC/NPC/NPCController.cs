using E.Story;
using UnityEngine;

public enum NPCStates { StartTask, ProgressTask, CompleteTask }

public class NPCController : MonoBehaviour
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private StoryDataSO currentStoryDataSO;
    [SerializeField] private TipController tipController;

    private bool canTalk = false;

    void Start()
    {
        // 一开始的时候清空对话管理器所有数据
        dialogueController.ResetData();

        EventCenter.GetInstance().AddEventListener<StoryDataSO>("更新对话数据", (nextStory) =>
        {
            if(nextStory != null)
            {
                currentStoryDataSO = nextStory;
            }
        });
        EventCenter.GetInstance().AddEventListener("重置对话管理器", () =>
        {
            dialogueController.ResetData();
        });
        EventCenter.GetInstance().AddEventListener("锁定玩家", () =>
        {
            PlayerController.Instance.SetControl(false);
        });
        EventCenter.GetInstance().AddEventListener("开启玩家", () =>
        {
            PlayerController.Instance.SetControl(true);
        });
    }

    void Update()
    {
        Vector3 direction = PlayerController.Instance.transform.position - transform.position;
        direction.y = 0;

        //设置旋转
        transform.rotation = Quaternion.LookRotation(direction);

        if(canTalk && Input.GetKeyDown(KeyCode.E) && !dialogueController.IsActive())
        {
            // 开启鼠标
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // 开启当前对话
            GetNowDialogue();
            dialogueController.StartStory();
        }
        else if(dialogueController.IsActive())
        {
            // 开启鼠标
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // 关闭鼠标
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void OnTriggerStay(Collider other)
    {
        //TODO: 显示可以进行对话的UI
        tipController.Show();
        canTalk = true;
    }

    void OnTriggerExit(Collider other)
    {
        //TODO: 隐藏可以进行对话的UI
        tipController.Hide();
        canTalk = false;
    }

    /// <summary>
    /// 设置当前故事数据
    /// </summary>
    public void GetNowDialogue()
    {
        dialogueController.ChangeCurrentStoryDataSO(currentStoryDataSO);
    }
}
