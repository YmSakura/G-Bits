using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameActor : MonoBehaviour
{
    public virtual void Attack() {}
    public virtual void UpAttack() {}
    public virtual void SwitchWeapon() {}
    public virtual void SwitchStatus() {}
    public virtual void Sprint() {}
}
