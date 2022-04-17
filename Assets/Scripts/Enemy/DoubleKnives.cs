using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DoubleKnives : Enemies
{
    [Header("基本属性")] 
    private int faceDirection = 1;

    [Header("组件")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject daggerPrefab;
    [SerializeField] private GameObject[] daggers = new GameObject[3];
    [SerializeField] private Animator anim;
    [SerializeField] private Transform dagger;
    [SerializeField] private Transform patrolAreaPointA;
    [SerializeField] private Transform patrolAreaPointB;
    
    [Header("技能属性")]
    private bool inSkill = false;

    private bool moveable = true;
    private bool attackable = true;
    private int daggerIndex = 0;
    


    [Header("常量")]
    private const float SprintSpeed = 5f;          //冲刺速度
    private const float SprintTime = 0.3f;          //冲刺时间
    private const float ChargeTime = 2f;            //蓄力时间
    private const float SprintCd = 3f;              //冲刺技能CD
    private const float DaggerSpeed = 16;           //匕首飞行速度
    private const float DaggerForce = 8f;           //匕首冲击力
    private const float DaggerFlyTime = 0.4f;       //匕首飞行时间
    private const float DaggerThrowInterval = 0.5f; //匕首投掷间隔
    private const float DaggerThrowCd = 3f;         //匕首投掷CD
    private const float AttackCd = 1f;              //攻击间隙
    private const int WalkingSpeed = 1;             //移动速度
    private const float ScaleMultiplier = 0.15f;    //缩放比例
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsCharging = Animator.StringToHash("isCharging");
    private static readonly int IsSprinting = Animator.StringToHash("isSprinting");
    private static readonly int IsSlashing = Animator.StringToHash("isSlashing");
    private static readonly int IsThrowing = Animator.StringToHash("isThrowing");
    
    
    

    private void Start()
    {
        BaseInit();//加载基类脚本初始化
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!inSkill)
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

    private void Patrol()
    {
        anim.SetBool(IsWalking,true);
        if (faceDirection == 1 && transform.position.x < patrolAreaPointA.position.x ||
            faceDirection == -1 && transform.position.x > patrolAreaPointB.position.x)
        {
            faceDirection *= -1;
            transform.localScale = new Vector3(faceDirection*ScaleMultiplier,ScaleMultiplier,1);
        }
        if(faceDirection==1&& transform.position.x > patrolAreaPointA.position.x)//面向左边在左点右方向左走
            rb.velocity=Vector2.left*WalkingSpeed;
        if(faceDirection == -1 && transform.position.x < patrolAreaPointB.position.x)//面向右边在右点左方向左走
            rb.velocity=Vector2.right*WalkingSpeed;
        
    }
    
    private void DirectionChange()
    {
        faceDirection = transform.position.x < playerTrans.position.x ? -1 : 1;
        transform.localScale=new Vector3(faceDirection*ScaleMultiplier, ScaleMultiplier, 1);
    }

    
    private void InCombat()
    {
        if (!inSkill)               //如果不在使用技能
        {
            DirectionChange();      //可以转向
            if(attackable)          //如果可以攻击就攻击
                SkillChoose();
            else                    //不可以就追玩家
                ChasePlayer();
        }
        else if (moveable)          //当处于投掷匕首的蓄力阶段可以移动
        {
            DirectionChange();
            ChasePlayer();
        }
            
    }
    

    private void SkillChoose()
    {
       
        if (Distance > 6)
            Attack("ChargeSprint");
        else if (Distance > 3)
        {
            if (Random.Range(0, 10) < 5)
            {
                Attack("ChargeSprint");
                if (Random.Range(0, 3) < 1)
                {
                    DirectionChange();
                    Attack("DaggerAttack"); 
                }
            }
            else
            {
                Attack("DaggerAttack");
            }
        }
        else
        {
            int rand = Random.Range(0, 10);
            if (rand<1)
            {
                Attack("ChargeSprint");
                if (Random.Range(0, 3) < 1)
                    Attack("DaggerAttack");
            }
            else if (rand < 4)
                Attack("DaggerAttack");
            else
                Attack("SlashAttackBegin");
        }
        
    }
    
    private void ChasePlayer()
    {
        anim.SetBool(IsWalking,true);
        rb.velocity = new Vector2(-faceDirection, 0) * WalkingSpeed;
    }




    private void Attack(String skill)
    {
        switch (skill)
        {
            case "ChargeSprint":
                ChargeSprint();
                break;
            case "DaggerAttack":
                DaggerAttack();
                //while (inSkill){}
                break;
            case "SlashAttackBegin":
                SlashAttackBegin();
                break;

        }
    }
    /// <summary>
    /// 蓄力冲刺调用函数
    /// </summary>
    private void ChargeSprint()
    {
        inSkill = true;                                 //技能使用阶段,禁用其他操作
        attackable = false;
        anim.SetBool(IsCharging,true);
        anim.SetBool(IsSprinting,true);
        anim.Play("charge");                   //播放指定动画
    }
    /// <summary>
    /// 蓄力开始
    /// </summary>
    private void ChargeBegin()
    {
        if (anim.GetBool(IsSprinting))
        {
            rb.velocity=Vector2.zero;
            moveable = false;
        }
    }
    /// <summary>
    /// 蓄力结束
    /// </summary>
    private void ChargeOver()
    {
        anim.SetBool(IsCharging,false);
        if(anim.GetBool(IsSprinting))
            rb.velocity = new Vector2(direction.x*SprintSpeed, 0);//给与当前朝向的速度
        moveable = false;
    }
    /// <summary>
    /// 冲刺结束
    /// </summary>
    private void SprintOver()
    {
        anim.SetBool(IsSprinting,false);
        rb.velocity=Vector2.zero;
        moveable = true;
        StartCoroutine(ResetAttackCd());
    }
    /// <summary>
    /// 匕首投掷攻击调用函数
    /// </summary>
    private void DaggerAttack()
    {
        inSkill = true;                                 //技能使用阶段,禁用其他操作
        attackable = false;
        Debug.Log("进入匕首投掷技能");
        anim.SetBool(IsThrowing,true);
        anim.Play("charge_throw_dagger");
    }
    /// <summary>
    /// 动画调用事件函数，生成匕首
    /// </summary>
    private void ThrowDagger()
    {
        StartCoroutine(DaggerGenerate(daggerIndex));
        daggerIndex++;
        if (daggerIndex == 3) daggerIndex = 0;
    }
    /// <summary>
    /// 生成匕首
    /// </summary>
    /// <param name="index">匕首序号</param>
    /// <returns></returns>
    private IEnumerator DaggerGenerate(int index)
    { 
        daggers[index]=ObjectPool.Instance.GetObject(daggerPrefab); //从池中取出匕首实例
        daggers[index].transform.position = dagger.position;     //将匕首坐标调整至自身坐标
        daggers[index].GetComponent<SpriteRenderer>().color=Color.white;
        daggers[index].transform.localScale = new Vector3(-faceDirection*0.4f, 0.4f, 1);
        Debug.Log("siu");
        
        //daggers[index].GetComponent<Rigidbody2D>().velocity=new Vector2(faceDirection,0)*10f;
        //StartCoroutine(DaggerAccelerate(daggers[index].GetComponent<Rigidbody2D>()));//上面这俩是变加速
        // daggers[index].GetComponent<Rigidbody2D>().AddForce(new Vector2(faceDirection,0)*DaggerForce,ForceMode2D.Impulse);//给与匕首一个短暂的冲击力
        daggers[index].GetComponent<Rigidbody2D>().velocity=new Vector2(faceDirection*-1,0)*DaggerSpeed;//给与匕首均匀速度

        yield return new WaitForSeconds(DaggerFlyTime);//匕首飞行
        
        Debug.Log("stop"+index);
        daggers[index].GetComponent<Rigidbody2D>().velocity=Vector2.zero;//匕首停下
        //daggers[index].GetComponent<Animator>().Play("DaggerFade");
        yield return new WaitForSeconds(0.2f);
        ObjectPool.Instance.PushObject(daggers[index]);
        StartCoroutine(DaggerFade(daggers[index].GetComponent<SpriteRenderer>()));//调用协程匕首淡出并放回池中
    }
    /// <summary>
    /// 匕首投掷结束
    /// </summary>
    private void DaggerThrowEnd()
    {
        anim.SetBool(IsThrowing,false);
        StartCoroutine(ResetAttackCd());
    }
    /// <summary>
    /// 匕首淡出消失
    /// </summary>
    /// <param name="dagger"></param>
    /// <returns></returns>
    private IEnumerator DaggerFade(SpriteRenderer dagger)
    {
        while (dagger.color.a > 0.05)
        {
            var color = dagger.color;
            color = new Color(color.r,color.g,color.b,color.a*0.9f);
            dagger.color = color;
            yield return 0;                                 //每隔一帧运行一次循环调低匕首透明度
        }
        dagger.color = Color.white;      //匕首颜色重置
        ObjectPool.Instance.PushObject(dagger.gameObject);  //放回池中
        
    }

    private IEnumerator DaggerAccelerate(Rigidbody2D dagger)
    {
        while (Math.Abs(dagger.velocity.x) < 64)
        {
            dagger.velocity *= 1.6f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void SlashAttackBegin()
    {
        inSkill = true;                                 //技能使用阶段,禁用其他操作
        moveable = false;
        attackable = false;
        anim.SetBool(IsSlashing,true);
        anim.Play("attack_near");
        
    }

    private void SlashAttackEnd()
    {
        anim.SetBool(IsSlashing,false);
        moveable = true;
        StartCoroutine(ResetAttackCd());
    }
    /// <summary>
    /// 重置攻击CD
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetAttackCd()
    {
        inSkill = false;
        moveable = true;                                //可以继续移动
        yield return new WaitForSeconds(AttackCd);      //等待攻击CD
        attackable = true;                              //CD过后重新可以继续攻击
    }
    

}
