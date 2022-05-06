using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Player : GameActor
{
    [Header("自身组件")]
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D collider2D;
    public Weapon weapon;
    
    [Header("移动/跳跃参数")]
    private float inputX;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private LayerMask groundLayer;

    [Header("Animator参数")] 
    private String speed = "speed",
        jumping = "jumping",
        falling = "falling",
        isIdle = "isIdle",
        isWalk = "isWalk";
    
    
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

    [Header("技能相关")] 
    private bool canSprint, sprintDone;

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
        sprintDone = true;
    }
    
    private void Update()
    {
        GetInput();
        UpdateAnimator();
        playerCommand?.Execute(this);
        if (canSprint)
        {
            Debug.Log("冲刺中");
            rb.velocity = new Vector2(transform.localScale.x * 10 * sprintSpeed, rb.velocity.y);
        }
    }
    private void FixedUpdate()
    {
        Move();
    }
    
    /// <summary>
    /// 获取各种输入
    /// </summary>
    private void GetInput()
    {
        //inputX = Input.GetAxis("Horizontal");
        inputX = Input.GetAxisRaw("Horizontal");
        
    }
    /// <summary>
    /// 更新Animator参数
    /// </summary>
    private void UpdateAnimator()
    {
        animator.SetFloat(speed, Math.Abs(inputX));//输入绝对值作为speed参数
        animator.SetBool(isIdle, inputX==0);//输入为0时处于idle状态
        animator.SetBool(isWalk, inputX>0);//输入不为0时处于walk状态
        animator.SetBool("canSprint", canSprint);
        animator.SetBool("sprintDone", sprintDone);

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
                animator.SetBool(falling, false);//接触到地面时停止下坠
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
        if (inputX != 0 && !canSprint)
        {
            //具体的移动
            rb.velocity = new Vector2(inputX * moveSpeed * Time.fixedDeltaTime, rb.velocity.y);
            //人物转向
            transform.localScale = new Vector3(inputX * 0.1f, 0.1f, 0.1f);
        }
    }

    /// <summary>
    /// 跳跃函数
    /// </summary>
    public override void Jump()
    {
        //处于地面才能跳跃
        if (collider2D.IsTouchingLayers(groundLayer))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool(jumping, true);
        }
    }
    
    public override void Attack()
    {
        animator.Play("knife_attack1");
        Debug.Log("Player进行普通攻击");
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

    
    /// <summary>
    /// 受击函数
    /// </summary>
    /// <param name="position">受伤的坐标</param>
    /// <param name="force">击退力</param>
    /// <param name="damage">受到的伤害</param>
    private void OnGetHurt(Vector2 position,Vector2 force,int damage)
    {
        hp -= damage;
        Debug.Log("Player被攻击");
    }

    /// <summary>
    /// 切换武器
    /// </summary>
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

    /// <summary>
    /// 切换形态
    /// </summary>
    public override void SwitchStatus()
    {
        
    }

    #region 冲刺相关
    public override void Sprint()
    {
        if (sprintDone)
        {
            //sprintDone为true即上一次冲刺结束时才能进行下一次冲刺
            animator.Play("knife_sprint");
            Debug.Log("Player进入冲刺准备状态");
            sprintDone = false;
        }
    }
    /// <summary>
    /// 作为动画事件，冲刺准备结束时调用
    /// </summary>
    public void CanSprint()
    {
        //animator.SetTrigger(canSprint);
        animator.Play("knife_sprint_attack");
        canSprint = true;
    }
    /// <summary>
    /// 动画事件，冲刺动画结束时调用
    /// </summary>
    public void CancelSprint()
    {
        canSprint = false;
        rb.velocity = Vector2.zero;//强行终止冲刺
        Debug.Log("冲刺结束");
    }
    public void SprintDone()
    {
        sprintDone = true;
    }
    
    #endregion
    
}