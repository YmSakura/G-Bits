using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEditor.Animations;
using UnityEngine;

enum Status { Normal, Sneak }

public class Player : GameActor
{
    [Header("不同的animator")] public AnimatorController scytheController;
    public AnimatorController gunController;
    public AnimatorController daggerController;

    [Header("自身组件")] private Rigidbody2D rb;
    private Animator animator;
    private Collider2D collider2D;
    public Weapon weapon;
    private Status status;

    [Header("移动/跳跃参数")] [SerializeField] private float fallMultiplier = 3.0f;
    [SerializeField] private float lowJumpMultiplier = 1.5f;
    [SerializeField] private LayerMask groundLayer;
    private float inputX;


    [Header("Animator参数")] private String speed = "speed",
        jumping = "jumping",
        falling = "falling",
        isIdle = "isIdle",
        isWalk = "isWalk";


    [Header("人物属性")] [SerializeField] private int maxHp = 100;
    [SerializeField] private int hp;
    [SerializeField] private float attack = 1f;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float sprintSpeed;
    private float preMoveSpeed, preLastAttack;

    [Header("蓄力设置")] [SerializeField] private float chargeMaxTime = 1.0f;
    [SerializeField] private float chargeMinTime = 0.1f;
    [SerializeField] private float chargeTime; //蓄力时间
    [SerializeField] private bool chargeDone; //蓄力完成
    [SerializeField] private bool charging; //是否在蓄力中

    [Header("特殊攻击")] private bool isSprinting, lastSprintDone;
    private bool isAttack, beAttacked;

    public ICommand playerCommand;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider2D = GetComponent<Collider2D>();
        weapon = GetComponent<Dagger>(); //默认武器为匕首
        //添加受击函数
        GetComponent<Attacked>().OnGetHit += OnGetHurt;
        //初始化人物属性
        hp = maxHp;
        //默认上一次冲刺结束
        lastSprintDone = true;
        //初始为默认状态
        status = Status.Normal;
    }

    /// <summary>
    /// 角色被销毁时移除受击函数
    /// </summary>
    private void OnDestroy()
    {
        GetComponent<Attacked>().OnGetHit -= OnGetHurt;
    }

    private void Update()
    {
        GetInput();
        UpdateAnimator();
        Jump();
        playerCommand?.Execute(this);
        if (isSprinting)
        {
            rb.velocity = new Vector2(transform.localScale.x * 10 * sprintSpeed, rb.velocity.y);
            Debug.Log("冲刺中");
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
    /// 实时更新Animator参数
    /// </summary>
    private void UpdateAnimator()
    {
        animator.SetFloat(speed, Math.Abs(inputX)); //输入绝对值作为speed参数
        animator.SetBool(isIdle, inputX == 0); //输入为0时处于idle状态
        animator.SetBool(isWalk, Math.Abs(inputX) > 0); //输入不为0时处于walk状态
        animator.SetBool("canSprint", isSprinting);
        animator.SetBool("sprintDone", lastSprintDone);

        if (animator.GetBool(jumping))
        {
            //跳跃状态中y轴速度衰减为0时开始下落
            if (rb.velocity.y < 0)
            {
                animator.SetBool(jumping, false);
                animator.SetBool(falling, true);
            }
        }

        if (animator.GetBool(falling))
        {
            //接触到地面时停止下坠
            if (collider2D.IsTouchingLayers(groundLayer))
            {
                animator.SetBool(falling, false);
            }
        }


    }

    private void isOnGround()
    {

    }

    /// <summary>
    /// 移动函数，由本脚本获得的输入控制
    /// </summary>
    private void Move()
    {
        Debug.Log("isAttack:" + isAttack);
        Debug.Log("beAttacked" + beAttacked);
        if (inputX != 0 && !isSprinting && !isAttack && !beAttacked)
        {
            //具体的移动
            rb.velocity = new Vector2(inputX * moveSpeed * Time.fixedDeltaTime, rb.velocity.y);
            //人物转向
            Debug.Log(inputX);
            transform.localScale = new Vector3(inputX * 0.1f, 0.1f, 0.1f);
        }
    }

    /// <summary>
    /// 跳跃，由命令模式调用
    /// </summary>
    private void Jump()
    {
        //处于地面才能跳跃
        if (collider2D.IsTouchingLayers(groundLayer) && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool(jumping, true);
            Debug.Log("Player进行跳跃");
        }

        //通过调整下落时的加速度调整跳跃手感
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    /// <summary>
    /// 普通攻击，由命令模式调用
    /// </summary>
    public override void Attack()
    {
        if (animator.runtimeAnimatorController == daggerController)
        {
            animator.Play("knife_attack1");
        }
        if (animator.runtimeAnimatorController == scytheController)
        {
            animator.Play("sickle_static_attack");
        }
        if (animator.runtimeAnimatorController == gunController)
        {
            if (animator.GetBool(jumping))
            {
                animator.Play("gun_jump_attack");
                rb.gravityScale = 0;
            }
            else
            {
                animator.Play("gun_attack");
            }
        }
        
        isAttack = true; //别忘了攻击结束将其置为false
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
    private void OnGetHurt(Vector2 position, Vector2 force, int damage)
    {
        hp -= damage;
        beAttacked = true; //受击动画播放完将其置为false
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
            animator.runtimeAnimatorController = scytheController;
        }
        else if (weapon is Scythe)
        {
            weapon = GetComponent<Gun>();
            Debug.Log("武器切换为枪");
            animator.runtimeAnimatorController = gunController;
        }
        else if (weapon is Gun)
        {
            weapon = GetComponent<Dagger>();
            Debug.Log("武器切换为匕首");
            animator.runtimeAnimatorController = daggerController;
        }
    }

    /// <summary>
    /// 切换形态
    /// </summary>
    public override void SwitchStatus()
    {
        if (status == Status.Normal)
        {
            status = Status.Sneak;
            preMoveSpeed = moveSpeed;
            preLastAttack = attack;
            moveSpeed *= 1.5f;
            attack *= 0.5f;
        }
        else
        {
            status = Status.Normal;
            moveSpeed = preMoveSpeed;
            attack = preLastAttack;
        }
    }

    #region 冲刺

    public override void Sprint()
    {
        if (lastSprintDone)
        {
            //sprintDone为true即上一次冲刺结束时才能进行下一次冲刺
            animator.Play("knife_sprint");
            isAttack = true; //别忘了在冲刺结束设置为false
            Debug.Log("Player进入冲刺准备状态");
            lastSprintDone = false;
        }
    }

    /// <summary>
    /// 作为动画事件，冲刺准备结束时调用
    /// </summary>
    public void CanSprint()
    {
        //animator.SetTrigger(canSprint);
        animator.Play("knife_sprint_attack");
        isSprinting = true;
    }

    /// <summary>
    /// 动画事件，冲刺动画结束时调用
    /// </summary>
    public void CancelSprint()
    {
        isSprinting = false;
        rb.velocity = Vector2.zero; //强行终止冲刺
        Debug.Log("冲刺结束");
        lastSprintDone = true;
    }

    #endregion

    /// <summary>
    /// 攻击结束时调用
    /// </summary>
    public void AttackDone()
    {
        isAttack = false;
        rb.gravityScale = 3;
    }

}