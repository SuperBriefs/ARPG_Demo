using System;
using UnityEngine;

namespace E.Story
{
    // 分组数据
    [Serializable]
    public class GroupData
    {
        [SerializeField] private string title;
        [SerializeField] private string guid;
        [SerializeField] private Vector2 position;

        /// <summary>
        /// 分组标题
        /// </summary>
        public string Title { get => title; set => title = value; }

        /// <summary>
        /// 分组GUID
        /// </summary>
        public string GUID { get => guid; set => guid = value; }

        /// <summary>
        /// 分组坐标
        /// </summary>
        public Vector2 Position { get => position; set => position = value; }
    }
}