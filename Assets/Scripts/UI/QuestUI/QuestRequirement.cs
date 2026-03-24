using UnityEngine;
using UnityEngine.UI;

public class QuestRequirement : MonoBehaviour
{
    private Text requireName;
    private Text progressNumber;

    void Awake()
    {
        requireName = GetComponent<Text>();
        progressNumber = transform.GetChild(0).GetComponent<Text>();
    }

    /// <summary>
    /// 给外部设置任务进度的显示
    /// </summary>
    /// <param name="name"></param>
    /// <param name="amount"></param>
    /// <param name="currentAmount"></param>
    public void SetupRequirement(string name, int amount, int currentAmount)
    {
        requireName.text = name;
        progressNumber.text = currentAmount.ToString() + "/" + amount.ToString();
    }
}
