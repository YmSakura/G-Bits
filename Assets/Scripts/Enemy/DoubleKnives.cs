using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;


enum Skill
{
    SSprintAttack,
    SDaggerThrow,
    SSlashAttack
}
public class DoubleKnives : Enemies ,IObserver
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
    [SerializeField] private Transform patrolAreaPointL;
    [SerializeField] private Transform patrolAreaPointR;
    [SerializeField] private GameObject SprintAttackArea;
    [SerializeField] private GameObject DaggerAttackArea;
    [SerializeField] private GameObject SlashAttackArea;

    [Header("技能属性")]
    private Coroutine CdReset;
    private Skill skill;

    [Header("常量")]
    //private const float WalkingSpeed = 0.21f;             //移动速度
    private const float SprintSpeed = 5f;           //冲刺速度
    private const float DaggerSpeed = 16;           //匕首飞行速度
    private const float DaggerInterval = 0.2f;           //匕首间隔
    private const float DaggerFlyTime = 0.4f;       //匕首飞行时间
    private const float AttackCd = 1f;              //攻击间隙
    private const float ScaleMultiplier = 0.1f;    //缩放比例
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    


    private void Start()
    {
        BaseInit(); //加载基类脚本初始化
        healthValue = 100;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        SprintAttackArea.GetComponentInChildren<TriggerAttack>().damage = 20;
        DaggerAttackArea.GetComponentInChildren<JaculatoryDagger>().damage = 10;
        SlashAttackArea.GetComponentInChildren<TriggerAttack>().damage = 10;
        SprintAttackArea.GetComponent<BoxCollider2D>().enabled = false;
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
    }

    private void Update()
    {
        if (!inCombat) return;
        InCombat();
        
    }

    private void OnDestroy()
    {
        gameObject.GetComponent<Attacked>().OnGetHit-=GetHit;
    }


    private void DirectionChange()
    {
        faceDirection = transform.position.x < playerTrans.position.x ? -1 : 1;
        transform.localScale=new Vector3(faceDirection*ScaleMultiplier, ScaleMultiplier, 1);
    }

    private void Patrol()
    {
        anim.SetBool(IsWalking,true);
        if (faceDirection == 1 && transform.position.x < patrolAreaPointL.position.x ||
            faceDirection == -1 && transform.position.x > patrolAreaPointR.position.x)
        {
            faceDirection *= -1;
            transform.localScale = new Vector3(faceDirection*ScaleMultiplier,ScaleMultiplier,1);
        }
        if(faceDirection==1&& transform.position.x > patrolAreaPointL.position.x)//面向左边在左点右方向左走
            rb.velocity=Vector2.left*WalkingSpeed;
        if(faceDirection == -1 && transform.position.x < patrolAreaPointR.position.x)//面向右边在右点左方向左走
            rb.velocity=Vector2.right*WalkingSpeed;
        
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
        if (Distance > 6)
            skill = Skill.SSprintAttack;
        else if (Distance > 3)
        {
            if (Random.Range(0, 10) < 5)
            {
                skill = Skill.SSprintAttack;
                if (Random.Range(0, 3) < 1)
                {
                    DirectionChange();
                    skill = Skill.SDaggerThrow;
                }
            }
            else
            {
                skill = Skill.SDaggerThrow;
            }
        }
        else
        {
            int rand = Random.Range(0, 10);
            if (rand<1)
            {
                skill = Skill.SSprintAttack;
                if (Random.Range(0, 3) < 1)
                    skill = Skill.SDaggerThrow;
            }
            else if (rand < 4)
                skill = Skill.SDaggerThrow;
            else
                skill = Skill.SSlashAttack;
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
                anim.Play("charge");
                break;
            case Skill.SDaggerThrow:
                if(CdReset!=null)
                    StopCoroutine(CdReset);
                anim.Play("throw_charge");
                break;
            case Skill.SSlashAttack:
                rb.velocity=Vector2.zero;
                Movable = false;
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
        Movable = true;
        CdReset=StartCoroutine(ResetAttackCd(AttackCd));
        Debug.Log("技能结束");
    }

    #endregion
    
    #region Charge Sprint Attack

    /// <summary>
    /// 蓄力结束
    /// </summary>
    private void ChargeOver()
    {
        rb.velocity = new Vector2(direction.x*SprintSpeed, 0);//给与当前朝向的速度
        anim.Play("attack_sprint");
    }
    
    #endregion

    #region Dagger Throw Attack

    
    /// <summary>
    /// 动画调用事件函数，生成匕首
    /// </summary>
    private IEnumerator ThrowDagger()
    {
        Vector2 pos = transform.position + new Vector3(-faceDirection * 5, 2, 0);
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(DaggerGenerate(i,pos));
            pos -= new Vector2(0, 1);
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
    

    public void OnNotify(Entity entity, EventType eventType)
    {
        throw new NotImplementedException();
    }


}
