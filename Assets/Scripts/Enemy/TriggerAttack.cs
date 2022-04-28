using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于通过trigger检测来给予伤害判定
/// 内涵OnTriggerEnter2D模板来给予玩家伤害
/// 可以通过给委托函数Func(Collider2D col)或协程委托CoroutineFunc(Collider2D col)添加函数来增加需要的效果
/// </summary>
public class TriggerAttack : MonoBehaviour
{
    public int damage;
    public Vector2 force=Vector2.zero;
    
    protected delegate void Func(Collider2D col);
    protected Func Function;
    
    
    private void Start()
    {
        //如果直接挂载此脚本则默认触发DefaultFunc
        //如果使用子类继承，需要用到DefaultFunc则在Start中添加  Function += DefaultFunc;
        //如果子类中需要更改或者不使用DefaultFunc，则无需添加
        Function += DefaultFunc;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        Function(col);
    }

    private void DefaultFunc(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<Attacked>().OnGetHurt(transform.position,force, damage);
        }
    }
}
