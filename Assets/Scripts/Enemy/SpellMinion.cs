using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
public class SpellMinion : Enemies
{
    [Header("基本属性")] 
    private int faceDirection = 1;

    [SerializeField] private float moveSpeed = 0.75f;

    [Header("组件")] 
    private Animator anim;
    private Rigidbody2D rb;
    [SerializeField]private GameObject bloodBallPrefab;
    [SerializeField]private GameObject bloodSpearPrefab;
    


    [Header("技能属性")]
    private GameObject[] BloodBall = new GameObject[3];
    private GameObject bloodSpear;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");

    [Header("常量")] 
    private const float AttackCd = 1.5f;
    private const float BloodBallScale = 0.2f;
    private const float BloodBallSpeed = 10f;
    private const int BloodBallDamage = 10;
    private const float BloodSpearScale = 0.2f;
    private const float BloodSpearSpeed = 30f;
    private const int BloodSpearDamage = 10;
    private const float ScaleMultiplier = 0.1f;
    private const float StickForce = 50;
    private const float BloodSpearTime = 0.2f;

    private enum Skill
    {
        SBloodBall,
        SBloodSpear,
        SStickAttack
    }

    private void Start()
    {
        BaseInit();
        healthValue = 80;
        anim=GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bloodBallPrefab.GetComponent<TriggerAttack>().damage = BloodBallDamage;
        bloodSpearPrefab.GetComponent<CollisionAttack>().damage = BloodSpearDamage;
    }

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        healthValue = 80;
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

        
    }

    private void OnDestroy()
    {
        gameObject.GetComponent<Attacked>().OnGetHit-=GetHit;
    }


    
    private void DirectionChange()
    {
        if (faceDirection * (transform.position.x - playerTrans.position.x) < 0) return;//当玩家敌人相对位移和朝向符合则无需转向
        faceDirection *= -1;
        transform.localScale=new Vector3(faceDirection, 1, 1)*ScaleMultiplier;
        transform.position += new Vector3(faceDirection*1.676f,0,0);
    }
    

    
    #region AI

    private void CombatAI()
    {
        if (InSkill) return;
        DirectionChange();
        KeepAway();
        if(Attackable)
            SkillChoose();
    }


    private void KeepAway()
    {
        var dis = GetDistance();
        if (dis < 3*3)
        {
            rb.velocity = new Vector2(faceDirection * -moveSpeed, 0);
            anim.SetBool(IsWalking, true);
        }
        else if (dis>6*6&&dis<9*9)
        {
            rb.velocity=Vector2.zero;
            anim.SetBool(IsWalking, false);
        }
        else if(dis>8*8)
        {
            rb.velocity = new Vector2(faceDirection * moveSpeed, 0);
            anim.SetBool(IsWalking, true);
        }
    }
    #endregion

    #region Skill

    private void SkillChoose()
    {
        Skill skill;
        int rand = Random.Range(1, 11);
        if(Distance<2)
        {
            if (rand<=6) skill = Skill.SStickAttack;
            else if (rand < 9) skill = Skill.SBloodBall;
            else skill = Skill.SBloodSpear;
        }
        else
        {
            skill = rand < 7 ? Skill.SBloodBall : Skill.SBloodSpear;
        }
        SkillBegin(skill);
    }

    private void SkillBegin(Skill skill)
    {
        InSkill = true;
        Attackable = false;
        rb.velocity=Vector2.zero;
        switch (skill)
        {
            case Skill.SBloodBall:
                StateLevel = 1;
                Debug.Log("血球");
                anim.Play("attack_consecutive");
                break;
            case Skill.SBloodSpear:
                StateLevel = 1;
                Debug.Log("血矛");
                anim.Play("charge_knife");
                break;
            case Skill.SStickAttack:
                StateLevel = 2;
                Debug.Log("棍击");
                anim.Play("charge");
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
                
                break;
        }
        StartCoroutine(ResetAttackCd(AttackCd));
    }
    

    protected override void GetInterrupted()
    {
        StateLevel = 10;
        anim.Play("being_attacked");
    }

    #endregion
    
    #region BloodBall
    

    private IEnumerator BloodBallGenerate(int index)
    {
        BloodBall[index] = ObjectPool.Instance.GetObject(bloodBallPrefab);
        
        Transform trans = BloodBall[index].transform;

        trans.position = position;
        trans.localScale = new Vector3(faceDirection*BloodBallScale,BloodBallScale,1);

        float timer = 0;
        do
        {
            if (!BloodBall[index]) yield break;
            var angle = Vector2.Angle(Vector2.zero, playerTrans.position-transform.position);
            trans.rotation = new Quaternion(0, 0, angle, 0);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        } while (timer<0.3f);
        
        if(BloodBall[index].GetComponent<SpriteRenderer>().color.a<1)yield break;
        Vector2 dir = playerTrans.position-transform.position;
        BloodBall[index].GetComponent<Rigidbody2D>().velocity=BloodBallSpeed * (dir/dir.magnitude);
        
        yield return new WaitForSeconds(2f);
        
        if (BloodBall[index])
            StartCoroutine(BloodBall[index].GetComponent<BloodBall>().BloodBallFade());
        
    }

    // private IEnumerator BloodBallShot()
    // {
    //     foreach (GameObject bloodBall in BloodBall)
    //     {
    //         if(!bloodBall) continue;
    //         Vector2 dir =   playerTrans.position-transform.position;
    //         bloodBall.GetComponent<Rigidbody2D>().velocity=BloodBallSpeed * (dir/dir.magnitude);
    //     }
    //     yield return new WaitForSeconds(2f);
    //     foreach (GameObject bloodBall in BloodBall)
    //     {
    //         if (bloodBall)
    //             StartCoroutine(bloodBall.GetComponent<BloodBall>().BloodBallFade());
    //     }
    // }
    
    #endregion

    #region BloodSpear


    private void BloodSpearGenerate()
    {
        bloodSpear = ObjectPool.Instance.GetObject(bloodSpearPrefab);
        bloodSpear.transform.position = transform.position +new Vector3(0, 0.5f, 0);
        bloodSpear.transform.localScale =
            new Vector3(faceDirection * BloodSpearScale, BloodSpearScale, BloodSpearScale);
    }

    private IEnumerator BloodSpearThrow()
    {
        bloodSpear.GetComponent<Rigidbody2D>().velocity = new Vector2(BloodSpearSpeed*faceDirection, 0);
        yield return new WaitForSeconds(BloodSpearTime);
        if (bloodSpear)
            StartCoroutine(bloodSpear.GetComponent<BloodSpear>().BloodSpearFade());
        bloodSpear = null;
    }
    #endregion

    #region StickAttack

    public void StickAttack()
    {
        Collider2D[] player=new Collider2D[1];
        Vector2 center = transform.position;
        Vector2 left = center + new Vector2(-2, -1);
        Vector2 right = center + new Vector2(2, 0);
        Vector2 dir = new Vector2(center.x < playerTrans.position.x ? -1 : 1,0);
        Physics2D.OverlapArea(left, right, PlayerFilter2D, player);
        Debug.DrawLine(left,right,Color.white);
        player[0]?.gameObject.GetComponent<Attacked>().OnGetHurt(playerTrans.position,dir*StickForce,40,2);
            
    }

    #endregion
}
