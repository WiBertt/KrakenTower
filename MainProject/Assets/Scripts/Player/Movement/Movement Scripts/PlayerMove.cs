using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Sirenix.OdinInspector;
using WibertStudio;

[RequireComponent(typeof(PlayerManager))]
public class PlayerMove : MonoBehaviour
{
    // references
    private PlayerManager playerManager;
    private PlayerAnimator animator;
    private Player player;

    #region Movement variables
    [BoxGroup("Base Parameters")]
    [SerializeField] private float baseMoveSpeed;
    [BoxGroup("Base Parameters")]
    [PropertyTooltip("The move speed of the player while attacking")]
    [SerializeField] private float moveAttackSpeed;
    [BoxGroup("Base Parameters")]
    [PropertyTooltip("How long the player is affected by the attack move speed slow")]
    [SerializeField] private float moveSpeedAttackSlowDuration;
    [BoxGroup("Physics")]
    [PropertyTooltip("How fast the player reaches their max move speed on the ground and in air")]
    [SerializeField] private float acceleration;
    [BoxGroup("Physics")]
    [PropertyTooltip("How fast the player slows down on the ground")]
    [SerializeField] private float groundDecceleration;
    [BoxGroup("Physics")]
    [PropertyTooltip("How fast the player slows down in the air")]
    [SerializeField] private float airDecceleration;
    [BoxGroup("Physics")]
    [PropertyTooltip("Controls how much of an influence the velocity adds to the equation. SHOULD BE FINE DO NOT TAMPER!")]
    [SerializeField] private float velPower;
    [BoxGroup("Physics")]
    [PropertyTooltip("Artificial friction added to the player while on the ground")]
    [SerializeField] private float groundFriction;
    [BoxGroup("Physics")]
    [PropertyTooltip("Artificial friction added to the player while in the air")]
    [SerializeField] private float airFriction;
    #region Holders / Debug Variables
    [FoldoutGroup("Debug")]
    [ReadOnly]
    public float initialMoveSpeed;
    [FoldoutGroup("Debug")]
    [ReadOnly]
    [ShowInInspector]
    private float deceleration;
    [FoldoutGroup("Debug")]
    [ReadOnly]
    [ShowInInspector]
    private float friction;
    [FoldoutGroup("Debug")]
    [ReadOnly]
    public float horizontalInput;
    [FoldoutGroup("Debug")]
    [ReadOnly]
    public float currentMoveSpeed;
    [FoldoutGroup("Debug")]
    [ReadOnly]
    #endregion
    public bool isAttackMoveSlowActive;
    public bool IsCoroutineActive { get; set; }
    #endregion

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        animator = GetComponent<PlayerAnimator>();

        player = playerManager.Player;

        currentMoveSpeed = baseMoveSpeed;
        initialMoveSpeed = baseMoveSpeed;
    }

    public void SetGroundValues()
    {
        deceleration = groundDecceleration;
        friction = groundFriction;
    }

    public void SetAirValues()
    {
        deceleration = airDecceleration;
        friction = airFriction;
    }

    public void Update()
    {
        PlayerInput();
    }

    private void PlayerInput()
    {
        if (!playerManager.DoesPlayerHaveControl)
            return;

        horizontalInput = player.GetAxis("Move Horizontal");
    }

    public void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (!playerManager.DoesPlayerHaveControl)
            return;

        // movement equations
        float targetSpeed = horizontalInput * currentMoveSpeed;
        float speedDif = targetSpeed - playerManager.Rb.velocity.x;

        float accelRate = (Mathf.Abs(targetSpeed) > .01f) ? acceleration : deceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        playerManager.Rb.AddForce(movement * Vector2.right);

        if (playerManager.Rb.velocity.x > .01f || playerManager.Rb.velocity.x < -.01f && playerManager.IsGrounded)
        {
            if (!IsCoroutineActive)
            {
                IsCoroutineActive = true;
            }
        }

        // Friction
        if (Mathf.Abs(horizontalInput) == 0)
        {
            float amount = Mathf.Min(Mathf.Abs(playerManager.Rb.velocity.x), Mathf.Abs(friction));

            amount *= Mathf.Sign(playerManager.Rb.velocity.x);

            playerManager.Rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }

    public void ChangeMoveSpeed(float newMoveSpeed)
    {
        baseMoveSpeed = newMoveSpeed;
    }

    public IEnumerator AttackSlow()
    {
        if (!playerManager.IsGrounded)
            yield break;

        isAttackMoveSlowActive = true;
        currentMoveSpeed = moveAttackSpeed;
        yield return new WaitForSecondsRealtime(moveSpeedAttackSlowDuration);
        currentMoveSpeed = baseMoveSpeed;
        isAttackMoveSlowActive = false;
    }

    /// <summary>
    /// sets move speed back to its initial value
    /// </summary>
    public void ResetMoveSpeed()
    {
        baseMoveSpeed = initialMoveSpeed;
    }
}