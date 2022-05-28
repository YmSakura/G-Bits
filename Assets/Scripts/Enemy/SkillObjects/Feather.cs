using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feather : MonoBehaviour
{
    [SerializeField] private Collider2D area;

    private Collider2D[] _collider2Ds = new Collider2D[1];

    [SerializeField]private ContactFilter2D player;

    [SerializeField]private LayerMask playerLayer;
    // Update is called once per frame
    private void Start()
    {
        player.useLayerMask = true;
        player.layerMask= playerLayer;
    }

    void FixedUpdate()
    {
        
        if (Physics2D.OverlapCollider(area,player,_collider2Ds)!=1) return;
        if (Time.frameCount%5==0)
        {
            _collider2Ds[0].GetComponent<Attacked>().OnGetHurt(_collider2Ds[0].transform.position,Vector2.zero, 4,1);
        }
        
    }
}
