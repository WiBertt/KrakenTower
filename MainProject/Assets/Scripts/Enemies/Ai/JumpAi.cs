using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAi : EnemyAi
{
    private Rigidbody2D rb;

    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCoolDown;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("Jump", 1, jumpCoolDown);
    }

    private void Jump()
    {
        if (CanMove)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
}
