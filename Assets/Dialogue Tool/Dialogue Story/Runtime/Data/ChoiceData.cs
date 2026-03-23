using System;
using System.Collections.Generic;
using UnityEngine;

namespace E.Story
{
    // 选项数据
    [Serializable]
    public class ChoiceData
    {
        [SerializeField] private string text;
        [SerializeField] private string nextNodeID;
        [SerializeField] private ConditionDetectMode detectMode;
        [SerializeField] private List<ConditionData> conditions;

        /// <summary>
        /// 选项文本
        /// </summary>
        public string Text { get => text; set => text = value; }

        /// <summary>
        /// 节点GUID
        /// </summary>
        public string NextNodeID { get => nextNodeID; set => nextNodeID = value; }

        /// <summary>
        /// 条件检测模式
        /// </summary>
        public ConditionDetectMode DetectMode { get => detectMode; set => detectMode = value; }

        /// <summary>
        /// 条件列表
        /// </summary>
        public List<ConditionData> Conditions { get => conditions; set => conditions = value; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="text"></param>
        public ChoiceData(string text)
        {
            this.text = text;
            conditions = new List<ConditionData>();
        }

        public ChoiceData(string text, string nextNodeID, ConditionDetectMode detectMode, List<ConditionData> conditions)
        {
            this.text = text;
            this.nextNodeID = nextNodeID;
            this.detectMode = detectMode;
            this.conditions = conditions;
        }
    }

    public enum ConditionDetectMode
    {
        满足全部 = 0,
        满足任意 = 1,
    }
}