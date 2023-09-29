using DG.Tweening;
using MoreMountains.Feedbacks;
using Rewired;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using WibertStudio;

public class PlayerDash : MonoBehaviour
{
    // References
    private Rigidbody2D rb;
    private PlayerAnimator animator;
    private Player player;

    [SerializeField] private MMF_Player dashFeedbacks;

    // Base parameters
    [BoxGroup("Base Parameters")]
    [PropertyTooltip("The amount of time between dashes")]
    [SerializeField] private float dashCoolDown;
    [BoxGroup("Base Parameters")]
    [PropertyTooltip("The amount of force that is applied to the player when dash is completed")]
    [SerializeField] private float dashForce;
    [BoxGroup("Base Parameters")]
    [PropertyTooltip("The distance the player goes when dashing")]
    [SerializeField] private float dashDistance;
    [BoxGroup("Base Parameters")]
    [PropertyTooltip("How long it takes to complete the dash")]
    [SerializeField] private float timeToCompleteDash;
    [BoxGroup("Base Parameters")]
    [PropertyTooltip("How long input is removed for the player when dashing")]
    [SerializeField] private float timeToRemovePlayerInput;

    // Logic / Debug variables
    [FoldoutGroup("Debug")]
    [ReadOnly]
    public bool CanDash = true;
    [FoldoutGroup("Debug")]
    [ReadOnly]
    public bool IsDashing;
    [FoldoutGroup("Debug")]
    [ReadOnly]
    [ShowInInspector]
    private bool isCoolDownRunning;

    private void Start()
    {
        rb = PlayerManager.instance.Rb;
        player = PlayerManager.instance.Player;
        animator = GetComponent<PlayerAnimator>();
    }

    private void Update()
    {
        PlayerInput();
        HandleDashCoolDown();
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

        // set up for dash
        CanDash = false;
        rb.velocity = new Vector2(0, 0);
        IsDashing = true;
        StartCoroutine(RemovePlayerInput());
        if (PlayerManager.instance.IsGrounded)
            StartCoroutine(DashCoolDown());

        // dashing to the right
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
        // dashing to the left
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

    private void HandleDashCoolDown()
    {
        if (PlayerManager.instance.IsGrounded && !CanDash && !isCoolDownRunning)
            StartCoroutine(DashCoolDown());
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
