using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("普通攻击")] 
    protected float basicAttackFirst;
    protected float basicAttackSecond;
    protected float basicAttackThird;
    protected float basicAttackDistance;
    [Header("跳跃攻击")]
    protected float jumpAttack;
    protected float jumpAttackDistance;
    [Header("向上攻击")]
    protected float upwardAttack;
    protected float upwardAttackDistance;
    
    public virtual void BasicAttack(){}

    public virtual void JumpAttack(){}

    public virtual void UpwardAttack(){}
    
    
    
}
