using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestP : MonoBehaviour
{
    private Rigidbody2D rb;
    public int speed = 200;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameObject.GetComponent<Attacked>().OnGetHit += GetHit;
    }

    protected void GetHit(Vector2 position, Vector2 force,int damage)
    {
        Debug.Log("玩家受到"+damage+"伤害");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GroundMovement();
    }
    
    //水平移动
    //读取horizontal按键
    //控制Running真假
    //根据运动方向控制任务朝向
    void GroundMovement()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float faceDirection = Input.GetAxisRaw("Horizontal");

        if (horizontalMove != 0)
        {
            rb.velocity = new Vector2(horizontalMove * speed * Time.fixedDeltaTime, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, 0.5f), rb.velocity.y);
        }

        if (faceDirection != 0)
        {
            transform.localScale = new Vector3(faceDirection, 1, 1);
        }

        
    }
}
