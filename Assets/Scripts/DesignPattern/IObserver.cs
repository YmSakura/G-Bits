using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 观察者接口，需要作为观察者的类继承此接口
/// </summary>
public interface IObserver
{ 
    public void OnNotify(Entity entity, EventType eventType);
}
