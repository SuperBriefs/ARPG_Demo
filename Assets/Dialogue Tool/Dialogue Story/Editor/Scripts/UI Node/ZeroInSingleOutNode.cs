using UnityEngine;

namespace E.Story
{
    public class ZeroInSingleOutNode : BaseNode
    {
        public override void Init(StoryGraphView graphView, string title, Vector2 position)
        {
            base.Init(graphView, title, position);
            
            // 重设属性默认值
            Type = NodeType.ZeroInSingleOut;
        }

        public override void Draw()
        {
            DrawMainContainer();
            DrawTitleContainer();
            DrawTitleButtonContainer();
            DrawTopContainer();
            DrawOutputContainer();
            DrawExtensionContainer();
        }
    }
}