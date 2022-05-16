using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 输入处理类，根据输入对游戏角色发出相应指令
/// </summary>
public class InputHandler : MonoBehaviour
{
    private ICommand buttonJ;
    private ICommand buttonK;
    private ICommand buttonL;
    private ICommand buttonQ;
    private ICommand buttonE;
    private ICommand buttonF;
    private ICommand buttonU;

    [Header("可被控制的游戏角色")]
    public Player player;
    
    private void Awake()
    {
        buttonJ = new AttackCommand();
        buttonQ = new SwitchWeaponCommand();
        buttonE = new SwitchStatusCommand();
        buttonF = new SprintCommand();
    }

    
    private void Update()
    {
        //每一帧更新命令
        player.playerCommand = HandleInput();
    }

    /// <summary>
    /// 根据输入返回对应命令
    /// </summary>
    /// <returns>相应命令</returns>
    public ICommand HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.J)) return buttonJ;//普通攻击
        if (Input.GetKeyDown(KeyCode.K)) return buttonK;
        if (Input.GetKeyDown(KeyCode.L)) return buttonL;
        if (Input.GetKeyDown(KeyCode.F)) return buttonF;//冲刺攻击
        if (Input.GetKeyDown(KeyCode.Q)) return buttonQ;//切换武器
        if (Input.GetKeyDown(KeyCode.E)) return buttonE;//切换形态
        if (Input.GetKeyDown(KeyCode.U)) return buttonU;//蓄力

        //没输入就返回null
        return null;
    }
}
