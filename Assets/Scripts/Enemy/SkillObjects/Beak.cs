using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beak : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<Attacked>().OnGetHurt(transform.position,Vector2.zero, 20,1);
            
        }
    }
}
