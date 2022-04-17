using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : Weapon
{
    private void Awake()
    {
        basicAttackFirst = 10.0f;
        basicAttackSecond = 10.0f;
        basicAttackThird = 15.0f;
        basicAttackDistance = 1.0f;
        
        jumpAttack = 10.0f;
        jumpAttackDistance = 1.0f;
        
        upwardAttack = 10.0f;
        upwardAttackDistance = 1.0f;
        
    }

    public override void BasicAttack()
    {
        
    }

    public override void JumpAttack()
    {
        
    }

    public override void UpwardAttack()
    {
        
    }

    private void Update()
    {
        //Debug.Log("Update:Dagger.basicAttackFirst:" + basicAttackFirst);
    }
}
