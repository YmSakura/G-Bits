using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;


public class Bird : Enemies
{
    [Header("基本属性")] 
    private int faceDirection = 1;
    
    [SerializeField] private float moveSpeed = 0.75f;

    [Header("组件")] 
    private Animator anim;
    private Rigidbody2D rb;
    [SerializeField]private Collider2D beakCheck;
    private ContactFilter2D VisibleFilter2D;
        
    [Header("常量")] 
    private const float AttackCd = 0.75f;
    
    private enum Skill
    {
        FeatherAttack,
        BeakStrike
    }

    #region UnityEvent

    private void Start()
    {
        BaseInit();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        VisibleFilter2D.useLayerMask = true;
        VisibleFilter2D.layerMask = playerMask+visibleMask;
    }

    private void OnEnable()
    {
        healthValue = 25;
    }

    private void FixedUpdate()
    {
        if (!InSkill)
        {
            AlertValueChange();
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!inCombat) return;
        CombatAI();
    }

    private void OnDestroy()
    {
        gameObject.GetComponent<Attacked>().OnGetHit-=GetHit;
    }

    #endregion
    
    
    private void CombatAI()
    {
        throw new NotImplementedException();
    }
    
    private void SkillChoose()
    {
        Skill skill;
        int rand = Random.Range(1, 11);
        if (Distance > 4)
        {
            if (rand>6) return;
            skill = Skill.BeakStrike;
        }
        else
            skill = rand < 7 ? Skill.BeakStrike : Skill.FeatherAttack;
        SkillBegin(skill);
    }
    private void SkillBegin(Skill skill)
    {
        InSkill = true;
        Attackable = false;
        rb.velocity=Vector2.zero;
        switch (skill)
        {
            case Skill.FeatherAttack:
                Debug.Log("飞羽");
                //anim.Play("attack_consecutive");
                break;
            case Skill.BeakStrike :
                Collider2D[] collider2Ds=new Collider2D[2];
                if (Physics2D.OverlapCollider(beakCheck, PlayerFilter2D, collider2Ds) == 0 ||
                    Physics2D.OverlapCollider(beakCheck, VisibleFilter2D, collider2Ds) != 0)
                {
                    Debug.Log("区域内有障碍物");
                    return;
                }
                Debug.Log("喙击");
                //anim.Play("charge_knife");
                break;

        }
    }

    protected override void GetInterrupted()
    {
        StateLevel = 10;
        anim.Play("being_attacked");
        
    }
}
