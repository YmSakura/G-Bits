using System;
using System.Collections;
using Cinemachine;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

enum Status { Normal, Sneak }

public class Player : GameActor
{
    [Header("不同的animatorController")] 
    public AnimatorController scytheController;
    public AnimatorController gunController;
    public AnimatorController daggerController;

    [Header("自身组件")] 
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D collider2D;
    public Weapon weapon;

    [Header("移动/跳跃参数")] 
    [SerializeField] private float fallMultiplier = 3.0f;
    [SerializeField] private float lowJumpMultiplier = 1.5f;
    [SerializeField] private LayerMask groundLayer;
    private float inputX;
    private float scaleMultiplier = 0.5f;


    [Header("Animator参数")] private String speed = "speed",
        jumping = "jumping",
        falling = "falling",
        isIdle = "isIdle",
        isWalk = "isWalk";


    [Header("人物属性")] 
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int hp;
    [SerializeField] private float attack = 1f;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float sprintSpeed;
    private float normalMoveSpeed, normalLastAttack;//用于记录普通状态的速度和攻击力
    private bool isAttack, beAttacked;
    private Status status;

    [Header("蓄力设置")] 
    [SerializeField] private float chargeMaxTime = 1.0f;
    [SerializeField] private float chargeMinTime = 0.1f;
    [SerializeField] private float chargeTime; //蓄力时间
    [SerializeField] private bool chargeDone; //蓄力完成
    [SerializeField] private bool charging; //是否在蓄力中
    private bool chargeButtonDown;//是否按下蓄力按键

    [Header("特殊攻击")] 
    private bool isSprinting, lastSprintDone;

    [Header("特殊效果")] 
    private CinemachineImpulseSource myInpulse;
    
    [Header("升级与点数")] 
    [SerializeField] private int level;
    private int basicPoint, skillPoint;
    private int experiencePoint;
    private int levelCount;

    [Header("UI")] 
    public Image healthbarPoint, healthbarEffect;
    private float reduceSpeed = 0.05f;
    
