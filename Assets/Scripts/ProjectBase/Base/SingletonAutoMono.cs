using UnityEngine;

/// <summary>
/// 继承Mono的单例模式
/// 支持两种模式：
/// 1. 自动创建：如果场景中没有该组件，会自动创建一个GameObject并添加
/// 2. 手动拖拽：如果场景中已有该组件，直接返回现有实例
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T GetInstance()
    {
        if (instance == null)
        {
            // 先尝试在场景中查找是否已存在该组件
            instance = FindFirstObjectByType<T>();

            if (instance == null)
            {
                // 场景中没有，手动创建新的
                GameObject obj = new GameObject();
                // 设置对象的名字为脚本名
                obj.name = typeof(T).ToString();
                // 让这个单例模式对象 过场景 不移除
                // 因为 单例模式对象 往往 是存在整个程序生命周期中的
                DontDestroyOnLoad(obj);
                instance = obj.AddComponent<T>();
            }
        }
        return instance;
    }
}
