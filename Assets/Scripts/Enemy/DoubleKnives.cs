using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class DoubleKnives : Enemies
{
    [Header("基本属性")] 
    //private float dirction;

    [Header("组件")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject daggerPrefab;
    [SerializeField] private GameObject[] daggers = new GameObject[3];
    
    [Header("技能属性")]
    private bool inSkill = false;
    private bool chargeDashEnabled = true;
    private bool daggerThrowEnabled = true;
    
    
    [Header("常量")]
    private const float DashSpeed = 10f;           //冲刺速度
    private const float DashTime = 0.3f;            //冲刺时间
    private const float ChargeTime = 2f;            //蓄力时间
    private const float DashCd = 3f;                //冲刺技能CD
    private const float DaggerSpeed = 16;           //匕首飞行速度
    private const float DaggerForce = 8f;           //匕首冲击力
    private const float DaggerFlyTime = 0.1f;       //匕首飞行时间
    private const float DaggerThrowInterval = 0.5f; //匕首投掷间隔
    private const float DaggerThrowCd = 3f;         //匕首投掷CD
    

    private void Start()
    {
        BaseInit();//加载基类脚本初始化
    }

    private void FixedUpdate()
    {
        if (!inSkill)
        {
            AlertValueChange();
            if (inCombat)
            {
                transform.localScale = new Vector3(transform.position.x < playerTrans.position.x ? 1 : -1, 1, 0);
                InCombat();
            }


        }
        else
        {
            rb.velocity *= 0.8f;
        }
    }

    private void InCombat()
    {
        if (!inSkill)                                   //如果未在使用技能则
        {
            if (distance > 4 && chargeDashEnabled)      //如果满足A条件且技能冷却完毕
                StartCoroutine(ChargeDash());
            else if (daggerThrowEnabled)                //如果满足B条件且技能冷却完毕
                StartCoroutine(DaggerThrow());
        }
        
    }

    
    private IEnumerator ChargeDash()
    {
        chargeDashEnabled = false;                      //技能CD开启,禁用技能
        inSkill = true;                                 //技能使用阶段,禁用其他操作
        //anim.Play("isCharging")                       //播放蓄力动画
        Debug.Log("蓄力冲刺开始"+Time.time);
        yield return new WaitForSeconds(ChargeTime);    //等待蓄力
        rb.velocity = new Vector2(direction.x*DashSpeed, 0);//给与当前朝向的速度
        yield return new WaitForSeconds(DashTime);      //等待冲刺时间后
        rb.velocity=Vector2.zero;                       //速度置零
        inSkill = false;                                //技能使用完毕,启用其他操作
        Debug.Log("蓄力冲刺结束"+Time.time);      
        yield return new WaitForSeconds(DashCd);        //等待技能CD
        chargeDashEnabled = true;                       //CD过后重新可用技能
    }
    
    private IEnumerator DaggerThrow()
    {
        inSkill = true;                                 //技能使用阶段,禁用其他操作
        daggerThrowEnabled = false;                     //技能CD开启,禁用技能
        Debug.Log("进入匕首投掷技能");
        for (int i = 0; i < 3; i++)
        {
            Debug.Log("投出第" + i + "个匕首");
            StartCoroutine(DaggerGenerate(i));      //调用协程发射匕首
            yield return new WaitForSeconds(DaggerThrowInterval);      //每隔一段时间投出一个匕首
        }
        
        inSkill = false;                                //技能使用完毕,启用其他操作
        yield return new WaitForSeconds(DaggerThrowCd); //冷却5s
        daggerThrowEnabled = true;                      //CD过后重新可用技能
    }
    private IEnumerator DaggerGenerate(int index)
    { 
        daggers[index]=ObjectPool.Instance.GetObject(daggerPrefab); //从池中取出匕首实例
        daggers[index].transform.position = transform.position;     //将匕首坐标调整至自身坐标
        Debug.Log("siu");
        
        //daggers[index].GetComponent<Rigidbody2D>().velocity=new Vector2(transform.localScale.x,0)*10f;
        //StartCoroutine(DaggerAccelerate(daggers[index].GetComponent<Rigidbody2D>()));//上面这俩是变加速
         daggers[index].GetComponent<Rigidbody2D>().velocity=new Vector2(direction.x,0)*DaggerSpeed;//给与匕首均匀速度
        // daggers[index].GetComponent<Rigidbody2D>().AddForce(new Vector2(direction.x,0)*DaggerForce,ForceMode2D.Impulse);//给与匕首一个短暂的冲击力
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
}
