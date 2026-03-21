using UnityEngine;

namespace E.Story
{
    // UI槽模板类
    public abstract class UIItem<T> : MonoBehaviour
    {
        [Tooltip("数据")][SerializeField] private T data;

        public T Data { get => data; }
        
        /// <summary>
        /// 设置数据并自动更新
        /// </summary>
        /// <param name="data">数据</param>
        public void SetData(T data)
        {
            this.data = data;
            Refresh();
        }

        /// <summary>
        /// 更新
        /// </summary>
        public abstract void Refresh();
    }
}
