using DG.Tweening;
using MoreMountains.Feedbacks;
using Rewired;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WibertStudio
{
    public class PlayerWallSlide_WallJump : MonoBehaviour
    {
        // References
        private PlayerManager playerManager;
        private Player player;

        [SerializeField] private MMF_Player wallSlideFeedbacks;
        [SerializeField] private MMF_Player wallJumpFeedbacks;

        #region Base parameters
        // Base parameters
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("How long does the player have to be in the air before allowing the ability to wall slide")]
        [SerializeField] private float minTimeToBeInAirToWallSlide;
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("How fast does the player slide down a wall")]
        [SerializeField] private float wallSlideSpeed;
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("How long to remove player input after doing a wall jump")]
        [SerializeField] private float timeToRemovePlayerInput;
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("How long does it take to get back to your normal movement speed after a wall jump")]
        [SerializeField] private float timeToResetMoveSpeed;
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("Grace period after pressing jump button that the game will still register a jump")]
        [SerializeField] private float wallJumpBufferTimer;
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("The amount of force applied on the x axis after a wall jump")]
        [SerializeField] private float xForceOnJump;
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("The amount of force applied on the y axis after a wall jump")]
        [SerializeField] private float yForceOnJump;
        #endregion

        #region Logic / Debug Variables
        // Logic / Debug variables
        [FoldoutGroup("Debug")]
        [ReadOnly]
        [ShowInInspector]
        private bool canSlideOnWall;
        [FoldoutGroup("Debug")]
        [ReadOnly]
        public bool isSlidingOnWall;
        [FoldoutGroup("Debug")]
        [ReadOnly]
        [ShowInInspector]
        private bool isWallJumpBufferActive;
        [FoldoutGroup("Debug")]
        [ReadOnly]
        [ShowInInspector]
        private bool wasWallJumpPressed;
        [FoldoutGroup("Debug")]
        [ReadOnly]
        [ShowInInspector]
        private bool isWallJumpComplete = true;
        #endregion

        private void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            player = playerManager.Player;
        }

        public void Update()
        {
            if (PlayerManager.instance.IsGrounded)
                return;

            PlayerInput();

            // sets gravity for wall slide
            if (isSlidingOnWall)
                PlayerManager.instance.SetGravity(PlayerManager.instance.BaseGravityScale);

          
        }

        private void PlayerInput()
        {
            // Starts jump buffer coroutine
            if (player.GetButtonDown("Jump"))
            {
                StopCoroutine(WallJumpBufferCoroutine());
                StartCoroutine(WallJumpBufferCoroutine());
            }

            // checks if able to do wall jump and executes if able
            if ((PlayerManager.instance.IsOnTopLeftWall && PlayerManager.instance.IsOnBottomLeftWall || PlayerManager.instance.IsOnTopRightWall && PlayerManager.instance.IsOnBottomRightWall))
                if (isWallJumpBufferActive && isWallJumpComplete)
                    WallJump();
        }

        private IEnumerator WallJumpBufferCoroutine()
        {
            isWallJumpBufferActive = true;
            yield return new WaitForSecondsRealtime(wallJumpBufferTimer);
            isWallJumpBufferActive = false;
        }

        private void WallJump()
        {
            // set up for wall jump
            PlayerManager.instance.SetGravity(PlayerManager.instance.BaseGravityScale);
            isWallJumpBufferActive = false;
            isWallJumpComplete = false;
            wasWallJumpPressed = true;
            StopCoroutine(StopPlayerInputForWallJump());
            StartCoroutine(StopPlayerInputForWallJump());
            PlayerManager.instance.PlayerAnimator.SetWallJumpAnimation();
           
            // manually flips the character to the right facing direction and executes jump
            if (PlayerManager.instance.IsOnRightWall)
            {
                if (!PlayerManager.instance.IsFacingRight)
                    PlayerManager.instance.ManualFlip();

                PlayerManager.instance.Rb.velocity = (new Vector2(-xForceOnJump, yForceOnJump));
                wallJumpFeedbacks.PlayFeedbacks();
            }
            else if (PlayerManager.instance.IsOnLeftWall)
            {
                if (PlayerManager.instance.IsFacingRight)
                    PlayerManager.instance.ManualFlip();

                PlayerManager.instance.Rb.velocity = (new Vector2(xForceOnJump, yForceOnJump));
                wallJumpFeedbacks.PlayFeedbacks();
            }
            
            isWallJumpComplete = true;
        }

        private IEnumerator StopPlayerInputForWallJump()
        {
            // set up for input loss
            DOTween.CompleteAll();
            PlayerManager.instance.PlayerMove.currentMoveSpeed = 0;
            PlayerManager.instance.DoesPlayerHaveControl = false;

            // gradually restorers player movement controls
            DOTween.To(() => PlayerManager.instance.PlayerMove.currentMoveSpeed, x => PlayerManager.instance.PlayerMove.currentMoveSpeed = x, PlayerManager.instance.PlayerMove.initialMoveSpeed, timeToResetMoveSpeed);
            yield return new WaitForSecondsRealtime(timeToRemovePlayerInput);

            PlayerManager.instance.DoesPlayerHaveControl = true;
        }

        public void FixedUpdate()
        {
            if (!canSlideOnWall || PlayerManager.instance.IsGrounded)
                return;

            // sets gravity and acitvates wall slide feedback if wall sliding
            if (PlayerManager.instance.IsOnTopLeftWall && PlayerManager.instance.IsOnBottomLeftWall && playerManager.PlayerMove.horizontalInput < 0 || PlayerManager.instance.IsOnTopRightWall && PlayerManager.instance.IsOnBottomRightWall && playerManager.PlayerMove.horizontalInput > 0)
            {
                isSlidingOnWall = true;
                if (!wasWallJumpPressed)
                {
                    wallSlideFeedbacks.PlayFeedbacks();
                    PlayerManager.instance.Rb.velocity = Vector2.ClampMagnitude(PlayerManager.instance.Rb.velocity, wallSlideSpeed);
                }

            }
            // stops wall slide feedback and resets appropriate variables
            else
            {
                wallSlideFeedbacks.StopFeedbacks();
                isSlidingOnWall = false;
                wasWallJumpPressed = false;
            }
        }

        public void ResetWallSlideAttributes()
        {
            isSlidingOnWall = false;
            canSlideOnWall = false;
            wallSlideFeedbacks.StopFeedbacks();
        }

        private IEnumerator WallSlideCountDown()
        {
            canSlideOnWall = false;
            yield return new WaitForSecondsRealtime(minTimeToBeInAirToWallSlide);
            canSlideOnWall = true;
        }
    }
}
