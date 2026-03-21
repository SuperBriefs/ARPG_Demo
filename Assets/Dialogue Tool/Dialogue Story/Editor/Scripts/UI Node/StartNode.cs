using UnityEngine;

namespace E.Story
{
    public class StartNode : ZeroInSingleOutNode
    {
        public override void Init(StoryGraphView graphView, string title, Vector2 position)
        {
            base.Init(graphView, title, position);

            // 重设属性默认值
            Type = NodeType.Start;
        }
    }
}