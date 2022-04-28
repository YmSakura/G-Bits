using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable CheckNamespace

/// <summary>
/// 丢出的匕首
/// </summary>
public class JaculatoryDagger : TriggerAttack
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Rigidbody2D rb;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        Function += Function1;
    }
    
    /// <summary>
    /// 委托函数1
    /// 进行碰撞判定，如果是碰到玩家则给于伤害，如果是碰到墙则消失
    /// </summary>
    /// <param name="col"></param>
    private void Function1(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<Attacked>().OnGetHurt(transform.position,force, damage);
            StartCoroutine(DaggerFade());
        }
        else if(col.gameObject.CompareTag("Wall"))
        {
            StartCoroutine(DaggerFade());
        }
    }
    
    
    /// <summary>
    /// 匕首淡出消失
    /// </summary>
    /// <returns></returns>
    public IEnumerator DaggerFade()
    {
        rb.velocity=Vector2.zero;
        while (sprite.color.a > 0.05)
        {
            var color = sprite.color;
            color = new Color(color.r,color.g,color.b,color.a*0.9f);
            sprite.color = color;
            yield return 0;                                 //每隔一帧运行一次循环调低匕首透明度
        }
        sprite.color = Color.white;      //匕首颜色重置
        ObjectPool.Instance.PushObject(gameObject);  //放回池中
    }

}
