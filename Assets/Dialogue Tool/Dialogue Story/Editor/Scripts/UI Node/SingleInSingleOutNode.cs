using UnityEngine;

namespace E.Story
{
    // 单进单出节点
    public class SingleInSingleOutNode : BaseNode
    {
        public override void Init(StoryGraphView graphView, string title, Vector2 position)
        {
            base.Init(graphView, title, position);
            
            // 重设属性默认值
            Type = NodeType.SingleInSingleOut;
        }
    }
}