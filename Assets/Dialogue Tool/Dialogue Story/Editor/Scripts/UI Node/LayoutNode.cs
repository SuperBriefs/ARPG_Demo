using UnityEngine;

namespace E.Story
{
    // 布局节点
    public class LayoutNode : SingleInSingleOutNode
    {
        public override void Init(StoryGraphView graphView, string title, Vector2 position)
        {
            base.Init(graphView, title, position);

            // 重设属性默认值
            Type = NodeType.Layout;

            // 添加USS类
            mainContainer.RemoveFromClassList("node__main-container");
            mainContainer.AddToClassList("node__main-container-small");
        }

        public override void Draw()
        {
            DrawMainContainer();
            DrawTitleContainer();
            DrawTitleButtonContainer();
            DrawTopContainer();
            DrawInputContainer();
            DrawOutputContainer();
        }
    }
}