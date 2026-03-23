using UnityEngine;

namespace E.Story
{
    public enum NodeType
    {
        /// <summary>
        /// 基础
        /// </summary>
        Base = 0,
        /// <summary>
        /// 零进零出
        /// </summary>
        ZeroInZeroOut = 1,
        /// <summary>
        /// 零进单出
        /// </summary>
        ZeroInSingleOut = 2,
        /// <summary>
        /// 零进多出
        /// </summary>
        ZeroInMultiOut = 3,
        /// <summary>
        /// 单进零出
        /// </summary>
        SingleInZeroOut = 4,
        /// <summary>
        /// 单进单出
        /// </summary>
        SingleInSingleOut = 5,
        /// <summary>
        /// 单进多出
        /// </summary>
        SingleInMultiOut = 6,
        /// <summary>
        /// 多进零出
        /// </summary>
        MultiInZeroOut = 7,
        /// <summary>
        /// 多进单出
        /// </summary>
        MultiInSingleOut = 8,
        /// <summary>
        /// 多进多出
        /// </summary>
        MultiInMultiOut = 9,

        /// <summary>
        /// 开始
        /// </summary>
        Start = 21,
        /// <summary>
        /// 结束
        /// </summary>
        End = 31,
        /// <summary>
        /// 对话
        /// </summary>
        Dialogue = 51,
        /// <summary>
        /// 背景图片
        /// </summary>
        BGI = 52,
        /// <summary>
        /// 编辑变量
        /// </summary>
        EditVar = 53,
        /// <summary>
        /// 布局
        /// </summary>
        Layout = 59,
        /// <summary>
        /// 跳转
        /// </summary>
        Skip = 60,
        /// <summary>
        /// 分支
        /// </summary>
        Branch = 61,
        /// <summary>
        /// 获取任务
        /// </summary>
        GetQuest = 71,
        /// <summary>
        /// 判断任务
        /// </summary>
        CheckQuest = 72,
    }
}