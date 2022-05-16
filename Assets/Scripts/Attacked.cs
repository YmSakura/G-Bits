using System;
using UnityEngine;

public class Attacked: MonoBehaviour
{
    /// <summary>
    /// 受击事件，受攻击者向此事件中添加或移除监听函数以控制事件回调
    /// </summary>
    public event Action<Vector2, Vector2,int> OnGetHit;

    public event Action GetInterrupted; 
    /// <summary>
    /// 攻击者将获取这个组件，并调用这个组件此函数以触发被击者的受击函数
    /// </summary>
    public virtual void OnGetHurt(Vector2 position,Vector2 force,int damage,int priorityLevel)
    {
        OnGetHit?.Invoke(position, force, damage);
    }

    public virtual void OnGetInterrupted()
    {
        GetInterrupted?.Invoke();
    }
    
}
