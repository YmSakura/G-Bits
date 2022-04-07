using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemies : MonoBehaviour
{
    [Header("基本属性")] 
    public float healthValue;       //生命值
    private bool isInAlertArea;     //玩家是否处于警戒范围
    public bool inCombat;           //自身是否处于战斗状态
    public float alertValue;        //警戒值
    public Vector2 position;        //自身坐标
    public Vector2 direction;       //自身朝向

    [Header("Component")] 
    [SerializeField]private Transform alertAreaPointA;      //矩形警戒区域对角线一段
    [SerializeField]private Transform alertAreaPointB;      //矩形警戒区域对角线另一段
    [SerializeField]protected Transform playerTrans;          //玩家坐标

    [SerializeField]private LayerMask playerMask;       //Player层包括玩家(所有可以被看见的)(用于判断)
    [SerializeField]private LayerMask visibleMask;      //Visible层包括墙体(所有可以被看见的,会被阻挡视线的)(用于判断)
    [SerializeField]private LayerMask enemyMask;        //Enemy层包括所有敌人(用于范围唤醒敌人进入战斗状态)
    private ContactFilter2D playerFilter2D;             //用于筛选出Player层的对象
    private ContactFilter2D enemyFilter2D;              //用于筛选出Enemy层的对象
    private readonly Collider2D[] _result = new Collider2D[1];          //overlap获取到的玩家碰撞体
    private readonly Collider2D[] _enemyColls = new Collider2D[8];      //overlap获取到的周边敌方目标碰撞体
    
    [SerializeField] private Slider alertBar;
    [SerializeField] private Image alertFill;
    protected float distance;


    //private float alertMuliplier = 10f;

    protected void BaseInit()
    {
        playerFilter2D.layerMask = playerMask;
        enemyFilter2D.layerMask = enemyMask;
        playerFilter2D.useLayerMask = true;
        enemyFilter2D.useLayerMask = true;
        Debug.Log("加载");
    }

    /// <summary>
    /// 判断玩家是否处于警戒区域内
    /// </summary>
    /// <returns>玩家是否处于警戒区域内的布尔值,在->真;不在->假</returns>
    private bool IsInAlertArea()
    {
        _result[0] = null;
        Physics2D.OverlapArea(alertAreaPointA.position, alertAreaPointB.position, playerFilter2D,
            _result); //以AB两点画矩形警戒区域,并筛选出其中的Player层的object(即玩家),存入_result数组中
        return _result[0]; //_result数组中有对象 则代表 警戒区域内有玩家 返回真,无则假
    }

    /// <summary>
    /// 敌方目标是否能够看见玩家（玩家处于警戒范围内并且无墙体物品阻挡）
    /// </summary>
    /// <returns>distance</returns>
    private float AlertCheck()
    {
        position = transform.position;                              //确定自身坐标
        direction = (Vector2)playerTrans.position - position;              //计算看向玩家的方向
        Debug.DrawRay(position,direction,Color.red,0.5f);
        RaycastHit2D hit = Physics2D.Raycast(position,direction,12f, visibleMask+playerMask);     //射线射向玩家
        if (hit.collider != null&&hit.collider.CompareTag("Player"))//如果射线目标为玩家，意味着敌方目标可以看见玩家
        {
            var distance = (int)Mathf.Abs(hit.point.x - position.x);
            Debug.Log("看到玩家，距离为："+distance);
            //Debug.Log(alertValue);
            return distance;
        }

        return -1f;
    }

    /// <summary>
    /// 警戒值改变函数，发现玩家则增加，未发现则减少或不变
    /// </summary>
    protected void AlertValueChange()
    {
        if (Time.frameCount % 10 == 0)              //每0.2s侦测一次玩家位置(可根据需要调整)
            isInAlertArea = IsInAlertArea();
        if (isInAlertArea)                          //如果在警戒区域内
        {
            distance = AlertCheck();                                                //战斗中但是不在警戒区域内是否需要检测
            if (distance >= 0 && !inCombat)         //敌方目标还不在战斗状态中并且看到了玩家则警戒值上升
            {

                if (alertValue < 100)               //警戒值小于100则不断提升警戒值
                    if (distance <= 2) 
                        alertValue += 5;
                    else 
                        alertValue += 1f;
                else //警戒值大于100则进入战斗状态
                {
                    inCombat = true;
                    GroupEnmity();
                }
            }
            else if(distance < 0)
            {
                if (alertValue > 0)
                    alertValue -= 1;                //如果还在警戒区域内但是视线被阻挡了则警戒值
            }
            //Debug.Log("玩家处于警戒范围内或者在战斗中");
        }
        else                                        //如果玩家在警戒区外则快速下降
        {
            if (alertValue > 0)
                alertValue -= 2;
        }
        
        if (alertValue > 0 && !inCombat)            //如果不在战斗状态下警戒值改变,则显示警示条
            alertBar.gameObject.SetActive(true);
        else                                        //否则禁用警示条
            alertBar.gameObject.SetActive(false);
        
        if (alertBar.enabled)
        {
            alertBar.value = alertValue;
            alertFill.color = new Color(alertBar.value / 100, 0, 0, 1);
        }
        
        if (alertValue < 0.1 && inCombat)           //如果警戒值归0则从战斗状态中退出
            inCombat = false;
        
    }
    
    
    /// <summary>
    /// 当有一敌方目标发现玩家进入战斗状态，会调用此函数让附近区域内的敌方目标一同进入战斗状态
    /// 数值：敌人个数8，呼唤范围5
    /// </summary>
    private void GroupEnmity()
    {
        
        Physics2D.OverlapCircle(position, 5f,enemyFilter2D,_enemyColls);   //画一个圆获取
        foreach (var coll in _enemyColls)
        {
            if(coll!=null)
                coll.gameObject.GetComponent<Enemies>().inCombat = true;
        }
    }
    


}
