using MoreMountains.Feedbacks;
using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WibertStudio
{
    [RequireComponent(typeof(PlayerManager))]
    public class PlayerJump : MonoBehaviour
    {
        private PlayerManager playerManager;
        private PlayerMove playerMove;
        private PlayerAnimator playerAnimator;

        // Rewired
        private Player player;

        [SerializeField] private MMF_Player jumpFeedback;
        #region Jump variables
        [Header("Jump Variables")]
        [Tooltip("The amount of force that is applied to the player when jumping")]
        [SerializeField] private float jumpForce = 17;
        [Tooltip("Max number of available jumps before landing. If above 1 can jump in air")]
        [SerializeField] private int maxNumberOfJumps = 1;
        [Tooltip("Grace time after leaving the grounded state that the player can still jump")]
        [Range(0, 2)]
        [SerializeField] private float coyoteTime = .1f;
        [Tooltip("Grace time after pressing jump while in air that will still register a jump when entering grounded state")]
        [Range(2, -2)]
        [SerializeField] private float jumpBufferDuration = .2f;
        [Tooltip("If true will apply the fall force on releasing jump input")]
        [SerializeField] private bool applyForceOnJumpRelease = true;

        private bool canJump()
        {
            if (isJumpPressed && !doesJumpNeedToBePressedAgain && availableJumps > 0 && CoyoteJump)
                return true;
            else
                return false;
        }
        private float initialJumpForce;
        private int availableJumps;
        private float coyoteTimer;
        private bool isJumpPressed;
        private bool doesJumpNeedToBePressedAgain;
        private bool hasJumped;
        private bool wasJumpPressed = false;
        private bool hasMultipleJumps()
        {
            if (maxNumberOfJumps > 1)
                return true;
            else
                return false;
        }
        private bool isJumpBufferActive = false;
        private float jumpBufferTimer;
        private bool isJumpBufferTimerActive;
        [Space()]
        #endregion
        #region Apex modifier variables
        [Header("Apex Modifier Variables")]
        [Tooltip("If true will allow the use of an apex modifier. An apex modifier is a time at the apex of a jump that forces can be applied")]
        [SerializeField] private bool hasApexModifier = true;
        [Tooltip("The time the apex modifier will last for")]
        [Range(0, 2)]
        [SerializeField] private float apexModifierTime = .03f;
        [Tooltip("The amount of gravity to apply while in the apex modifier timer")]
        [SerializeField] private float apexModifierGravityScale = 0;
        [Tooltip("The amount of move speed to set to the base move speed")]
        [SerializeField] private float apexModifierMoveSpeedIncrease = 14;
        private float apexModifierTimer;
        private bool isApexModifierComplete = false;
        #endregion
        #region Getters/Setters
        public bool IsJumpPressed { get { return isJumpPressed; } }
        public bool WasJumpPressed { get { return wasJumpPressed; } set { wasJumpPressed = value; } }
        public bool DoesJumpNeedToBePressedAgain { get { return doesJumpNeedToBePressedAgain; } set { doesJumpNeedToBePressedAgain = value; } }
        public bool HasJumped { get { return hasJumped; } }
        public bool ApplyForceOnJumpRelease { get { return applyForceOnJumpRelease; } }
        public bool HasApexModifier { get { return hasApexModifier; } }
        public bool IsApexModifierComplete { get { return isApexModifierComplete; } }
        public bool HasMultipleJumps { get { return hasMultipleJumps(); } }
        public float CoyoteTime { get { return coyoteTime; } }
        public float CoyoteTimer { get { return coyoteTimer; } set { coyoteTimer = value; } }
        public bool CoyoteJump { get; set; }

        public bool IsJumpBufferActive { get { return isJumpBufferActive; } set { isJumpBufferActive = value; } }
        public float JumpBufferTimer { get { return jumpBufferTimer; } set { jumpBufferTimer = value; } }
        public float AvailableJumps { get { return availableJumps; } }
        public float JumpBufferDuration { get { return jumpBufferDuration; } }

        #endregion

        private void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            playerMove = GetComponent<PlayerMove>();
            playerAnimator = GetComponent<PlayerAnimator>();

            player = playerManager.Player;
        }

        public void ResetJumpAttributes()
        {
            if (maxNumberOfJumps > 0)
            {
                if (isJumpPressed)
                    doesJumpNeedToBePressedAgain = true;

                availableJumps = maxNumberOfJumps;
                hasJumped = false;
            }

            if (playerManager.IsGrounded && IsJumpBufferActive && AvailableJumps > 0)
                Jump();

            ResetCoyoteTimer();
        }

        public void ResetApexModifierAttributes()
        {
            if (hasApexModifier)
            {
                apexModifierTimer = apexModifierTime;
                isApexModifierComplete = false;
            }
        }

        public void Update()
        {
            PlayerInput();
            HandleJump();
            HandleApexModifier();
            HandleJumpBuffer();

            if(!playerManager.IsGrounded && CoyoteJump)
                HandleCoyoteTime();
        }

        private void PlayerInput()
        {
            if (player.GetButtonDown("Jump"))
            {
                isJumpPressed = true;
            }
            else if (player.GetButtonUp("Jump"))
            {
                isJumpPressed = false;
                doesJumpNeedToBePressedAgain = false;
            }
        }

        private void HandleJump()
        {
            if (!playerManager.DoesPlayerHaveControl)
                return;
            if (canJump())
                Jump();

            
        }

        public void Jump()
        {
            playerManager.Rb.velocity = new Vector2(playerManager.Rb.velocity.x, jumpForce);
            playerManager.PlayerMove.ResetMoveSpeed();
            availableJumps--;
            doesJumpNeedToBePressedAgain = true;
            hasJumped = true;
            PlayFX();
            playerManager.SetGravity(playerManager.BaseGravityScale);
            ResetApexModifier();
            print("Jump");
            if (hasApexModifier)
                apexModifierTimer = apexModifierTime;
        }

        public void FixedUpdate()
        {

        }

        private void HandleJumpBuffer()
        {
            if (IsJumpPressed && !DoesJumpNeedToBePressedAgain)
            {
                JumpBufferTimer =   JumpBufferDuration;
                WasJumpPressed = true;
                DoesJumpNeedToBePressedAgain = true;
            }

            if (WasJumpPressed)
            {
                JumpBuffer();
            }
        }

        private void JumpBuffer()
        {
            if (JumpBufferTimer > 0)
            {
                IsJumpBufferActive = true;
                JumpBufferTimer -= Time.deltaTime;
            }
            else if (JumpBufferTimer <= 0)
            {
                IsJumpBufferActive = false;
            }
        }

        private void ResetCoyoteTimer()
        {
            if (!HasMultipleJumps)
                CoyoteTimer = CoyoteTime;
        }

        private void HandleCoyoteTime()
        {
            if (!HasMultipleJumps && CoyoteTimer > 0)
                CoyoteTimeClock();
            else if (CoyoteTimer < 0)
                CoyoteJump = false;
        }

        private void CoyoteTimeClock()
        {
            if (CoyoteTimer > 0)
            {
                CoyoteTimer -= Time.deltaTime;
                CoyoteJump = true;
            }
        }
        private void HandleApexModifier()
        {
            if (hasApexModifier && !isApexModifierComplete && playerManager.Rb.velocity.y < 1.5 && hasJumped)
            {
                ApexModifier();
            }
        }

        private void ApexModifier()
        {
            if (apexModifierTimer > 0 && !playerManager.PlayerMove.isAttackMoveSlowActive)
            {
                apexModifierTimer -= Time.deltaTime;
                isApexModifierComplete = false;
                playerManager.Rb.gravityScale = apexModifierGravityScale;
                playerManager.Rb.velocity = new Vector2(playerManager.Rb.velocity.x, 0);
                playerMove.ChangeMoveSpeed(apexModifierMoveSpeedIncrease);
            }
            else if (apexModifierTimer < 0)
            {
                playerManager.Rb.gravityScale = playerManager.FallGravityScale;
                playerManager.Rb.velocity = new Vector2(playerManager.Rb.velocity.x, playerManager.Rb.velocity.y);
                playerMove.ResetMoveSpeed();
                isApexModifierComplete = true;
            }

        }

        private void ResetApexModifier()
        {
            apexModifierTimer = apexModifierTime;
            isApexModifierComplete = false;
        }


        private void PlayFX()
        {
            jumpFeedback.PlayFeedbacks();
        }
    }
}