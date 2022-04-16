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
    private float faceDirection;

    [Header("组件")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject daggerPrefab;
    [SerializeField] private GameObject[] daggers = new GameObject[3];
    [SerializeField] private Animator anim;
    
    [Header("技能属性")]
    private bool inSkill = false;
    private bool chargeSprintEnabled = true;
    private bool daggerThrowEnabled = true;
    private bool attackable = true;
    private int daggerIndex = 0;
    


    [Header("常量")]
    private const float SprintSpeed = 10f;            //冲刺速度
    private const float SprintTime = 0.3f;            //冲刺时间
    private const float ChargeTime = 2f;            //蓄力时间
    private const float SprintCd = 3f;                //冲刺技能CD
    private const float DaggerSpeed = 16;           //匕首飞行速度
    private const float DaggerForce = 8f;           //匕首冲击力
    private const float DaggerFlyTime = 0.1f;       //匕首飞行时间
    private const float DaggerThrowInterval = 0.5f; //匕首投掷间隔
    private const float DaggerThrowCd = 3f;         //匕首投掷CD
    private const float AttackCd = 2f;              //攻击间隙
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
            if (inCombat)
            {
                DirectionChange();
                
                InCombat();
            }


        }
        else
        {
            rb.velocity *= 0.8f;
        }
    }


    private void DirectionChange()
    {
        faceDirection = transform.position.x < playerTrans.position.x ? 1 : -1;
        transform.localScale=new Vector3(faceDirection, 0.15f, 1);
    }
    private void InCombat()
    {
        if (!inSkill)                                   //如果未在使用技能则
        {
            // if (distance > 4 && chargeSprintEnabled)      //如果满足A条件且技能冷却完毕
            //     // StartCoroutine(ChargeSprint());
            // else if (daggerThrowEnabled)                //如果满足B条件且技能冷却完毕
            //     StartCoroutine(DaggerThrowBegin());
        }

    }


    private void CombatAI()
    {
        if (!inSkill && attackable)
        {
            if (distance > 8)
                Attack("ChargeSprint");
            else if (distance > 5)
            {
                if (Random.Range(0, 10) < 5)
                {
                    Attack("ChargeSprint");
                    if (Random.Range(0, 3) < 1)
                        Attack("DaggerThrowBegin");
                    while (inSkill){}
                    
                }
                else
                {
                    Attack("DaggerThrowBegin");
                }
            }
            else if (distance < 5)
            {
                int rand = Random.Range(0, 10);
                if (rand<1)
                {
                    Attack("ChargeSprint");
                    if (Random.Range(0, 3) < 1)
                        Attack("DaggerThrowBegin");
                    while (inSkill){}
                }
                else if (rand < 4)
                    Attack("DaggerThrowBegin");
                else
                    Attack("SlashAttack");
            }

        }
        

    }

    private void Attack(String skill)
    {
        switch (skill)
        {
            case "ChargeSprint":
                ChargeSprint();
                break;
            case "DaggerThrowBegin":
                DaggerThrowBegin();
                while (inSkill) ;
                break;
            case "SlashAttack":
                StartCoroutine(SlashAttack());
                break;

        }
    }
    private void ChargeSprint()
    {
        chargeSprintEnabled = false;                    //技能CD开启,禁用技能
        inSkill = true;                                 //技能使用阶段,禁用其他操作
        anim.Play("charge");                   //播放指定动画
        inSkill = false;                                //技能使用完毕,启用其他操作
        StartCoroutine(ResetAttackCd());
    }
    
    private void DaggerThrowBegin()
    {
        inSkill = true;                                 //技能使用阶段,禁用其他操作
        daggerThrowEnabled = false;                     //技能CD开启,禁用技能
        Debug.Log("进入匕首投掷技能");
        anim.Play("charge");


        inSkill = false;                                //技能使用完毕,启用其他操作
        StartCoroutine(ResetAttackCd());
    }
    private IEnumerator DaggerGenerate(int index)
    { 
        daggers[index]=ObjectPool.Instance.GetObject(daggerPrefab); //从池中取出匕首实例
        daggers[index].transform.position = transform.position;     //将匕首坐标调整至自身坐标
        Debug.Log("siu");
        
        //daggers[index].GetComponent<Rigidbody2D>().velocity=new Vector2(faceDirection,0)*10f;
        //StartCoroutine(DaggerAccelerate(daggers[index].GetComponent<Rigidbody2D>()));//上面这俩是变加速
        // daggers[index].GetComponent<Rigidbody2D>().AddForce(new Vector2(faceDirection,0)*DaggerForce,ForceMode2D.Impulse);//给与匕首一个短暂的冲击力
         daggers[index].GetComponent<Rigidbody2D>().velocity=new Vector2(faceDirection,0)*DaggerSpeed;//给与匕首均匀速度

        yield return new WaitForSeconds(DaggerFlyTime);//匕首飞行
        
        Debug.Log("stop"+index);
        daggers[index].GetComponent<Rigidbody2D>().velocity=Vector2.zero;//匕首停下
        StartCoroutine(DaggerFade(daggers[index].GetComponent<SpriteRenderer>()));//调用协程匕首淡出并放回池中
    }

    private IEnumerator DaggerFade(SpriteRenderer dagger)
    {
        while (dagger.color.a > 0.05)
        {
            var color = dagger.color;
            color = new Color(color.r,color.g,color.b,color.a*0.9f);
            dagger.color = color;
            yield return 0;                                 //每隔一帧运行一次循环调低匕首透明度
        }
        
        ObjectPool.Instance.PushObject(dagger.gameObject);  //放回池中
        dagger.color = new Color(0, 0, 0, 1);       //匕首颜色重置
    }

    private IEnumerator DaggerAccelerate(Rigidbody2D dagger)
    {
        while (Math.Abs(dagger.velocity.x) < 64)
        {
            dagger.velocity *= 1.6f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator SlashAttack()
    {
        inSkill = true;                                 //技能使用阶段,禁用其他操作
        //anim.Play("isCharging")                       //播放蓄力动画
        Debug.Log("蓄力冲刺开始"+Time.time);

        yield return new WaitForSeconds(AttackCd);      //等待攻击CD
        attackable = true;                              //CD过后重新可以继续攻击
    }

    private IEnumerator ResetAttackCd()
    {
        yield return new WaitForSeconds(AttackCd);      //等待攻击CD
        attackable = true;                              //CD过后重新可以继续攻击
    }
    private void ChargeBegin()
    {
        if(anim.GetBool(IsSprinting))
            rb.velocity=Vector2.zero;
        else if (anim.GetBool(IsThrowing))
            anim.speed = 4;
    }

    private void ChargeOver()
    {
        rb.velocity = new Vector2(direction.x*SprintSpeed, 0);//给与当前朝向的速度
        anim.Play("attack_sprint");
    }

    private void SprintOver()
    {
        rb.velocity=Vector2.zero;
        StartCoroutine(ResetAttackCd());
    }

    private void ThrowDagger()
    {
        daggerIndex = daggerIndex++ % 3;
        StartCoroutine(DaggerGenerate(daggerIndex));
    }
    
}
