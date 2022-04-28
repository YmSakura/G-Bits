using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable CheckNamespace

public class JaculatoryDagger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Rigidbody2D rb;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Attacked>().OnGetHurt(transform.position,Vector2.zero, 10);
            StartCoroutine(DaggerFade());
        }
        else if(collision.gameObject.CompareTag("Wall"))
        {
            StopCoroutine(DaggerFade());
        }
        
    }
    
    
    /// <summary>
    /// 匕首淡出消失
    /// </summary>
    /// <returns></returns>
    public IEnumerator DaggerFade()
    {
        rb.velocity=Vector2.zero;
        while (sprite.color.a > 0.05)
        {
            var color = sprite.color;
            color = new Color(color.r,color.g,color.b,color.a*0.9f);
            sprite.color = color;
            yield return 0;                                 //每隔一帧运行一次循环调低匕首透明度
        }
        sprite.color = Color.white;      //匕首颜色重置
        ObjectPool.Instance.PushObject(gameObject);  //放回池中
        
    }

}
