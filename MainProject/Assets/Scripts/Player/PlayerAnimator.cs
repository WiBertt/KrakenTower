using MoreMountains.Feedbacks;
using Rewired;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WibertStudio;

public class PlayerAnimator : MonoBehaviour
{
    // references
    [BoxGroup("Base Parameters")]
    public Animator playerAnimator;
    private PlayerManager playerManager;
    private PlayerWallSlide_WallJump wallSlide_WallJump;
    private Player player;


    [BoxGroup("Base Parameters")]
    [PropertyTooltip("How fast the player has to be moving to do the stop animation")]
    [SerializeField] private float minSpeedToDoStopAnim;
    [BoxGroup("Base Parameters")]
    [PropertyTooltip("How fast the player has to be moving to do the turn animation")]
    [SerializeField] private float moveSpeedTurnThreshHold;

    [FoldoutGroup("Debug")]
    [ReadOnly]
    [ShowInInspector]
    private bool isTurning;
    [FoldoutGroup("Debug")]
    [ReadOnly]
    [ShowInInspector]
    private bool isRunStopping;
    [FoldoutGroup("Debug")]
    [ReadOnly]
    [ShowInInspector]
    private bool isDashing;
    [FoldoutGroup("Debug")]
    [ReadOnly]
    [ShowInInspector]
    private bool isMovingRight;
    [FoldoutGroup("Debug")]
    [ReadOnly]
    [ShowInInspector]
    private bool isWallJumping;
    [FoldoutGroup("Debug")]
    [ReadOnly]
    [ShowInInspector]
    private bool isWallContactComplete;
    [FoldoutGroup("Debug")]
    [ReadOnly]
    [ShowInInspector]
    private bool isAttacking;
    [FoldoutGroup("Debug")]
    [ReadOnly]
    [ShowInInspector]
    private bool isTimerRunning;
    [FoldoutGroup("Clip Dictionary")]
    [ReadOnly]
    [ShowInInspector]
    private Dictionary<string, float> clipDictionary = new Dictionary<string, float>();

    private bool isIdle()
    {
        if (Mathf.Abs(playerManager.Rb.velocity.x) < .01 && playerManager.IsGrounded)
            return true;
        else return false;
    }

    private void StoreAllClipNamesAndDurations()
    {
        foreach (AnimationClip ac in playerAnimator.runtimeAnimatorController.animationClips)
        {
            clipDictionary.Add(ac.name, ac.length);
        }
    }

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        player = playerManager.Player;
        wallSlide_WallJump = playerManager.PlayerWallSlide;

        StoreAllClipNamesAndDurations();
    }

    private void Update()
    {
        SetAnimationParameters();
        HandleTurnAnimation();
        
        // everything below this will not be set if you are doing any onf these animations
        if (isRunStopping || isTurning || isDashing)
            return;

        HandleStopAnimation();
        HandleWallContactAnimation();      
    }
    
    private void SetAnimationParameters()
    {
        playerAnimator.SetBool("IsIdle", isIdle());
        playerAnimator.SetFloat("XVelocity", Mathf.Abs(playerManager.Rb.velocity.x));
        playerAnimator.SetFloat("YVelocity", (playerManager.Rb.velocity.y));
        playerAnimator.SetBool("IsGrounded", playerManager.IsGrounded);
        playerAnimator.SetBool("IsRunStopping", isRunStopping);
        playerAnimator.SetBool("IsTurning", isTurning);
        playerAnimator.SetBool("IsDashing", isDashing);
        playerAnimator.SetBool("IsWallSliding", wallSlide_WallJump.isSlidingOnWall);
        playerAnimator.SetBool("IsWallContactComplete", isWallContactComplete);
        playerAnimator.SetBool("IsDoingWallJump", isWallJumping);
    }

    private void HandleTurnAnimation()
    {
        if (playerManager.Rb.velocity.x < -moveSpeedTurnThreshHold)
            isMovingRight = false;
        else if (playerManager.Rb.velocity.x > moveSpeedTurnThreshHold)
            isMovingRight = true;

        if (player.GetAxis("Move Horizontal") < -.01f && isMovingRight && playerManager.Rb.velocity.x > moveSpeedTurnThreshHold || player.GetAxis("Move Horizontal") > .01f && !isMovingRight && playerManager.Rb.velocity.x < -moveSpeedTurnThreshHold)
        {
            SetRunTurnAnimation();
            return;
        }
    }

    private void HandleStopAnimation()
    {
        if (!playerManager.IsGrounded || isTurning || isDashing)
            return;

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
    }

    private void HandleWallContactAnimation()
    {
        if (!wallSlide_WallJump.isSlidingOnWall)
            isWallContactComplete = false;
        if (wallSlide_WallJump.isSlidingOnWall && !isWallContactComplete)
            StartCoroutine(AnimationTimer(clipDictionary.GetValueOrDefault("WallContact")));       
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
        isRunStopping = true;
        StartCoroutine(AnimationTimer(clipDictionary.GetValueOrDefault("RunStop")));
    }

    /// <summary>
    /// setups jump animation to avoid errors
    /// </summary>
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
            playerAnimator.SetInteger("Attack", 1);
            StartCoroutine(AnimationTimer(clipDictionary.GetValueOrDefault("FrontAttack1")));
        }
        else if (rng == 2)
        {
            playerAnimator.SetInteger("Attack", 2);
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
        print("Hi");
        isTurning = false;
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
        isWallContactComplete = true;
        isTimerRunning = false;
        playerAnimator.SetInteger("Attack", 0);
    }

    private void ResetAnimations()
    {
        isWallJumping = false;
        isDashing = false;
        isRunStopping = false;
        isTurning = false;
        isWallContactComplete = true;
        isTimerRunning = false;
    }

    private IEnumerator RemovePlayerInput(float time)
    {
        playerManager.DoesPlayerHaveControl = false;
        yield return new WaitForSecondsRealtime(time);
        playerManager.DoesPlayerHaveControl = true;
    }
}
