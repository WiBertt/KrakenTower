using DG.Tweening;
using MoreMountains.Feedbacks;
using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WibertStudio
{
    public class PlayerWallSlideState : MonoBehaviour, IStateAccessable
    {
        // References
        private PlayerMove playerMove;
        private PlayerAnimator playerAnimator;

        // Rewired
        [SerializeField] private int playerID = 0;
        [SerializeField] private Player player;
        [SerializeField] private MMF_Player wallSlideFeedbacks;
        [SerializeField] private MMF_Player wallJumpFeedbacks;

        [SerializeField] private float minTimeToBeInAirToWallSlide;
        [SerializeField] private float timeToRemovePlayerInput;
        [SerializeField] private float timeToResetMoveSpeed;
        [SerializeField] private float wallJumpBufferTimer;
        [SerializeField] private float wallSlideSpeed;
        [SerializeField] private float xForceOnJump;
        [SerializeField] private float yForceOnJump;

        private bool canSlideOnWall;
        public bool isSlidingOnWall { get; set; }
        private bool isWallJumpBufferActive;
        private bool wasWallJumpPressed;
        private bool isWallJumpComplete = true;

        private void Start()
        {
            playerMove = GetComponent<PlayerMove>();
            playerAnimator = GetComponent<PlayerAnimator>();
            player = ReInput.players.GetPlayer(playerID);
        }

        public void EnterState()
        {
            StartCoroutine(WallSlideCountDown());
        }

        public void UpdateState()
        {
            if (isSlidingOnWall)
            {
                PlayerManager.instance.Rb.gravityScale = PlayerManager.instance.BaseGravityScale;
            }
            // Starts jump buffer coroutine
            if (player.GetButtonDown("Jump"))
            {
                StopCoroutine(WallJumpBufferCoroutine());
                StartCoroutine(WallJumpBufferCoroutine());
            }

            if ((PlayerManager.instance.IsOnTopLeftWall && PlayerManager.instance.IsOnBottomLeftWall || PlayerManager.instance.IsOnTopRightWall && PlayerManager.instance.IsOnBottomRightWall))
                if (isWallJumpBufferActive && isWallJumpComplete)
                    WallJump();
        }

        private void WallJump()
        {
            PlayerManager.instance.Rb.gravityScale = 1;
            isWallJumpBufferActive = false;
            isWallJumpComplete = false;
            wasWallJumpPressed = true;
            playerAnimator.SetWallJumpAnimation();
            StopCoroutine(StopPlayerInputForWallJump());
            StartCoroutine(StopPlayerInputForWallJump());
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
            playerAnimator.StartCoroutine("HardLandingCoroutine");
            playerAnimator.StopCoroutine("HardLandingCoroutine");
            isWallJumpComplete = true;
        }

        private IEnumerator WallJumpBufferCoroutine()
        {
            isWallJumpBufferActive = true;
            yield return new WaitForSecondsRealtime(wallJumpBufferTimer);
            isWallJumpBufferActive = false;
        }

        private IEnumerator StopPlayerInputForWallJump()
        {
            DOTween.CompleteAll();
            playerMove.moveSpeed = 0;
            PlayerManager.instance.DoesPlayerHaveControl = false;
            DOTween.To(() => playerMove.moveSpeed, x => playerMove.moveSpeed = x, playerMove.initialMoveSpeed, timeToResetMoveSpeed);
            yield return new WaitForSecondsRealtime(timeToRemovePlayerInput);
            PlayerManager.instance.DoesPlayerHaveControl = true;
        }

        public void FixedUpdateState()
        {
            if (!canSlideOnWall)
                return;

            if (PlayerManager.instance.IsOnTopLeftWall && PlayerManager.instance.IsOnBottomLeftWall && playerMove.horizontalInput < 0 || PlayerManager.instance.IsOnTopRightWall && PlayerManager.instance.IsOnBottomRightWall && playerMove.horizontalInput > 0)
            {
                isSlidingOnWall = true;
                if (!wasWallJumpPressed)
                {
                    wallSlideFeedbacks.PlayFeedbacks();
                    PlayerManager.instance.Rb.velocity = Vector2.ClampMagnitude(PlayerManager.instance.Rb.velocity, wallSlideSpeed);
                    playerAnimator.StartCoroutine("HardLandingCoroutine");
                    playerAnimator.StopCoroutine("HardLandingCoroutine");
                }

            }
            else
            {
                wallSlideFeedbacks.StopFeedbacks();
                isSlidingOnWall = false;
                wasWallJumpPressed = false;
            }
        }

        public void ExitState()
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
