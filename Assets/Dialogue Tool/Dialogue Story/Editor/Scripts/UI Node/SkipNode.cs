using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace E.Story
{
    public class SkipNode : SingleInZeroOutNode
    {
        // 跳转的目标视图
        public StoryDataSO nextStoryDataSO;

        public override void Init(StoryGraphView graphView, string title, Vector2 position)
        {
            base.Init(graphView, title, position);

            // 重设属性默认值
            Type = NodeType.Skip;
        }

        public override void DrawExtensionContainer()
        {
            // 创建自定义容器
            customDataContainer = new VisualElement();
            // 创建折叠框
            foldout = ElementUtility.CreateFoldout("节点内容");
            // 创建故事数据选择字段
            ObjectField objStory = ElementUtility.CreateObjectField(typeof(StoryDataSO), nextStoryDataSO, null, (callback) =>
            {
                nextStoryDataSO = callback.newValue as StoryDataSO;
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
            nodeData.NextStoryDataSO = nextStoryDataSO;

            return nodeData;
        }
    }
}