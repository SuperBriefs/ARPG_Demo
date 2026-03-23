using System;
using UnityEngine;

namespace E.Story
{
    // 条件数据
    [Serializable]
    public class ConditionData
    {
        [SerializeField] private int varIndex;
        [SerializeField] private int value;
        [SerializeField] private CompareType compare;

        /// <summary>
        /// 目标变量索引
        /// </summary>
        public int VarIndex { get => varIndex; set => varIndex = value; }

        /// <summary>
        /// 对比值
        /// </summary>
        public int Value { get => value; set => this.value = value; }

        /// <summary>
        /// 对比方式
        /// </summary>
        public CompareType Compare { get => compare; set => compare = value; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="varIndex"></param>
        /// <param name="value"></param>
        /// <param name="compare"></param>
        public ConditionData(int varIndex, int value, CompareType compare)
        {
            this.varIndex = varIndex;
            this.value = value;
            this.compare = compare;
        }
    }

    public enum CompareType
    {
        大于 = 0,
        大于等于 = 1,
        小于 = 2,
        小于等于 = 3,
        等于 = 4,
        不等于 = 5,
    }
}