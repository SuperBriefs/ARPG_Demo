using UnityEngine;

namespace E.Story
{
    public class EndNode : SingleInZeroOutNode
    {
        public override void Init(StoryGraphView graphView, string title, Vector2 position)
        {
            base.Init(graphView, title, position);

            // 重设属性默认值
            Type = NodeType.End;
        }
    }
}