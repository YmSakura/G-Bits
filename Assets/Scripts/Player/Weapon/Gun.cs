using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    private void Awake()
    {
        basicAttackFirst = 12;
        basicAttackSecond = 10;
        basicAttackThird = 15;
        basicAttackDistance = 1;
        
        jumpAttack = 10;
        jumpAttackDistance = 1;
        
        upwardAttack = 10;
        upwardAttackDistance = 1;
        
    }

    private void Update()
    {
        //Debug.Log("Update:Gun.basicAttackFirst" + basicAttackFirst);
    }
}
