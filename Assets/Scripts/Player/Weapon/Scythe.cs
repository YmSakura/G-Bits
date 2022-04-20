using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scythe : Weapon
{
    private void Awake()
    {
        basicAttackFirst = 20;
        basicAttackSecond = 15;
        basicAttackThird = 25;
        basicAttackDistance = 2;
        
        jumpAttack = 25;
        jumpAttackDistance = 2;
        
        upwardAttack = 25;
        upwardAttackDistance = 2;
        
    }
    
    
}
