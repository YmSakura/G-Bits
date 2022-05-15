using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bird : Enemies
{
    [Header("基本属性")] 
    private int faceDirection = 1;

    [SerializeField] private float moveSpeed = 0.75f;

    [Header("组件")] 
    private Animator anim;
    private Rigidbody2D rb;

    private enum Skill
    {
    
    }

    private void Start()
    {
        BaseInit();
    }

    private void OnEnable()
    {
        healthValue = 25;
    }

    void Update()
    {
        
    }
}
