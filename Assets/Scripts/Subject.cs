using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 被观察者，需要被观察的对象继承此类
/// </summary>
public abstract class Subject : MonoBehaviour
{
    private List<IObserver> observers = new List<IObserver>();

    /// <summary>
    /// 添加观察者
    /// </summary>
    /// <param name="observer"></param>
    public void AddObserver(IObserver observer)
    {
        observers.Add(observer);
    }

    /// <summary>
    /// 移除观察者
    /// </summary>
    /// <param name="observer"></param>
    public void RemoveObserver(IObserver observer)
    {
        observers.Remove(observer);
    }

    /// <summary>
    /// 向观察者发送通知
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="eventType"></param>
    protected void Notify(Entity entity,EventType eventType)
    {
        //遍历列表，通知观察者
        for (int i = 0; i < observers.Count; i++)
        {
            observers[i].OnNotify(entity, eventType);
        }
    }
}
