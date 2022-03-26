using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float inputX, inputY;
    private Vector2 moveInput;
    private float moveSpeed = 5.0f;
    private bool openRarar;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        GetInput();
        Movement();
    }

    private void Movement()
    {
        transform.Translate(moveInput * moveSpeed * Time.deltaTime, Space.World);
    }

    private void GetInput()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
        moveInput = new Vector2(inputX, inputY);

        openRarar = Input.GetKeyDown(KeyCode.F);
    }

    private void OpenRadar()
    {
        
    }
    
}