    public ICommand playerCommand;

    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// 人物初始化
    /// </summary>
    private void Init()
    {
        hp = maxHp;
        lastSprintDone = true;//默认上一次冲刺结束
        status = Status.Normal;//初始为默认状态
        //初始化组件
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider2D = GetComponent<Collider2D>();
        weapon = GetComponent<Dagger>(); //默认武器为匕首
        //添加受击函数
        GetComponent<Attacked>().OnGetHit += OnGetHurt;
        //相机抖动脚本
        myInpulse = GetComponent<CinemachineImpulseSource>();
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
        LevelUp();
        Jump();
        Charge();
        playerCommand?.Execute(this);
        if (isSprinting)
        {
            rb.velocity = new Vector2(transform.localScale.x * 10 * sprintSpeed, rb.velocity.y);
            //Debug.Log("冲刺中");
        }
        //测试镜头抖动
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnGetHurt(transform.position, Vector2.zero, 10, 1);
        }
        
    }
    private void FixedUpdate()
    {
        Move();
        if (charging)
        {
            chargeTime += Time.fixedDeltaTime;
            if (chargeTime >= chargeMaxTime && Input.GetKeyUp(KeyCode.U))
            {
                animator.Play("knife_chargeattack");
                animator.speed = 1;
                chargeTime = 0;
                charging = false;
                isAttack = true;
            }
        }
    }

    /// <summary>
    /// 获取各种输入
    /// </summary>
    private void GetInput()
    {
        //inputX = Input.GetAxis("Horizontal");
        inputX = Input.GetAxisRaw("Horizontal");
        chargeButtonDown = Input.GetKey(KeyCode.U);
    }

    /// <summary>
    /// 判断是否升级
    /// </summary>
    private void LevelUp()
    {
        if (experiencePoint >= level + 1 )
        {
            level++;
            levelCount++;
            experiencePoint -= level;
            maxHp += 10;
            hp = maxHp;
            basicPoint++;
            //每升三级额外给予一个基础点
            if (levelCount == 3)
            {
                basicPoint++;
                levelCount = 0;
            }
        }
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
        animator.SetBool("chargeButtonDown", chargeButtonDown);
        animator.SetBool("isAttack", isAttack);

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
    

    /// <summary>
    /// 移动函数，由本脚本获得的输入控制
    /// </summary>
    private void Move()
    {
        Debug.Log("isAttack:" + isAttack);
        Debug.Log("beAttacked" + beAttacked);
        Debug.Log("isSprinting" + isSprinting);
        Debug.Log(inputX);
        if (inputX != 0 && !isSprinting && !isAttack && !beAttacked)
        {
            //具体的移动
            rb.velocity = new Vector2(inputX * moveSpeed * Time.fixedDeltaTime, rb.velocity.y);
            //人物转向
            Debug.Log(inputX);
            transform.localScale = new Vector3(inputX * scaleMultiplier, scaleMultiplier, scaleMultiplier);
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
            if (animator.GetBool(jumping))
            {
                animator.Play("knife_jump_attack");
            }
            else if(animator.GetBool(falling))
            {
                animator.Play("knife_fall_attack");
            }
            else
            {
                animator.Play("knife_attack1");
            }
            
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
                rb.gravityScale = 0.5f;
            }
            else
            {
                animator.Play("gun_attack");
            }
        }
        
        isAttack = true; //别忘了攻击结束将其置为false
        Debug.Log("Player进行普通攻击");
    }

    /// <summary>
    /// 受击函数
    /// </summary>
    /// <param name="position">受伤的坐标</param>
    /// <param name="force">击退力</param>
    /// <param name="damage">受到的伤害</param>
    private void OnGetHurt(Vector2 position, Vector2 force, int damage,int priorityLevel)
    {
        Debug.Log("相机抖动");
        myInpulse.GenerateImpulse(Vector2.down * 10);
        hp -= damage;
        hp = Mathf.Clamp(hp, 0, maxHp);//将Hp数值钳定在一定范围内
        //beAttacked = true; //受击动画播放完将其置为false
        Debug.Log("Player被攻击");
        healthbarPoint.fillAmount = (float)hp / maxHp;
        StartCoroutine(Buffer());//血条缓冲效果
    }
    IEnumerator Buffer()
    {
        while (healthbarEffect.fillAmount > healthbarPoint.fillAmount)
        {
            healthbarEffect.fillAmount -= reduceSpeed;
            yield return new WaitForSeconds(0.05f);
        }
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
            Debug.Log("切换为潜行状态");
            //属性增益
            normalMoveSpeed = moveSpeed;
            normalLastAttack = attack;
            moveSpeed *= 1.5f;
            attack *= 0.5f;
        }
        else
        {
            status = Status.Normal;
            Debug.Log("切换为普通状态");
            moveSpeed = normalMoveSpeed;
            attack = normalLastAttack;
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

    private void OnCollisionEnter(Collision other)
    {
        //冲刺过程中碰到敌人
        if (other.gameObject.CompareTag("Enemies") && isSprinting)
        {
            Debug.Log("冲刺到敌人");
            other.gameObject.GetComponent<Attacked>()
                .OnGetHurt(other.transform.position, Vector2.zero, 10,1);
        }
    }

    #endregion

    /// <summary>
    /// 攻击结束时调用
    /// </summary>
    public void AttackDone()
    {
        isAttack = false;
        rb.gravityScale = 4;
    }

    #region 蓄力

    public void Charge()
    {
        if (chargeButtonDown && !charging)
        {
            animator.Play("knife_charge");
            charging = true;
            Debug.Log("Player进入蓄力状态");
        }

        if (!chargeButtonDown)
        {
            charging = false;
            chargeTime = 0;
        }
    }
    public void ChargeJudge()
    {
        if (chargeButtonDown)
        {
            animator.speed = 0;
        }
    }

    #endregion

    public override void UpAttack()
    {
        if (animator.runtimeAnimatorController == daggerController)
        {
            if (animator.GetBool(jumping))
            {
                animator.Play("knife_jump_attack_upward");
            }
            else if(animator.GetBool(falling))
            {
                animator.Play("knife_fall_attack_upward");
            }
            else
            {
                animator.Play("knife_attack_upward");
            }
        }
        isAttack = true; //别忘了攻击结束将其置为false
        Debug.Log("向上攻击");
    }
}