using System.Collections.Generic;
using UnityEngine;

namespace E.Story
{
    public abstract class UIList<T> : MonoBehaviour
    {
        [Tooltip("父对象")][SerializeField] protected Transform parent;
        [Tooltip("UI槽预制体")][SerializeField] protected GameObject prefab;
        [Tooltip("数据列表")][SerializeField] protected List<T> datas = new List<T>();

        public List<T> Datas { get => datas; }

        protected virtual void Awake()
        {
            Refresh();
        }

        /// <summary>
        /// 设置数据列表并自动更新
        /// </summary>
        /// <param name="datas">数据列表</param>
        public void SetData(List<T> datas)
        {
            this.datas = datas;
            Refresh();
        }

        /// <summary>
        /// 刷新数据列表
        /// </summary>
        public virtual void Refresh()
        {
            // 将所有子对象禁用
            for(int i = 0; i < parent.childCount; i++)
            {
                parent.GetChild(i).gameObject.SetActive(false);
            }

            // 检测数据列表是否为空
            if(datas == null)
            {
                return;
            }

            // 遍历设置数据
            foreach(T item in datas)
            {
                GameObject go = GetAvailableSlot();
                UIItem<T> slot = go.GetComponent<UIItem<T>>();
                slot.SetData(item);
            }
        }

        /// <summary>
        /// 获取可用的UI槽对象
        /// </summary>
        /// <returns></returns>
        protected GameObject GetAvailableSlot()
        {
            GameObject go;

            // 遍历子对象列表，获取一个禁用的子对象，启动它并返回
            for(int i = 0; i < parent.childCount; i++)
            {
                // 槽容器是否在整个层级中都处于激活状态
                if (parent.GetChild(i).gameObject.activeInHierarchy)
                {
                    continue;
                }

                go = parent.GetChild(i).gameObject;
                go.SetActive(true);
                return go;
            }

            // 遍历不到可用的对象时，直接新实例化一个
            go = Instantiate(prefab, parent);
            go.SetActive(true);
            return go;
        }
    }
}
