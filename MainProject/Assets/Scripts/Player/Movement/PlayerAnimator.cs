using MoreMountains.Feedbacks;
using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WibertStudio;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    private PlayerManager playerManager;
    private PlayerWallSlide_WallJump wallSlideState;
    [SerializeField] private float yVFXOffSet;
    [SerializeField] private MMF_Player HardLandingFeedBack;
    [SerializeField] private float minSpeedToDoStopAnim;
    [SerializeField] private float moveSpeedTurnThreshHold;
    [SerializeField] private float minTimeToInAirToDoHardLanding;
    [SerializeField] private float timeToRemovePlayerControlFromHardLanding;
    private bool isTurning;
    private bool isRunStopping;
    private bool isDashing;
    private bool isMovingRight;
    private bool isWallJumping;
    private bool willDoHardLanding;
    private bool isWallContactComplete;
    private bool isAttacking;
    private Dictionary<string, float> clipDictionary = new Dictionary<string, float>();

    private bool isTimerRunning;
    private bool isIdle()
    {
        if (Mathf.Abs(playerManager.Rb.velocity.x) < .01 && playerManager.IsGrounded)
            return true;
        else return false;
    }

    private Player player;
    private void Awake()
    {
        player = ReInput.players.GetPlayer(0);
        foreach (AnimationClip ac in animator.runtimeAnimatorController.animationClips)
        {
            clipDictionary.Add(ac.name, ac.length);
        }
    }
    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        wallSlideState = GetComponent<PlayerWallSlide_WallJump>();
    }

    private void Update()
    {
        animator.SetBool("IsIdle", isIdle());
        animator.SetFloat("XVelocity", Mathf.Abs(playerManager.Rb.velocity.x));
        animator.SetFloat("YVelocity", (playerManager.Rb.velocity.y));
        animator.SetBool("IsGrounded", playerManager.IsGrounded);
        animator.SetBool("IsRunStopping", isRunStopping);
        animator.SetBool("IsTurning", isTurning);
        animator.SetBool("IsDashing", isDashing);
        animator.SetBool("IsWallSliding", wallSlideState.isSlidingOnWall);
        animator.SetBool("WillDoHardLanding", willDoHardLanding);
        animator.SetBool("IsWallContactComplete", isWallContactComplete);
        animator.SetBool("IsDoingWallJump", isWallJumping);

        // Turning
        if (playerManager.Rb.velocity.x < -moveSpeedTurnThreshHold)
            isMovingRight = false;
        else if (playerManager.Rb.velocity.x > moveSpeedTurnThreshHold)
            isMovingRight = true;

        if (player.GetAxis("Move Horizontal") < -.01f && isMovingRight && playerManager.Rb.velocity.x > moveSpeedTurnThreshHold || player.GetAxis("Move Horizontal") > .01f && !isMovingRight && playerManager.Rb.velocity.x < -moveSpeedTurnThreshHold)
        {
            SetRunTurnAnimation();
            return;
        }

        if (isRunStopping || isTurning || isDashing)
            return;

        // Stopping
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) || player.GetAxis("Move Horizontal") == 0)
        {


            if (playerManager.IsFacingRight)
            {
                if (PlayerManager.instance.Rb.velocity.x >= minSpeedToDoStopAnim)
                {
                    SetRunStopAnimation();
                }
            }
            else
            {
                if (PlayerManager.instance.Rb.velocity.x <= -minSpeedToDoStopAnim)
                {
                    SetRunStopAnimation();
                }
            }
        }

        // Wall contact
        if (!wallSlideState.isSlidingOnWall)
            isWallContactComplete = false;
        if (wallSlideState.isSlidingOnWall && !isWallContactComplete)
        {
            StartCoroutine(AnimationTimer(clipDictionary.GetValueOrDefault("WallContact")));
        }
    }

    public void CheckLandAnimation()
    {
        if (willDoHardLanding)
        {
            playerManager.Rb.velocity = Vector2.zero;
            StartCoroutine(AnimationTimer(clipDictionary.GetValueOrDefault("Landing")));
            StartCoroutine(RemovePlayerInput(timeToRemovePlayerControlFromHardLanding));
        }
    }

    public void SetWallJumpAnimation()
    {
        StopAllCoroutines();
        ResetAnimations();
        isWallJumping = true;
        StartCoroutine(AnimationTimer(clipDictionary.GetValueOrDefault("WallJump")));
    }
    public void SetRunTurnAnimation()
    {
        if (!playerManager.IsGrounded || isTurning)
            return;

        if (isRunStopping)
            isRunStopping = false;

        isTurning = true;
        StartCoroutine(AnimationTimer(clipDictionary.GetValueOrDefault("Turn")));
    }

    public void SetRunStopAnimation()
    {
        if (!playerManager.IsGrounded || isTurning || isDashing)
            return;

        isRunStopping = true;
        StartCoroutine(AnimationTimer(clipDictionary.GetValueOrDefault("RunStop")));

    }

    public void SetJumpAnimation()
    {
        SetTurnBoolFalse();
        isRunStopping = false;
    }

    public void SetAttackAnimation()
    {
        int rng;
        rng = Random.Range(1, 3);
        if (rng == 1)
        {
            animator.SetInteger("Attack", 1);
            StartCoroutine(AnimationTimer(clipDictionary.GetValueOrDefault("FrontAttack1")));
        }
        else if (rng == 2)
        {
            animator.SetInteger("Attack", 2);
            StartCoroutine(AnimationTimer(clipDictionary.GetValueOrDefault("FrontAttack2")));
        }
    }

    public void SetDashAnimation()
    {
        if (isDashing)
            return;

        StopAllCoroutines();
        ResetAnimations();
        isDashing = true;

        StartCoroutine(AnimationTimer(clipDictionary.GetValueOrDefault("Dash")));

    }


    public void SetTurnBoolFalse()
    {
        isTurning = false;
    }

    public void SetRunStopTrue()
    {

    }

    public void SetRunStopFalse()
    {
        playerManager.Rb.velocity = new Vector2(0, playerManager.Rb.velocity.y);
        isRunStopping = false;
    }

    private IEnumerator AnimationTimer(float time)
    {
        if (isTimerRunning)
            yield return null;

        isTimerRunning = true;
        yield return new WaitForSeconds(time);
        isWallJumping = false;
        isDashing = false;
        isRunStopping = false;
        isTurning = false;
        willDoHardLanding = false;
        isWallContactComplete = true;
        isTimerRunning = false;
        animator.SetInteger("Attack", 0);
    }

    public IEnumerator HardLandingCoroutine()
    {
        willDoHardLanding = false;
        yield return new WaitForSecondsRealtime(minTimeToInAirToDoHardLanding);
        willDoHardLanding = true;
    }
    private IEnumerator RemovePlayerInput(float time)
    {
        HardLandingFeedBack.PlayFeedbacks();
        playerManager.DoesPlayerHaveControl = false;
        yield return new WaitForSecondsRealtime(time);
        playerManager.DoesPlayerHaveControl = true;
    }

    private void ResetAnimations()
    {
        isWallJumping = false;
        isDashing = false;
        isRunStopping = false;
        isTurning = false;
        willDoHardLanding = false;
        isWallContactComplete = true;
        isTimerRunning = false;
    }
}
