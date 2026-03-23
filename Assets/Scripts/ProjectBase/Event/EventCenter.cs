using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public interface IEventInfo
{

}

public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        this.actions += action;
    }
}

public class EventInfo : IEventInfo
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        this.actions += action;
    }
}

/// <summary>
/// 事件中心模块
/// </summary>
public class EventCenter : BaseManager<EventCenter>
{
    // key —— 事件名字（比如：怪物死亡，玩家死亡，通过等等）
    // value —— 对应的是 监听这个事件 对应的委托函数们
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="name">事件名字</param>
    /// <param name="action">准备添加的事件委托函数</param>
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        // 如果没有对应的事件
        // 有
        if(eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        // 没有
        else
        {
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }

    /// <summary>
    /// 添加不需要参数的事件
    /// </summary>
    /// <param name="name">事件名字</param>
    /// <param name="action"></param>
    public void AddEventListener(string name, UnityAction action)
    {
        // 有没有对应的事件监听
        // 有
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions += action;
        }
        // 没有
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }
    }

    /// <summary>
    /// 移除事件监听 当某物体移除了调用
    /// </summary>
    /// <param name="name">事件名字</param>
    /// <param name="action">对应之前添加的事件委托函数</param>
    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions -= action;
        }
    }

    /// <summary>
    /// 移除不需要参数的事件
    /// </summary>
    /// <param name="name">事件名字</param>
    /// <param name="action">对应之前添加的事件委托函数</param>
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions -= action;
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="name">第一个事件的名字</param>
    /// <param name="info">事件的参数</param>
    public void EventTrigger<T>(string name, T info)
    {
        if(eventDic.ContainsKey(name))
        {
            if ((eventDic[name] as EventInfo<T>).actions != null)
            {
                (eventDic[name] as EventInfo<T>).actions.Invoke(info);
            }
        }
    }

    /// <summary>
    /// 触发不需要参数的事件
    /// </summary>
    /// <param name="name"></param>
    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            if ((eventDic[name] as EventInfo).actions != null)
            {
                (eventDic[name] as EventInfo).actions.Invoke();
            }
        }
    }

    /// <summary>
    /// 清空事件中心
    /// 需要用在场景切换时
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
}
