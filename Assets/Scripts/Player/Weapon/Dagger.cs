using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : Weapon
{
    public Collider2D knifeL, knifeR;
    
    private void Awake()
    {
        knifeL.enabled = false;
        knifeR.enabled = false;
        
        basicAttackFirst = 10;
        basicAttackSecond = 10;
        basicAttackThird = 15;
        basicAttackDistance = 1;
        
        jumpAttack = 10;
        jumpAttackDistance = 1;
        
        upwardAttack = 10;
        upwardAttackDistance = 1;
        
    }

    /// <summary>
    /// 开启碰撞体，由动画事件调用
    /// </summary>
    public void OpenCollider()
    {
        knifeL.enabled = true;
        knifeR.enabled = true;
    }
    /// <summary>
    /// 关闭碰撞体，由动画事件调用
    /// </summary>
    public void CloseCollider()
    {
        knifeL.enabled = false;
        knifeR.enabled = false;
    }
    /// <summary>
    /// 碰撞检测，攻击到敌人则调用敌人的受击函数
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemies"))
        {
            other.gameObject.GetComponent<Attacked>()
                .OnGetHurt(other.transform.position, Vector2.zero, basicAttackFirst,1);
        }
    }
}
