using System;
using System.Collections.Generic;
using UnityEngine;

namespace E.Story
{
    // 节点数据
    [Serializable]
    public class NodeData
    {
        [SerializeField] private string title;
        [SerializeField] private string guid;
        [SerializeField] private NodeType type;
        [SerializeField] private Vector2 position;

        [SerializeField] private string note;
        [SerializeField] private List<ChoiceData> choiceDatas;
        [SerializeField] private string groupID;

        [SerializeField] private Sprite bgi;
        [SerializeField] private Sprite portrait; //TODO：给对话节点添加玩家立绘
        [SerializeField] private string roleName;
        [SerializeField] private List<SentenceData> sentenceDatas;
        [SerializeField] private List<EditVarData> editVarDatas;

        [SerializeField] private StoryDataSO nextStoryDataSO;

        [SerializeField] private QuestSO toGetQuest;
        [SerializeField] private QuestSO toCheckQuest;


        /// <summary>
        /// 节点标题
        /// </summary>
        public string Title { get => title; set => title = value; }
        
        /// <summary>
        /// 节点GUID
        /// </summary>
        public string GUID { get => guid; set => guid = value; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public NodeType Type { get => type; set => type = value; }

        /// <summary>
        /// 节点坐标
        /// </summary>
        public Vector2 Position { get => position; set => position = value; }

        /// <summary>
        /// 节点文本内容
        /// </summary>
        public string Note { get => note; set => note = value; }

        /// <summary>
        /// 选项视图列表
        /// </summary>
        public List<ChoiceData> ChoiceDatas { get => choiceDatas; set => choiceDatas = value; }

        /// <summary>
        /// 所属分组GUID
        /// </summary>
        public string GroupID { get => groupID; set => groupID = value; }

        /// <summary>
        /// 背景图片
        /// </summary>
        public Sprite BGI { get => bgi; set => bgi = value; }

        /// <summary>
        /// 立绘
        /// </summary>
        public Sprite Portrait { get => portrait; set => portrait = value; }

        /// <summary>
        /// 节点角色名称
        /// </summary>
        public string RoleName { get => roleName; set => roleName = value; }

        /// <summary>
        /// 句子列表
        /// </summary>
        public List<SentenceData> SentenceDatas { get => sentenceDatas; set => sentenceDatas = value; }

        /// <summary>
        /// 编辑变量列表
        /// </summary>
        public List<EditVarData> EditVarDatas { get => editVarDatas; set => editVarDatas = value; }

        /// <summary>
        /// 跳转到的目标视图
        /// </summary>
        public StoryDataSO NextStoryDataSO { get => nextStoryDataSO; set => nextStoryDataSO = value; }

        /// <summary>
        /// 要通过节点获取的任务
        /// </summary>
        public QuestSO ToGetQuest { get => toGetQuest; set => toGetQuest = value; }
        /// <summary>
        /// 要检测是否完成的任务
        /// </summary>
        public QuestSO ToCheckQuest { get => toCheckQuest; set => toCheckQuest = value; }
    }
}