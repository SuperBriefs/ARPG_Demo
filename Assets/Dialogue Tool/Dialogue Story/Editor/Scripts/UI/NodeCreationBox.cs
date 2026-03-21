using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace E.Story
{
    // 节点对话框
    public class NodeCreationBox : ScriptableObject, ISearchWindowProvider
    {
        private StoryGraphView graphViewer;
        private Texture2D indentationIcon;

        // 初始化
        public void Init(StoryGraphView viewer)
        {
            graphViewer = viewer;

            // 设置缩进图标
            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, Color.clear);
            indentationIcon.Apply();
        }

        /// <summary>
        /// 创建搜索树
        /// </summary>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                // 标题
                new SearchTreeGroupEntry(new GUIContent("添加节点")),

                // new SearchTreeEntry(new GUIContent("零进单出", indentationIcon))
                // {
                //     level = 1,
                //     userData = NodeType.ZeroInSingleOut
                // },
                // new SearchTreeEntry(new GUIContent("单进单出", indentationIcon))
                // {
                //     level = 1,
                //     userData = NodeType.SingleInSingleOut
                // },
                // new SearchTreeEntry(new GUIContent("单进多出", indentationIcon))
                // {
                //     level = 1,
                //     userData = NodeType.SingleInMultiOut
                // },
                // new SearchTreeEntry(new GUIContent("单进零出", indentationIcon))
                // {
                //     level = 1,
                //     userData = NodeType.SingleInZeroOut
                // },

                new SearchTreeEntry(new GUIContent("对话", indentationIcon))
                {
                    level = 1,
                    userData = NodeType.Dialogue
                },
                new SearchTreeEntry(new GUIContent("分支", indentationIcon))
                {
                    level = 1,
                    userData = NodeType.Branch
                },
                new SearchTreeEntry(new GUIContent("背景图片", indentationIcon))
                {
                    level = 1,
                    userData = NodeType.BGI
                },
                new SearchTreeEntry(new GUIContent("编辑变量", indentationIcon))
                {
                    level = 1,
                    userData = NodeType.EditVar
                },
                new SearchTreeEntry(new GUIContent("开始", indentationIcon))
                {
                    level = 1,
                    userData = NodeType.Start
                },
                new SearchTreeEntry(new GUIContent("结束", indentationIcon))
                {
                    level = 1,
                    userData = NodeType.End
                },
                new SearchTreeEntry(new GUIContent("跳转", indentationIcon))
                {
                    level = 1,
                    userData = NodeType.Skip
                },
                new SearchTreeEntry(new GUIContent("布局", indentationIcon))
                {
                    level = 1,
                    userData = NodeType.Layout
                },
                new SearchTreeEntry(new GUIContent("接受任务", indentationIcon))
                {
                    level = 1,
                    userData = NodeType.GetQuest
                },
                new SearchTreeEntry(new GUIContent("检测任务", indentationIcon))
                {
                    level = 1,
                    userData = NodeType.CheckQuest
                },
            };

            return searchTreeEntries;
        }

        /// <summary>
        /// 当点击对话框某个按钮时
        /// </summary>
        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            // 获取本地点击坐标
            Vector2 localMousePosition = graphViewer.GetLocalMousePosition(context.screenMousePosition, true);

            // 检测节点类型执行对应操作
            NodeType type = (NodeType)SearchTreeEntry.userData;

            switch (type)
            {
                // case NodeType.ZeroInSingleOut:
                // case NodeType.SingleInSingleOut:
                // case NodeType.SingleInMultiOut:
                // case NodeType.SingleInZeroOut:
                case NodeType.Dialogue:
                case NodeType.Branch:
                case NodeType.BGI:
                case NodeType.EditVar:
                case NodeType.Start:
                case NodeType.End:
                case NodeType.Skip:
                case NodeType.GetQuest:
                case NodeType.CheckQuest:
                case NodeType.Layout:
                    graphViewer.CreateNode(SearchTreeEntry.content.text, type, localMousePosition);
                    return true;
                default:
                    return false;
            }
        }
    }
}