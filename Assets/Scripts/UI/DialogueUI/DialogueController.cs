using System.Collections.Generic;
using UnityEngine;

namespace E.Story
{
    public class DialogueController : MonoBehaviour
    {
        private const string PANEL_NAME = "DialoguePanel";

        [Header("主要组件")]
        [SerializeField] private DialoguePanel dialoguePanel;

        [Header("当前故事")]
        [SerializeField] private StoryDataSO currentStory;
        [SerializeField] private StoryDataSO nextStory;

        [Header("运行时变量")]
        [SerializeField] [ReadOnly] private bool isStoryPlaying = false;
        [SerializeField] [ReadOnly] private NodeData currentNodeData;
        [SerializeField] [ReadOnly] private NodeData nextNodeData;
        [SerializeField] [ReadOnly] private int currentSentenceIndex;
        [SerializeField] [ReadOnly] private List<VarData> varDatas;

        /// <summary>
        /// 开始故事
        /// </summary>
        public void StartStory()
        {
            if (currentStory == null)
            {
                Debug.LogError("未指定故事");
                return;
            }

            NodeData nodeData = currentStory.GetStartNode();
            if (nodeData == null)
            {
                Debug.LogError("开始节点不存在");
                return;
            }

            isStoryPlaying = true;
            UIManager.GetInstance().ShowPanel<DialoguePanel>(PANEL_NAME, E_UI_Layer.System);
            dialoguePanel.HideBranchs();
            
            EventCenter.GetInstance().EventTrigger("锁定玩家");
            EventCenter.GetInstance().EventTrigger("锁定视角");

            DoNode(nodeData);
        }

        /// <summary>
        /// 结束故事
        /// </summary>
        public void EndStory()
        {
            // 重置数据
            ResetData();

            // 隐藏界面
            dialoguePanel.HideBranchs();
            EventCenter.GetInstance().EventTrigger<StoryDataSO>("更新对话数据", nextStory);
            EventCenter.GetInstance().EventTrigger("重置对话管理器");
            EventCenter.GetInstance().EventTrigger("开启玩家");
            EventCenter.GetInstance().EventTrigger("开启视角");
            UIManager.GetInstance().HidePanel(PANEL_NAME);
        }

        /// <summary>
        /// 执行节点
        /// </summary>
        /// <param name="nodeID">节点GUID</param>
        public void DoNode(string nodeID)
        {
            NodeData nodeData = currentStory.GetNode(nodeID);
            DoNode(nodeData);
        }

