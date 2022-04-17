using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scythe : Weapon
{
    private void Awake()
    {
        basicAttackFirst = 20.0f;
        basicAttackSecond = 15.0f;
        basicAttackThird = 25.0f;
        basicAttackDistance = 2.0f;
        
        jumpAttack = 25.0f;
        jumpAttackDistance = 2.0f;
        
        upwardAttack = 25.0f;
        upwardAttackDistance = 2.5f;
        
    }
    
    
}
