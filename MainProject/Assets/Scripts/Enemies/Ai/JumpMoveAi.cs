using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpMoveAi : EnemyAi
{

    private Rigidbody2D rb;

    [SerializeField] private float moveSpeed;

    [SerializeField] private LayerMask groundLayers;

    // Ground check
    [SerializeField] private Vector2 groundCheckOffset;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private float groundCheckDistance;
    private bool isGrounded()
    {
        if (Physics2D.BoxCast(new Vector2(transform.position.x + groundCheckOffset.x, transform.position.y + groundCheckOffset.y), groundCheckSize, 0, transform.up, groundCheckDistance, groundLayers))
            return true;
        else
            return false;
    }

    // Wall check
    [SerializeField] private Vector2 wallCheckOffset;
    [SerializeField] private Vector2 wallCheckSize;
    [SerializeField] private float wallCheckDistance;

    private bool isTouchingWall()
    {
        if (Physics2D.BoxCast(new Vector2(transform.position.x + wallCheckOffset.x, transform.position.y + wallCheckOffset.y), wallCheckSize, 0, transform.right, wallCheckDistance, groundLayers))
            return true;
        else
            return false;
    }

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

    private void Update()
    {
        if (isTouchingWall() || !isGrounded() && CanMove && rb.velocity.y == 0)
        {
            moveSpeed *= -1;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            groundCheckOffset.x *= -1;
            wallCheckOffset.x *= -1;

        }
    }


    private void FixedUpdate()
    {
        if (CanMove)
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    private void OnDrawGizmosSelected()
    {
        // Ground
        if (isGrounded())
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(new Vector3(transform.position.x + groundCheckOffset.x, transform.position.y + groundCheckOffset.y, 0) + transform.up * groundCheckDistance, groundCheckSize);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(transform.position.x + groundCheckOffset.x, transform.position.y + groundCheckOffset.y, 0) + transform.up * groundCheckDistance, groundCheckSize);
        }

        // Wall
        if (!isTouchingWall())
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(new Vector3(transform.position.x + wallCheckOffset.x, transform.position.y + wallCheckOffset.y, 0) + transform.right * wallCheckDistance, wallCheckSize);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(transform.position.x + wallCheckOffset.x, transform.position.y + wallCheckOffset.y, 0) + transform.right * wallCheckDistance, wallCheckSize);
        }

    }
}
