using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameActor : MonoBehaviour
{
    public virtual void Jump(){}
    public virtual void Attack() {}
    public virtual void SwitchWeapon() {}
    public virtual void Damage(int damage) {}
}
