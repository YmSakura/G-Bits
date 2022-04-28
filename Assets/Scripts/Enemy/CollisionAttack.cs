using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于通过collider的碰撞来给予伤害判定
/// 内涵OnCollisionEnter模板来给予玩家伤害
/// 可以通过给委托函数Func(Collision collision)或协程委托CoroutineFunc(Collision collision)添加函数来增加需要的效果
/// </summary>
public class CollisionAttack : MonoBehaviour
{
    public int damage;
    public Vector2 force=Vector2.zero;
    
    protected delegate void Func(Collision collision);
    protected Func Function;

    private void Start()
    {
        //如果直接挂载此脚本则默认触发DefaultFunc
        //如果使用子类继承，需要用到DefaultFunc则在Start中添加  Function += DefaultFunc;
        //如果子类中需要更改或者不使用DefaultFunc，则无需添加
        Function += DefaultFunc;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Function(collision);
    }

    private void DefaultFunc(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Attacked>().OnGetHurt(transform.position,force, damage);
        }
    }
}
