using System;
using UnityEngine;

namespace E.Story
{
    //便签数据
    [Serializable]
    public class NoteData
    {
        [SerializeField] private Rect layout;

        [SerializeField] private string title;
        [SerializeField] private string content;

        /// <summary>
        /// 便签布局
        /// </summary>
        public Rect Layout { get => layout; set => layout = value; }
        
        /// <summary>
        /// 便签标题
        /// </summary>
        public string Title { get => title; set => title = value; }

        /// <summary>
        /// 便签内容
        /// </summary>
        public string Content { get => content; set => content = value; }
    }
}
