using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace E.Story
{
    // 导航地图
    public class NavMap : MiniMap
    {
        public void Init()
        {
            anchored = true;
            visible = false;
        }
    }
}