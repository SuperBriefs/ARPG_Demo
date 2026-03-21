using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace E.Story
{
    // 基类分组
    public class BaseGroup : Group
    {
        // 分组GUID
        public string ID { get; set; }

        // 旧分组标题
        public string OldTitle { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="position">坐标</param>
        public BaseGroup(string title, Vector2 position)
        {
            // 设置公共属性
            ID = GUID.Generate().ToString();
            this.title = title;
            OldTitle = title;
            SetPosition(new Rect(position, Vector2.zero));

            // 添加 USS 类
            headerContainer.AddToClassList("group__head-container");
        }

        /// <summary>
        /// 获取分组数据
        /// </summary>
        /// <returns></returns>
        public GroupData GetGroupData()
        {
            GroupData groupData = new GroupData()
            {
                GUID = ID,
                Title = title,
                Position = GetPosition().position
            };

            return groupData;
        }
    }
}