using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    private void Awake()
    {
        basicAttackFirst = 12.0f;
        basicAttackSecond = 10.0f;
        basicAttackThird = 15.0f;
        basicAttackDistance = 1.0f;
        
        jumpAttack = 10.0f;
        jumpAttackDistance = 1.0f;
        
        upwardAttack = 10.0f;
        upwardAttackDistance = 1.0f;
        
    }

    private void Update()
    {
        //Debug.Log("Update:Gun.basicAttackFirst" + basicAttackFirst);
    }
}
