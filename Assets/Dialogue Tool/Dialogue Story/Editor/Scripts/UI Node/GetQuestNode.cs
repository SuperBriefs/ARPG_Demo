using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace E.Story
{
    // 获取任务节点
    public class GetQuestNode : SingleInSingleOutNode
    {
        // 将要获取的任务数据
        public QuestSO toGetQuest;

        public override void Init(StoryGraphView graphView, string title, Vector2 position)
        {
            base.Init(graphView, title, position);

            Type = NodeType.GetQuest;
        }

        public override void DrawExtensionContainer()
        {
            // 创建自定义容器
            customDataContainer = new VisualElement();
            // 创建折叠框
            foldout = ElementUtility.CreateFoldout("节点内容");
            // 创建获取的任务选择字段
            ObjectField objGetQuest = ElementUtility.CreateObjectField(typeof(QuestSO), toGetQuest, null, (callback) =>
            {
                toGetQuest = callback.newValue as QuestSO;
            });

            // 放置UI元素
            foldout.Add(objGetQuest);
            customDataContainer.Add(foldout);
            extensionContainer.Add(customDataContainer);

            // 添加USS类名
            customDataContainer.AddClasses
            (
                "node__custom-data-container"
            );
            objGetQuest.AddClasses
            (
                "foldout-item"
            );

            RefreshExpandedState();
        }

        public override NodeData GetNodeData()
        {
            NodeData nodeData  = base.GetNodeData();
            nodeData.ToGetQuest = toGetQuest;

            return nodeData;
        }
    }
}