using System;
using UnityEngine;

namespace E.Story
{
    //句子数据
    [Serializable]
    public class SentenceData
    {
        [SerializeField] private string text;

        /// <summary>
        /// 句子文本
        /// </summary>
        public string Text { get => text; set => text = value; }

        /// <summary>
        /// 构造器
        /// </summary>
        public SentenceData(string text)
        {
            this.text = text;
        }
    }
}