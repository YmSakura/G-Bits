using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;



public class DoubleKnives : Enemies
{
    [Header("基本属性")] 
    private int faceDirection = 1;

    [Range(0,5)]
    public float WalkingSpeed = 0.21f;
    
    [Header("组件")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject daggerPrefab;
    [SerializeField] private GameObject[] daggers = new GameObject[3];
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject SprintAttackArea;
    [SerializeField] private GameObject DaggerAttackArea;
    [SerializeField] private GameObject SlashAttackArea;

    [Header("技能属性")]
    private Vector3 patrolAreaPointL;
    private Vector3 patrolAreaPointR;
    private Coroutine CdReset;
    private Skill skill;

    [Header("常量")]
    //private const float WalkingSpeed = 0.21f;             //移动速度
    private const float SprintSpeed = 5f;           //冲刺速度
    private const float DaggerSpeed = 20;           //匕首飞行速度
    private const float DaggerInterval = 0.2f;           //匕首间隔
    private const float DaggerFlyTime = 0.2f;       //匕首飞行时间
    private const float AttackCd = 1f;              //攻击间隙
    private const float ScaleMultiplier = 0.1f;    //缩放比例
    private static readonly int IsWalking = Animator.StringToHash("isWalking");

    private enum Skill
    {
        SSprintAttack,
        SDaggerThrow,
        SSlashAttack
    }
    private void Start()
    {
        BaseInit(); //加载基类脚本初始化
        healthValue = 100;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        SprintAttackArea.GetComponentInChildren<CollisionAttack>().damage = 20;
        DaggerAttackArea.GetComponentInChildren<JaculatoryDagger>().damage = 10;
        SlashAttackArea.GetComponentInChildren<TriggerAttack>().damage = 10;
        SprintAttackArea.GetComponent<BoxCollider2D>().enabled = false;
    }
    
    private void OnEnable()
    {
        healthValue = 100;
        SetPatrolPoint();
    }

    private void FixedUpdate()
    {
        if (!InSkill)
        {
            AlertValueChange();
        }
        if(!inCombat)
        {
            Patrol();
            
        }
        else
        {
            InCombat();
        }
    }

    private void Update()
    {
        if (!inCombat) return;
        
        
    }

    private void OnDestroy()
    {
        gameObject.GetComponent<Attacked>().OnGetHit-=GetHit;
    }


    private void DirectionChange()
    {
        faceDirection = transform.position.x < playerTrans.position.x ? -1 : 1;
        transform.localScale=new Vector3(faceDirection, 1, 1)*ScaleMultiplier;
    }

    
    public void SetPatrolPoint()
    {
        var pos = transform.position;
        var offset = new Vector3(5, 0);
        patrolAreaPointL = pos - offset;
    }
    public void SetPatrolPoint(float left,float right)
    {
        var pos = transform.position;
        patrolAreaPointL = pos - new Vector3(left, 0);
        patrolAreaPointR = pos + new Vector3(right, 0);
    }
    private void Patrol()
    {
        anim.SetBool(IsWalking,true);
        float selfX = transform.position.x;
        float xL = patrolAreaPointL.x;
        float xR = patrolAreaPointR.x;
        if (faceDirection == 1 && selfX < xL || faceDirection == -1 && selfX > xR)
        {
            faceDirection *= -1;
            transform.localScale = new Vector3(faceDirection,1,1)*ScaleMultiplier;
        }

        rb.velocity = Vector2.left*faceDirection*WalkingSpeed;
    }

    #region Combat

    private void InCombat()
    {
        if (!InSkill)               //如果不在使用技能
        {
            DirectionChange();      //可以转向
            if(Attackable)          //如果可以攻击就攻击
                SkillChoose();
            else                    //不可以就追玩家
                ChasePlayer();
        }
        else if (Movable)          //当处于投掷匕首的蓄力阶段可以移动
        {
            DirectionChange();
            ChasePlayer();
        }
        
    }

    private void ChasePlayer()
    {
        anim.SetBool(IsWalking,true);
        rb.velocity = new Vector2(-faceDirection, 0) * WalkingSpeed;
    }


    private void SkillChoose()
    {
        var dis = GetDistance();
        Debug.Log("Distance"+dis);
        int rand = Random.Range(1, 11);
        if (dis > 6*6)
        {
            if (rand<8) 
                skill = Skill.SSprintAttack;
            else 
                Attackable = false;
        }
        else if(dis>3*3)
        {
            Attackable = true;
            skill = rand < 4 ? Skill.SSprintAttack : Skill.SDaggerThrow;
        }
        else
        {
            skill = rand < 8 ? Skill.SDaggerThrow : Skill.SSlashAttack;
        }
        SkillBegin(skill);
    }
    
    private void SkillBegin(Skill skillIndex)
    {
        Debug.Log("技能开始");
        InSkill = true;
        Attackable = false;
        switch (skill)
        {
            case Skill.SSprintAttack:
                rb.velocity=Vector2.zero;
                Movable = false;
                StateLevel = 2;
                anim.Play("charge");
                break;
            case Skill.SDaggerThrow:
                if(CdReset!=null)
                    StopCoroutine(CdReset);
                StateLevel = 1;
                anim.Play("throw_charge");
                break;
            case Skill.SSlashAttack:
                rb.velocity=Vector2.zero;
                Movable = false;
                StateLevel = 1;
                anim.Play("attack_near");
                break;
        }
    }

    private void SkillEnd()
    {
        switch (skill)
        {
            case Skill.SSprintAttack:
                rb.velocity=Vector2.zero;
                break;
            case Skill.SDaggerThrow :
                break;
            case Skill.SSlashAttack:
                break;
        }
        CdReset=StartCoroutine(ResetAttackCd(AttackCd));
        Debug.Log("技能结束");
    }

    protected override void GetInterrupted()
    {
        StateLevel = 10;
        anim.Play("being_attacked");
        
        
    }

    #endregion
    
    #region Charge Sprint Attack

    /// <summary>
    /// 蓄力结束
    /// </summary>
    private void ChargeOver()
    {
        rb.velocity = new Vector2(direction.x*SprintSpeed, 0);//给与当前朝向的速度
    }
    
    #endregion

    #region Dagger Throw Attack

    
    /// <summary>
    /// 动画调用事件函数，生成匕首
    /// </summary>
    private IEnumerator ThrowDagger()
    {
        Vector2 pos = transform.position + new Vector3(0, +0.5f, 0);
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(DaggerGenerate(i,pos));
            pos -= new Vector2(0, 0.6f);
            yield return new WaitForSeconds(DaggerInterval);
        }
    }
    
    /// <summary>
    /// 生成匕首
    /// </summary>
    /// <param name="index">匕首序号</param>
    /// <returns></returns>
    private IEnumerator DaggerGenerate(int index,Vector2 pos)
    { 
        daggers[index]=ObjectPool.Instance.GetObject(daggerPrefab); //从池中取出匕首实例
        daggers[index].transform.position = pos;     //将匕首坐标调整至自身坐标
        daggers[index].GetComponent<SpriteRenderer>().color=Color.white;
        daggers[index].transform.localScale = new Vector3(-faceDirection*0.4f, 0.4f, 1);
        daggers[index].GetComponent<Rigidbody2D>().velocity=new Vector2(faceDirection*-1,0)*DaggerSpeed;//给与匕首均匀速度

        yield return new WaitForSeconds(DaggerFlyTime);//匕首飞行
        
        if(daggers[index])
            StartCoroutine(daggers[index].gameObject.GetComponent<JaculatoryDagger>().DaggerFade());

    }

    #endregion

    #region Slash Attack

    #endregion
    


}
