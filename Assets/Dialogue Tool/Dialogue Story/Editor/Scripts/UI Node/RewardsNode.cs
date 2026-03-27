using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace E.Story
{
    // 获取奖励节点
    public class RewardsNode : SingleInSingleOutNode
    {
        // 将要获取的任务数据
        public QuestSO toGetRewards;

        public override void Init(StoryGraphView graphView, string title, Vector2 position)
        {
            base.Init(graphView, title, position);

            Type = NodeType.Rewards;
        }

        public override void DrawExtensionContainer()
        {
            // 创建自定义容器
            customDataContainer = new VisualElement();
            // 创建折叠框
            foldout = ElementUtility.CreateFoldout("节点内容");
            // 创建获取的任务选择字段
            ObjectField objStory = ElementUtility.CreateObjectField(typeof(QuestSO), toGetRewards, null, (callback) =>
            {
                toGetRewards = callback.newValue as QuestSO;
            });

            // 放置UI元素
            foldout.Add(objStory);
            customDataContainer.Add(foldout);
            extensionContainer.Add(customDataContainer);

            // 添加USS类名
            customDataContainer.AddClasses
            (
                "node__custom-data-container"
            );
            objStory.AddClasses
            (
                "foldout-item"
            );

            RefreshExpandedState();
        }

        public override NodeData GetNodeData()
        {
            NodeData nodeData  = base.GetNodeData();
            nodeData.ToGetRewards = toGetRewards;

            return nodeData;
        }
    }
}