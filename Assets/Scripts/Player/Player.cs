using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GameActor
{
    [Header("自身组件")]
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D collider2D;
    public Weapon weapon;
    
    
    [Header("移动/跳跃参数")]
    private float faceDirection;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private LayerMask groundLayer;


    [Header("Animator参数")] private String speed = "speed",
        jumping = "jumping",
        falling = "falling",
        isIdle = "isIdle",
        isWalk = "isWalk",
        canSprint = "canSprint";
    
    
    [Header("人物属性")]
    private int maxHp = 100;
    private int hp;
    private int attack = 1;

    [Header("蓄力设置")] 
    [SerializeField] private float chargeMaxTime = 1.0f;
    [SerializeField] private float chargeMinTime = 0.1f;
    [SerializeField] private float chargeTime;//蓄力时间
    [SerializeField] private bool chargeDone;//蓄力完成
    [SerializeField] private bool charging;//是否在蓄力中
    
    public ICommand playerCommand;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider2D = GetComponent<Collider2D>();
        //默认武器为匕首
        weapon = GetComponent<Dagger>();
        //添加受击函数
        GetComponent<Attacked>().OnGetHit += OnGetHurt;
        //初始化人物属性
        hp = maxHp;
    }
    
    private void Update()
    {
        GetInput();
        UpdateCommand();
        UpdateAnimator();
    }
    private void FixedUpdate()
    {
        Move();
    }
    
    /// <summary>
    /// 获取移动所需输入
    /// </summary>
    private void GetInput()
    {
        //inputX = Input.GetAxis("Horizontal");
        faceDirection = Input.GetAxisRaw("Horizontal");
        charging = Input.GetKey(KeyCode.F);
        
    }
    
    private void UpdateCommand()
    {
        playerCommand?.Execute(this);
    }
    /// <summary>
    /// 更新Animator参数
    /// </summary>
    private void UpdateAnimator()
    {
        animator.SetFloat(speed, Math.Abs(faceDirection));
        animator.SetBool(isIdle, faceDirection==0);
        animator.SetBool(isWalk, faceDirection>0);
        
        if (animator.GetBool(jumping))
        {
            if(rb.velocity.y < 0)
            {
                animator.SetBool(jumping, false);
                animator.SetBool(falling, true);
            }
        }
        if (animator.GetBool(falling))
        {
            if(collider2D.IsTouchingLayers(groundLayer))
            {
                animator.SetBool(falling, false);
            }
        }
        
        
    }


    private void isOnGround()
    {
        
    }

    /// <summary>
    /// 移动函数
    /// </summary>
    private void Move()
    {
        if (faceDirection != 0)
        {
            //具体的移动
            rb.velocity = new Vector2(faceDirection * moveSpeed * Time.fixedDeltaTime, rb.velocity.y);
            //人物转向
            transform.localScale = new Vector3(faceDirection * 0.1f, 0.1f, 0.1f);
        }
    }

    /// <summary>
    /// 跳跃函数
    /// </summary>
    public override void Jump()
    {
        if (collider2D.IsTouchingLayers(groundLayer))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool(jumping, true);
        }
    }

    
    public override void Attack()
    {
        animator.Play("knife_attack1");
        Debug.Log("Player普通攻击");
    }

    private void SetCharge(bool charging, float deltaTime)
    {
        if (charging)
        {
            
        }
        else
        {
            
        }
    }

    public void Sprint()
    {
        animator.Play("knife_sprint");
        Debug.Log("Player进入蓄力状态");
    }

    /// <summary>
    /// 作为动画事件，蓄力结束时调用
    /// </summary>
    public void CanSprint()
    {
        animator.SetTrigger(canSprint);
        transform.position += transform.position * sprintSpeed;
    }

    private void OnGetHurt(Vector2 position,Vector2 force,int damage)
    {
        hp -= damage;
        Debug.Log("Player被攻击");
    }

    public override void SwitchWeapon()
    {
        if (weapon is Dagger)
        {
            weapon = GetComponent<Scythe>();
            Debug.Log("武器切换为镰刀");
        }
        else if (weapon is Scythe)
        {
            weapon = GetComponent<Gun>();
            Debug.Log("武器切换为枪");
        }
        else if (weapon is Gun)
        {
            weapon = GetComponent<Dagger>();
            Debug.Log("武器切换为匕首");
        }
    }
    
}