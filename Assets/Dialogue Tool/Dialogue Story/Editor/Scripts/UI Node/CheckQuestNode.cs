using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace E.Story
{
    // 检测任务节点
    public class CheckQuestNode : SingleInMultiOutNode
    {
        // 将要获取的任务数据
        public QuestSO toCheckQuest;

        public override void Init(StoryGraphView graphView, string title, Vector2 position)
        {
            base.Init(graphView, title, position);

            Type = NodeType.CheckQuest;

            ChoiceDatas.Clear();
            ChoiceDatas.Add(new ChoiceData("任务完成"));
            ChoiceDatas.Add(new ChoiceData("任务未完成"));
        }

        protected override void DrawOutputContainer()
        {
            // 遍历选项视图列表：创建对应端口
            for(int i = 0; i < ChoiceDatas.Count; i++)
            {
                ChoiceData choiceData = ChoiceDatas[i];
                output = this.CreatePort(choiceData.Text);
                output.userData = choiceData;
                outputContainer.Add(output);
            }
        }

        public override void DrawExtensionContainer()
        {
            // 创建自定义容器
            customDataContainer = new VisualElement();
            // 创建折叠框
            foldout = ElementUtility.CreateFoldout("节点内容");
            // 创建获取的任务选择字段
            ObjectField objCheckQuest = ElementUtility.CreateObjectField(typeof(QuestSO), toCheckQuest, null, (callback) =>
            {
                toCheckQuest = callback.newValue as QuestSO;
            });

            // 放置UI元素
            foldout.Add(objCheckQuest);
            customDataContainer.Add(foldout);
            extensionContainer.Add(customDataContainer);

            // 添加USS类名
            customDataContainer.AddClasses
            (
                "node__custom-data-container"
            );
            objCheckQuest.AddClasses
            (
                "foldout-item"
            );

            RefreshExpandedState();
        }

        public override NodeData GetNodeData()
        {
            NodeData nodeData  = base.GetNodeData();
            nodeData.ToCheckQuest = toCheckQuest;

            return nodeData;
        }
    }
}