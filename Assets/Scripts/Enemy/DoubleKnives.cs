using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoubleKnives : Enemies
{
    //[Header("基本属性")] private float healthValue = 100f;
    [SerializeField] private Slider alertBar;
    [SerializeField] private Image alertFill;

    private void FixedUpdate()
    {
        AlertValueChange();

    }
    /// <summary>
    /// 警戒值改变函数，发现玩家则增加，未发现则减少或不变
    /// </summary>
    private void AlertValueChange()
    {
        if (Time.frameCount % 10 == 0)              //每0.2s侦测一次玩家位置(可根据需要调整)
            isInAlertArea = IsInAlertArea();
        if (isInAlertArea)                          //如果在警戒区域内
        {
            var distance = AlertCheck();
            if (distance >= 0 && !base.inCombat)    //敌方目标还不在战斗状态中并且看到了玩家则警戒值上升
            {

                if (alertValue < 100)               //警戒值小于100则不断提升警戒值
                    if (distance <= 2) 
                        alertValue += 5;
                    else 
                        alertValue += 1f;
                else                                //警戒值大于100则进入战斗状态
                    inCombat = true;
            }
            else if(distance < 0)
            {
                alertValue -= 1;                    //如果还在警戒区域内但是视线被阻挡了则警戒值
            }
            Debug.Log("玩家处于警戒范围内");
        }
        else                                        //如果玩家在警戒区外则快速下降
        {
            if (alertValue > 0)
                alertValue -= 2;
        }
        
        if (alertValue > 0 && !inCombat)            //如果不在战斗状态下警戒值上升,则显示警示条
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
    
    
    
}
