using System;
using UnityEngine;

namespace E.Story
{
    // 编辑变量数据
    [Serializable]
    public class EditVarData
    {
        [SerializeField] private int varIndex;
        [SerializeField] private int value;
        [SerializeField] private OperationType operation;

        /// <summary>
        /// 目标变量索引
        /// </summary>
        public int VarIndex { get => varIndex; set => varIndex = value; }

        /// <summary>
        /// 编辑值
        /// </summary>
        public int Value { get => value; set => this.value = value; }

        /// <summary>
        /// 编辑方式
        /// </summary>
        public OperationType Operation { get => operation; set => operation = value; }
    }

    public enum OperationType
    {
        加法 = 0,
        乘法 = 1,
        指定 = 2,
    }
}
