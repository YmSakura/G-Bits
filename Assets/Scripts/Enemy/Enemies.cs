using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    [Header("基本属性")] 
    public float healthValue;       //生命值
    protected bool isInAlertArea;   //玩家是否处于警戒范围
    public bool inCombat;           //自身是否处于战斗状态
    public float alertValue;        //警戒值
    public Vector2 position;        //自身坐标
    public Vector2 direction;       //自身朝向

    [Header("Component")] 
    [SerializeField]private Transform alertAreaPointA;      //矩形警戒区域对角线一段
    [SerializeField]private Transform alertAreaPointB;      //矩形警戒区域对角线另一段
    [SerializeField]private Transform selfTrans;            //自身坐标
    [SerializeField]private Transform playerTrans;          //玩家坐标

    [SerializeField]private LayerMask playerMask;       //Player层包括玩家(所有可以被看见的)(用于判断)
    [SerializeField]private LayerMask visibleMask;      //Visible层包括墙体(所有可以被看见的,会被阻挡视线的)(用于判断)
    [SerializeField]private LayerMask enemyMask;        //Enemy层包括所有敌人(用于范围唤醒敌人进入战斗状态)
    private ContactFilter2D playerFilter2D;             //用于筛选出Player层的对象
    private ContactFilter2D enemyFilter2D;              //用于筛选出Enemy层的对象
    private Collider2D[] _result = new Collider2D[1];   //overlap获取到的玩家碰撞体
    private Collider2D[] colls = new Collider2D[8];             //overlap获取到的周边敌方目标碰撞体

    //private float alertMuliplier = 10f;
    
    private void OnValidate()
    {
        playerFilter2D.layerMask = playerMask;
        enemyFilter2D.layerMask = enemyMask;
        playerFilter2D.useLayerMask = true;
        enemyFilter2D.useLayerMask = true;
    }
    


    /// <summary>
    /// 敌方目标是否能够看见玩家（玩家处于警戒范围内并且无墙体物品阻挡）
    /// </summary>
    /// <returns>distance的三次方+0.125</returns>
    protected float AlertCheck()
    {
        position = selfTrans.position;                              //确定自身坐标
        direction = (Vector2)playerTrans.position - position;              //计算看向玩家的方向
        Debug.DrawRay(position,direction,Color.red,0.5f);
        RaycastHit2D hit = Physics2D.Raycast(position,direction,8f, visibleMask+playerMask);     //射线射向玩家
        if (hit.collider != null&&hit.collider.CompareTag("Player"))//如果射线目标为玩家，意味着敌方目标可以看见玩家
        {
            float distance = (int)Mathf.Abs(hit.point.x - position.x);
            Debug.Log("看到玩家，距离为："+distance);
            //Debug.Log(alertValue);
            return distance;
        }

        return -1f;
    }

    /// <summary>
    /// 判断玩家是否处于警戒区域内
    /// </summary>
    /// <returns>玩家是否处于警戒区域内</returns>
    protected bool IsInAlertArea()
    {
        _result[0] = null;
        Physics2D.OverlapArea(alertAreaPointA.position, alertAreaPointB.position,playerFilter2D,
            _result);//以AB两点画矩形警戒区域,并筛选出其中的Player层的object(即玩家),存入_result数组中
        return _result[0];//_result数组中有对象 则代表 警戒区域内有玩家 返回真,无则假
    }
    
    /// <summary>
    /// 当有一敌方目标发现玩家进入战斗状态，会调用此函数让附近区域内的敌方目标一同进入战斗状态
    /// 数值：敌人个数8，呼唤范围5
    /// </summary>
    protected void GroupEnmity()
    {
        
        Physics2D.OverlapCircle(position, 5f,enemyFilter2D,colls);   //画一个圆获取
        foreach (var coll in colls)
        {
            coll.gameObject.GetComponent<Enemies>().inCombat = true;
        }
    }
    
    
}