        /// <summary>
        /// 执行节点
        /// </summary>
        /// <param name="nodeData">节点数据</param>
        public void DoNode(NodeData nodeData)
        {
            if (!isStoryPlaying)
            {
                Debug.LogError("故事未开始");
                return;
            }

            currentNodeData = nodeData;

            if (nodeData == null)
            {
                Debug.LogError("节点不存在");
                return;
            }

            // 根据当前节点类型执行对应的操作
            switch (nodeData.Type)
            {
                case NodeType.Start:
                    Debug.Log($"执行开始节点：{nodeData.Title}\n故事开始");
                    NextStep();
                    break;

                case NodeType.End:
                    Debug.Log($"执行结束节点：{nodeData.Title}\n故事结束");
                    EndStory();
                    break;

                case NodeType.Dialogue:
                    Debug.Log($"执行对话节点：{nodeData.Title}");
                    ShowPortrait(nodeData.Portrait);
                    ShowSentence(0);
                    break;

                case NodeType.Branch:
                    List<ChoiceData> availableChoices =  GetAvailableChoices(nodeData.ChoiceDatas);
                    Debug.Log($"执行分支节点：{nodeData.Title}\n包含有效选项{availableChoices.Count}个");
                    dialoguePanel.ShowBranchs(availableChoices);
                    break;

                case NodeType.GetQuest:
                    Debug.Log($"执行接受任务节点：{nodeData.Title}");
                    // TODO: 任务面板更新
                    NextStep();
                    break;

                case NodeType.CheckQuest:
                    Debug.Log($"执行检测任务节点：{nodeData.Title}");
                    // TODO: 检测任务状态
                    NextStep();
                    break;

                case NodeType.Skip:
                    Debug.Log($"执行跳转节点：{nodeData.Title}");
                    SkipStory(nodeData.NextStoryDataSO);
                    break;

                case NodeType.Layout:
                    Debug.Log($"执行布局节点：{nodeData.Title}");
                    NextStep();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 执行下个步骤
        /// </summary>
        public void NextStep()
        {
            if (!isStoryPlaying)
            {
                Debug.LogWarning("故事未开始");
                return;
            }

            if (currentStory == null)
            {
                Debug.LogError("未指定故事");
                return;
            }

            if (currentNodeData == null)
            {
                Debug.LogError("未指定当前节点");
                return;
            }

            nextNodeData = null;
            string nodeID;

            // 根据当前节点类型执行下一步操作
            switch (currentNodeData.Type)
            {
                // 自动执行
                case NodeType.Start:
                case NodeType.Layout:
                case NodeType.GetQuest:
                case NodeType.CheckQuest:
                    nodeID = currentNodeData.ChoiceDatas[0].NextNodeID;
                    nextNodeData = currentStory.GetNode(nodeID);
                    break;

                // 停止执行
                case NodeType.End:
                    Debug.LogWarning("故事已结束，没有后续节点。");
                    return;

                // 等待执行
                case NodeType.Dialogue:
                    // 检测下个句子索引是否越界
                    int nextSentenceIndex = currentSentenceIndex + 1;
                    if (nextSentenceIndex >= currentNodeData.SentenceDatas.Count)
                    {
                        // 句子展示完毕，执行下个节点
                        nodeID = currentNodeData.ChoiceDatas[0].NextNodeID;
                        nextNodeData = currentStory.GetNode(nodeID);
                        break;
                    }
                    else
                    {
                        // 句子未展示完毕，展示下个句子
                        ShowSentence(nextSentenceIndex);
                        return;
                    }
                case NodeType.Branch:
                    Debug.LogWarning("请选择一个分支选项");
                    return;

                default:
                    break;
            }

            if (nextNodeData == null)
            {
                Debug.LogError("未连接下个节点。如果故事剧情在此结束，请连接至结束节点。");
                return;
            }

            DoNode(nextNodeData);
        }

        /// <summary>
        /// 显示立绘
        /// </summary>
        /// <param name="sprite">立绘</param>
        public void ShowPortrait(Sprite sprite)
        {
            if (sprite != null)
            {
                dialoguePanel.ShowPortrait(sprite);
                Debug.Log($"显示立绘：{sprite.name}");
            }
            else
            {
                dialoguePanel.HidePortrait();
                Debug.Log($"隐藏立绘/立绘未指定");
            }
        }

        /// <summary>
        /// 显示句子
        /// </summary>
        public void ShowSentence(int index)
        {
            NodeData node = currentNodeData;
            currentSentenceIndex = index;

            dialoguePanel.UpdateDialogue(node.RoleName, node.SentenceDatas[index]);
            Debug.Log($"显示对话：{node.RoleName}（第{index + 1}句）\n{node.SentenceDatas[index].Text}");
        }

        /// <summary>
        /// 获取有效选项数据列表
        /// </summary>
        /// <param name="choiceDatas">选项数据列表</param>
        /// <returns>有效选项数据列表</returns>
        private List<ChoiceData> GetAvailableChoices(List<ChoiceData> choiceDatas)
        {
            List<ChoiceData> datas = new List<ChoiceData>();
            foreach (ChoiceData choiceData in choiceDatas)
            {
                datas.Add(choiceData);
            }

            return datas;
        }

        /// <summary>
        /// 检测选项是否有效
        /// </summary>
        /// <param name="choiceData">选项数据</param>
        /// <returns>是否有效</returns>
        private bool IsChoiceAvailable(ChoiceData choiceData)
        {
            // 如果没设置下个节点，跳过此选项
            if (string.IsNullOrEmpty(choiceData.NextNodeID))
                return false;

            // 遍历所有条件
            List<bool> passes = new();
            foreach (ConditionData conditionData in choiceData.Conditions)
            {
                // 检测索引是否越界
                if (conditionData.VarIndex >= varDatas.Count)
                {
                    Debug.LogError($"目标变量索引越界");
                    // 跳过该条件
                    continue;
                }

                // 获取待对比的变量数据
                VarData varData = varDatas[conditionData.VarIndex];
                // 记录节点是否通过
                bool temp = IsConditionPass(conditionData, varData);
                // 记加入列表
                passes.Add(temp);
            }

            switch (choiceData.DetectMode)
            {
                case ConditionDetectMode.满足全部:
                    foreach (bool pass in passes)
                    {
                        // 如果有任意条件不满足
                        if (!pass)
                        {
                            // 将当前选项标记为不可用
                            return false;
                        }
                    }
                    // 将当前选项标记为可用
                    return true;
                case ConditionDetectMode.满足任意:
                    foreach (bool pass in passes)
                    {
                        // 如果有任意条件满足
                        if (pass)
                        {
                            // 将当前选项标记为可用
                            return true;
                        }
                    }
                    // 将当前选项标记为不可用
                    return false;
            }

            // 默认将当前选项标记为可用
            return true;
        }

        /// <summary>
        /// 检测条件是否通过
        /// </summary>
        /// <param name="conditionData">条件数据</param>
        /// <param name="varData">变量数据</param>
        /// <returns>是否通过</returns>
        private bool IsConditionPass(ConditionData conditionData, VarData varData)
        {
            int value = varData.Value;
            int conditionValue = conditionData.Value;

            // 记录对比结果
            bool temp = false;

            // 执行对应操作
            switch (conditionData.Compare)
            {
                case CompareType.大于:
                    temp = value > conditionValue;
                    break;
                case CompareType.大于等于:
                    temp = value >= conditionValue;
                    break;
                case CompareType.小于:
                    temp = value < conditionValue;
                    break;
                case CompareType.小于等于:
                    temp = value <= conditionValue;
                    break;
                case CompareType.等于:
                    temp = value == conditionValue;
                    break;
                case CompareType.不等于:
                    temp = value != conditionValue;
                    break;
                default:
                    break;
            }

            return temp;
        }

        /// <summary>
        /// 跳转到下一个故事
        /// </summary>
        private void SkipStory(StoryDataSO nextStoryDataSO)
        {
            if(nextStoryDataSO != null)
            {
                nextStory = nextStoryDataSO;
            }
            else
            {
                Debug.LogWarning("跳转节点置空了");
            }

            // 结束故事
            EndStory();
        }

        /// <summary>
        /// 外部设置当前故事数据
        /// </summary>
        /// <param name="currentStoryDataSO"></param>
        public void ChangeCurrentStoryDataSO(StoryDataSO currentStoryDataSO)
        {
            currentStory = currentStoryDataSO;
        }

        /// <summary>
        /// 获取当前是否在播放对话
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return isStoryPlaying;
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        public void ResetData()
        {
            isStoryPlaying = false;
            currentNodeData = null;
            nextNodeData = null;
            currentSentenceIndex = 0;
            varDatas = null;
        }
    }
}