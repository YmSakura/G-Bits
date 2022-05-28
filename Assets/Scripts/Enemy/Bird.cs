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
    [SerializeField]private GameObject beakObj;
    [SerializeField]private GameObject featherObj;
    private ContactFilter2D VisibleFilter2D;
    
        
    [Header("常量")] 
    private const float AttackCd = 0.75f;
    private const float ScaleMultiplier = 0.1f;    //缩放比例
    
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
        if (!inCombat) return;
        CombatAI();
    }
    
    // Update is called once per frame
    void Update()
    {
        base.Update();
        
    }

    private void OnDestroy()
    {
        gameObject.GetComponent<Attacked>().OnGetHit-=GetHit;
    }

    #endregion

    #region Combat

    private void CombatAI()
    {
        if (InSkill) return;
        DirectionChange();
        MovementAI();
        if(Attackable)
            SkillChoose();
    }

    private void DirectionChange()
    {
        faceDirection = transform.position.x < playerTrans.position.x ? -1 : 1;
        transform.localScale=new Vector3(faceDirection, 1, 1)*ScaleMultiplier;
    }

    private void MovementAI()
    {
        
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
                anim.Play("attack");
                featherObj.SetActive(true);
                break;
            case Skill.BeakStrike :
                Collider2D[] collider2Ds=new Collider2D[2];
                if (Physics2D.OverlapCollider(beakCheck, PlayerFilter2D, collider2Ds) == 0 ||
                    Physics2D.OverlapCollider(beakCheck, VisibleFilter2D, collider2Ds) != 0)
                {
                    Debug.Log("区域内有障碍物");
                    InSkill = false;
                    Attackable = true;
                    return;
                }
                beakObj.SetActive(true);
                Debug.Log("喙击");
                anim.Play("sprint");
                break;

        }
    }


    private void SkillEnd(Skill skill)
    {
        switch (skill)
        {
            case Skill.FeatherAttack:
                featherObj.SetActive(false);
                //anim.Play
                break;
            case Skill.BeakStrike:
                beakObj.SetActive(false);
                //anim.Play
                break;
        }
        StartCoroutine(ResetAttackCd(AttackCd));
    }


    protected override void GetInterrupted()
    {
        StateLevel = 10;
        beakObj.SetActive(false);
        featherObj.SetActive(false);
        anim.Play("being_attacked");
        
    }

    #endregion
    
}
