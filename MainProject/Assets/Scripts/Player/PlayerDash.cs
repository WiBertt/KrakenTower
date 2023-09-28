using DG.Tweening;
using MoreMountains.Feedbacks;
using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WibertStudio;

public class PlayerDash : MonoBehaviour
{
    // References
    private Rigidbody2D rb;
    private PlayerAnimator animator;

    [SerializeField] private float dashCoolDown;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDistance;
    [SerializeField] private float timeToCompleteDash;
    [SerializeField] private float timeToRemovePlayerInput;

    [SerializeField] private MMF_Player dashFeedbacks;

    public bool CanDash { get; set; } = true;
    public bool IsDashing { get; private set; }

    // Rewired
    [SerializeField] private int playerID = 0;
    [SerializeField] private Player player;

    private bool isCoolDownRunning;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<PlayerAnimator>();
        player = ReInput.players.GetPlayer(playerID);
    }
    private void Update()
    {
        PlayerInput();
        if (PlayerManager.instance.IsGrounded && !CanDash && !isCoolDownRunning)
            StartCoroutine(DashCoolDown());
    }

    private void FixedUpdate()
    {
        //if(PlayerManager.instance.IsGrounded)
        //    ResetDash();
    }

    private void PlayerInput()
    {
        if (player.GetButtonDown("Dash"))
            Dash();
    }

    private void Dash()
    {
        if (!CanDash)
            return;

        StartCoroutine(RemovePlayerInput());
        CanDash = false;
        if (PlayerManager.instance.IsGrounded)
        {
            StartCoroutine(DashCoolDown());
        }
        rb.velocity = new Vector2(0, 0);
        IsDashing = true;
        if (PlayerManager.instance.IsFacingRight && !PlayerManager.instance.IsOnRightWall)
        {
            animator.SetDashAnimation();
            dashFeedbacks.PlayFeedbacks();
            rb.DOMoveX(transform.position.x + dashDistance, timeToCompleteDash).SetEase(Ease.InSine).OnUpdate(() =>
            {
                rb.velocity = new Vector2(transform.right.x * dashForce, rb.velocity.y);

            }).OnComplete(() =>
            {
                IsDashing = false;
                dashFeedbacks.StopFeedbacks();
            });
        }
        else if (!PlayerManager.instance.IsFacingRight && !PlayerManager.instance.IsOnLeftWall)
        {
            animator.SetDashAnimation();
            dashFeedbacks.PlayFeedbacks();
            rb.DOMoveX(transform.position.x - dashDistance, timeToCompleteDash).SetEase(Ease.InSine).OnUpdate(() =>
            {
                rb.velocity = new Vector2(-transform.right.x * dashForce, rb.velocity.y);
            }).OnComplete(() =>
            {
                IsDashing = false;
                dashFeedbacks.StopFeedbacks();
            });
        }
    }

    private IEnumerator RemovePlayerInput()
    {
        PlayerManager.instance.DoesPlayerHaveControl = false;
        yield return new WaitForSecondsRealtime(timeToRemovePlayerInput);
        PlayerManager.instance.DoesPlayerHaveControl = true;
    }
    private IEnumerator DashCoolDown()
    {
        isCoolDownRunning = true;
        yield return new WaitForSecondsRealtime(dashCoolDown);
        CanDash = true;
        isCoolDownRunning = false;
    }
}
