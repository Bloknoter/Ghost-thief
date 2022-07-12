using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private PlayerController playercon;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            playercon.OnInteraction();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playercon.OnShoot();
        }
    }

    private void FixedUpdate()
    {
        playercon.ZeroMoving();
        if (Input.GetKey(KeyCode.W))
        {
            playercon.OnUp();
        }
        if (Input.GetKey(KeyCode.S))
        {
            playercon.OnDown();
        }
        if (Input.GetKey(KeyCode.D))
        {
            playercon.OnRight();
        }
        if (Input.GetKey(KeyCode.A))
        {
            playercon.OnLeft();
        }
        playercon.OnMove();
    }
}
