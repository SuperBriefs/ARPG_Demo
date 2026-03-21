using System.Collections.Generic;
using UnityEngine;

namespace E.Story
{
    // 故事数据
    public class StoryDataSO : ScriptableObject
    {
        [SerializeField] private string fileName;
        [SerializeField] private List<GroupData> groupDatas;
        [SerializeField] private List<NodeData> nodeDatas;
        [SerializeField] private List<VarData> varDatas;
        [SerializeField] private List<NoteData> noteDatas;

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get => fileName; set => fileName = value; }

        /// <summary>
        /// 分组数据列表
        /// </summary>
        public List<GroupData> GroupDatas { get => groupDatas; set => groupDatas = value; }

        /// <summary>
        /// 节点数据列表
        /// </summary>
        public List<NodeData> NodeDatas { get => nodeDatas; set => nodeDatas = value; }

        /// <summary>
        /// 变量数据列表
        /// </summary>
        public List<VarData> VarDatas { get => varDatas; set => varDatas = value; }
        
        /// <summary>
        /// 标签数据列表
        /// </summary>
        public List<NoteData> NoteDatas { get => noteDatas ; set => NoteDatas = value; }
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(string fileName)
        {
            this.fileName = fileName;
            groupDatas = new List<GroupData>();
            nodeDatas = new List<NodeData>();
            VarDatas = new List<VarData>();
            noteDatas = new List<NoteData>();
        }

        /// <summary>
        /// 获取开始节点
        /// </summary>
        /// <returns>开始节点</returns>
        public NodeData GetStartNode()
        {
            foreach(NodeData nodeData in nodeDatas)
            {
                if(nodeData.Type == NodeType.Start)
                {
                    return nodeData;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取节点（根据节点的GUID）
        /// </summary>
        /// <param name="nodeID">节点的GUID</param>
        /// <returns></returns>
        public NodeData GetNode(string nodeID)
        {
            foreach(NodeData nodeData in nodeDatas)
            {
                if(nodeData.GUID == nodeID)
                {
                    return nodeData;
                }
            }
            return null;
        }
    }
}