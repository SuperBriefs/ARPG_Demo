using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace E.Story
{
    public class BGINode : SingleInSingleOutNode
    {
        // 背景图片
        public Sprite BGI{ get; set; }

        public override void Init(StoryGraphView graphView, string title, Vector2 position)
        {
            base.Init(graphView, title, position);

            // 重设属性默认值
            Type = NodeType.BGI;
        }

        public override void DrawExtensionContainer()
        {
            // 创建自定义容器
            customDataContainer = new VisualElement();
            // 创建折叠框
            foldout = ElementUtility.CreateFoldout("节点内容");
            // 创建角色立绘预览
            Image imgBGI = ElementUtility.CreateImage(BGI);
            // 创建背景图片选择字段
            ObjectField objBGI = ElementUtility.CreateObjectField(typeof(Sprite), BGI, null, (callback) =>
            {
                BGI = callback.newValue as Sprite;
                imgBGI.sprite = BGI;
            });

            // 放置UI元素
            foldout.Add(objBGI);
            foldout.Add(imgBGI);
            customDataContainer.Add(foldout);
            extensionContainer.Add(customDataContainer);

            // 添加USS类名
            customDataContainer.AddClasses
            (
                "node__custom-data-container"
            );
            objBGI.AddClasses
            (
                "foldout-item"
            );
            imgBGI.AddClasses
            (
                "foldout-item",
                "bgi-image"
            );

            RefreshExpandedState();
        }

        public override NodeData GetNodeData()
        {
            NodeData nodeData = base.GetNodeData();
            nodeData.BGI = BGI;

            return nodeData;
        }
    }
}