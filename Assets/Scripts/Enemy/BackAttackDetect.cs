using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAttackDetect : MonoBehaviour
{ 
    private Enemies Enemy;
    [SerializeField]private GameObject backAttackHintUIPrefab;
    private static GameObject backAttackHintUI;
    public static GameObject AimTarget;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!Enemy.inCombat && other.CompareTag("Player"))
        {
            backAttackHintUI.SetActive(true);
            AimTarget = transform.parent.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            backAttackHintUI.SetActive(false);
    }

    private void Awake()
    {
        if (backAttackHintUI) return;
        backAttackHintUI=ObjectPool.Instance.GetObject(backAttackHintUIPrefab);
        // //backAttackHintUI.GetComponent<RectTransform>().Translate(0, -228, 0);
    }

    private void Start()
    {
        Enemy = transform.parent.GetComponent<Enemies>();
    }

    private void Update()
    {
        if (Enemy.inCombat)
        {
            backAttackHintUI.SetActive(false);
            gameObject.SetActive(false);
        }
    }
    
}
