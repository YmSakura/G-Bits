using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSpear : CollisionAttack
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Rigidbody2D rb;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        Function += DefaultFunc;
    }

    // private void Function1(Collision2D col)
    // {
    //     if (col.gameObject.CompareTag("Player"))
    //     {
    //         col.gameObject.GetComponent<Attacked>().OnGetHurt(transform.position,Vector2.zero, 20);
    //         StartCoroutine(BloodSpearFade());
    //     }
    //     else if(col.gameObject.CompareTag("Wall"))
    //     {
    //         StopCoroutine(BloodSpearFade());
    //     }
    //     
    // }
    

    /// <summary>
    /// 血球淡出消失
    /// </summary>
    /// <returns></returns>
    public IEnumerator BloodSpearFade()
    {
        rb.velocity=Vector2.zero;
        while (sprite.color.a > 0.05)
        {
            var color = sprite.color;
            color = new Color(color.r,color.g,color.b,color.a*0.7f);
            sprite.color = color;
            yield return 0;                                 //每隔一帧运行一次循环调低血球透明度
        }
        sprite.color = Color.white;      //血球颜色重置
        ObjectPool.Instance.PushObject(gameObject);  //放回池中
        
    }
}
