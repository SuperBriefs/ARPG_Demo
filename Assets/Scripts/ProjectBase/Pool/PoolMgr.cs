using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 缓存池模块
/// </summary>
public class PoolMgr : BaseManager<PoolMgr>
{
    /// 缓存池容器
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    // 根节点
    private GameObject poolObj;

    /// <summary>
    /// 往外拿东西
    /// </summary>
    /// <param name="name">需要的物体的路径</param>
    /// <returns></returns>
    public void GetObj(string name, UnityAction<GameObject> callBack)
    {
        // 有存放该物体的抽屉 且抽屉里还有东西
        if(poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
        {
            callBack(poolDic[name].GetObj());
        }
        else
        {
            // 通过异步加载资源 加载外部物体
            ResourcesMgr.GetInstance().LoadAsync<GameObject>(name, (obj) =>
            {
                obj.name = name;
                callBack(obj);
            });

            //obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            // 把外部物体和缓存池的物体 设置成一样的名字
            //obj.name = name;
        }
    }

    /// <summary>
    /// 归还暂时不用的东西
    /// </summary>
    /// <param name="name">归还物体的路径</param>
    /// <param name="obj">归还物体</param>
    public void PushObj(string name, GameObject obj)
    {
        if (poolObj == null)
        {
            poolObj = new GameObject("Pool");
        }

        // 有存放该物体的抽屉
        if(poolDic.ContainsKey(name))
        {
            poolDic[name].PushObj(obj);
        }
        // 没有存放该物体的抽屉
        else
        {
            poolDic.Add(name, new PoolData(obj, poolObj));
        }
    }

    /// <summary>
    /// 清空缓存池
    /// 需要用在场景切换时
    /// </summary>
    public void Clear()
    {
        poolDic.Clear();
        poolObj = null;
    }
}