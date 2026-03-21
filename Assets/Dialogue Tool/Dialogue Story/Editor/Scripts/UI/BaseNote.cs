using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace E.Story
{
    // 基类便签
    [Serializable]
    public class BaseNote : StickyNote
    {
        public BaseNote(string title, string contents, Rect layout)
        {
            this.title = title;
            this.contents = contents;
            theme = StickyNoteTheme.Classic;
            fontSize = StickyNoteFontSize.Medium;
            SetPosition(layout);
        }

        /// <summary>
        /// 获取便签数据
        /// </summary>
        /// <returns>便签数据</returns>
        public NoteData GetNoteData()
        {
            NoteData noteData = new NoteData()
            {
                Title = title,
                Content = contents,
                Layout = GetPosition()
            };
            return noteData;
        }
    }
}
