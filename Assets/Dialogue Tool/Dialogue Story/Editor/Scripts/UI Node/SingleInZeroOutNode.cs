using UnityEngine;

namespace E.Story
{
    // 单进零出节点
    public class SingleInZeroOutNode : BaseNode
    {
        public override void Init(StoryGraphView graphView, string title, Vector2 position)
        {
            base.Init(graphView, title, position);
            
            // 重设属性默认值
            Type = NodeType.SingleInZeroOut;

            // 清除默认选项
            ChoiceDatas.Clear();
        }

        public override void Draw()
        {
            DrawMainContainer();
            DrawTitleContainer();
            DrawTitleButtonContainer();
            DrawTopContainer();
            DrawInputContainer();
            DrawExtensionContainer();
        }
    }
}