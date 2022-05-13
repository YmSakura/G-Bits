using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class SpellMinion : Enemies
{
    [Header("基本属性")] 
    private int faceDirection = 1;

    [Header("组件")] 
    private GameObject BloodBallPrefab;
    private GameObject BloodSpearPrefab;
    private GameObject[] BloodBall = new GameObject[3];
    
    
    [Header("技能属性")]


    [Header("常量")] 
    private const float AttackCd = 1.2f;
    private const float BloodBallScale = 0.5f;
    private const float BloodBallSpeed = 10f;
    private const float BloodBallDamage = 10f;
    private const float BloodSpearScale = 0.5f;
    private const float BloodSpearSpeed = 20f;
    private const float BloodSpearDamage = 10f;

    enum Skill
    {
        SBloodBall,
        SBloodSpear,
        SStickAttack
    }
    void Start()
    {
        BaseInit();
        healthValue = 80;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        gameObject.GetComponent<Attacked>().OnGetHit-=GetHit;
    }



    private void SkillBegin(Skill skill)
    {
        InSkill = true;
        Attackable = false;
        switch (skill)
        {
            case Skill.SBloodBall:
                //anim.Play
                break;
            case Skill.SBloodSpear:
                //anim.Play
                break;
            case Skill.SStickAttack:
                //anim.Play
                break;
        }
    }

    private void SkillEnd(Skill skill)
    {
        switch (skill)
        {
            case Skill.SBloodBall:
                //anim.Play
                break;
            case Skill.SBloodSpear:
                //anim.Play
                break;
            case Skill.SStickAttack:
                Vector2 selfPosition = transform.position;
                Vector2 left = new Vector2(selfPosition.x - 2, selfPosition.y + 1);
                Vector2 right = new Vector2(selfPosition.x + 2, selfPosition.y - 1);
                PlayerColl[0] = null;
                Physics2D.OverlapArea(left, right, PlayerFilter2D, PlayerColl); 
                //以AB两点画矩形区域,并筛选出其中的Player层的object(即玩家),存入PlayerColl数组中
                PlayerColl[0]?.GetComponent<Attacked>().OnGetHurt(PlayerColl[0].transform.position,Vector2.zero, 40);
                break;
        }
        InSkill = false;
        StartCoroutine(ResetAttackCd(AttackCd));
    }
    
    #region AI

    private void CombatAI()
    {
        
    }

    private void MovementAI()
    {
        
    }

    private void KeepAway()
    {
        
    }
    #endregion
    
    
    #region BloodBall
    

    private IEnumerator BloodBallGenerate(int index)
    {
        BloodBall[index] = ObjectPool.Instance.GetObject(BloodBallPrefab);
        BloodBall[index].transform.position = transform.position +new Vector3(5, 0, 0);
        BloodBall[index].transform.localScale =
            new Vector3(faceDirection * BloodBallScale, BloodBallScale, BloodBallScale);
        BloodBall[index].GetComponent<Rigidbody2D>().velocity = new Vector2(BloodBallSpeed*faceDirection, 0);
        yield return new WaitForSeconds(1f);

    }

    #endregion

    #region BloodSpear


    private IEnumerator BloodSpearGenerate()
    {
        GameObject bloodSpear = ObjectPool.Instance.GetObject(BloodBallPrefab);
        bloodSpear.transform.position = transform.position +new Vector3(5, 0, 0);
        bloodSpear.transform.localScale =
            new Vector3(faceDirection * BloodSpearScale, BloodSpearScale, BloodSpearScale);
        bloodSpear.GetComponent<Rigidbody2D>().velocity = new Vector2(BloodSpearSpeed*faceDirection, 0);
        yield return new WaitForSeconds(1f);

    }
    #endregion

    #region StickAttack
    

    #endregion
}
