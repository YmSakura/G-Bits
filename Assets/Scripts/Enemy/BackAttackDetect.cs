using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAttackDetect : MonoBehaviour
{
    [SerializeField]private Enemies Enemy;
    [SerializeField]private GameObject BackAttackHintUI;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!Enemy.inCombat&& other.CompareTag("Player"))
            BackAttackHintUI.SetActive(true);
            
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            BackAttackHintUI.SetActive(false);
    }

    private void Update()
    {
        if (Enemy.inCombat)
        {
            BackAttackHintUI.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
