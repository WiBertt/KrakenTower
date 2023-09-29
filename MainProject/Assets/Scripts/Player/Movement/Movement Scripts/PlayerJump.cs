using MoreMountains.Feedbacks;
using Rewired;
using Sirenix.OdinInspector;
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
        private Player player;

        [SerializeField] private MMF_Player jumpFeedback;
        #region Jump variables
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("The amount of force that is applied to the player when jumping")]
        [SerializeField] private float jumpForce = 17;
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("Max number of available jumps before landing. If above 1 can jump in air")]
        [SerializeField] private int maxNumberOfJumps = 1;
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("Grace time after leaving the grounded state that the player can still jump")]
        [SerializeField] private float CoyoteTime = .1f;
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("Grace time after pressing jump while in air that will still register as a jump when entering grounded state")]
        [SerializeField] private float jumpBufferDuration = .2f;
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("If true will apply the fall force on releasing jump input")]
        public bool ApplyForceOnJumpRelease;


        #endregion

        #region Apex modifier variables
        [BoxGroup("Apex Modifier Parameters")]
        [PropertyTooltip("If true will allow the use of an apex modifier. An apex modifier is a time at the apex of a jump that forces can be applied")]
        public bool HasApexModifier;
        [BoxGroup("Apex Modifier Parameters")]
        [PropertyTooltip("The time the apex modifier will last for")]
        [SerializeField] private float apexModifierTime = .03f;
        [BoxGroup("Apex Modifier Parameters")]
        [PropertyTooltip("What the gravity will be set too while in the apex modifier")]
        [SerializeField] private float apexModifierGravityScale = 0;
        [BoxGroup("Apex Modifier Parameters")]
        [PropertyTooltip("What the move speed will be set to while in the apex modifier")]
        [SerializeField] private float apexModifierMoveSpeed = 14;
        private float apexModifierTimer;
        private bool isApexModifierComplete = false;
        #endregion

        #region Logic / Debug Variables
        private bool hasMultipleJumps()
        {
            if (maxNumberOfJumps > 1)
                return true;
            else
                return false;
        }
        private bool canJump()
        {
            if (IsJumpPressed && !DoesJumpNeedToBePressedAgain && availableJumps > 0 && CoyoteJump)
                return true;
            else
                return false;
        }
        private float initialJumpForce;
        [FoldoutGroup("Debug")]
        [ReadOnly]
        [ShowInInspector]
        [Header("Jump")]
        private int availableJumps;
        [FoldoutGroup("Debug")]
        [ReadOnly]
        public bool IsJumpPressed;
        [HideInInspector]
        public bool WasJumpPressed;
        [FoldoutGroup("Debug")]
        [ReadOnly]
        public bool DoesJumpNeedToBePressedAgain;
        [FoldoutGroup("Debug")]
        [ReadOnly]
        public bool HasJumped;
        [FoldoutGroup("Debug")]
        [ReadOnly]
        [Header("Coyote")]
        public float CoyoteTimer;
        [FoldoutGroup("Debug")]
        [ReadOnly]
        public bool CoyoteJump;
        [FoldoutGroup("Debug")]
        [ReadOnly]
        [Header("Jump Buffer")]
        public bool IsJumpBufferActive = false;
        [FoldoutGroup("Debug")]
        [ReadOnly]
        public float JumpBufferTimer;
        private bool isJumpBufferTimerActive;
        [HideInInspector]
        public bool IsApexModifierComplete;
        [FoldoutGroup("Debug")]
        [ReadOnly]
        [Header("Apex Modifier")]
        public bool isInApexModifier;
        #endregion

        private void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            player = playerManager.Player;
        }

        public void ResetJumpAttributes()
        {
            if (maxNumberOfJumps > 0)
            {
                if (IsJumpPressed)
                    DoesJumpNeedToBePressedAgain = true;

                availableJumps = maxNumberOfJumps;
                HasJumped = false;
            }

            if (playerManager.IsGrounded && IsJumpBufferActive && availableJumps > 0)
                Jump();

            ResetCoyoteTimer();
        }

        public void ResetApexModifierAttributes()
        {
            if (HasApexModifier)
            {
                apexModifierTimer = apexModifierTime;
                isApexModifierComplete = false;
            }
        }

        public void Update()
        {
            if (!playerManager.IsGrounded && CoyoteJump)
                HandleCoyoteTime();

            PlayerInput();
            HandleJump();
            HandleApexModifier();
            HandleJumpBuffer();
        }

        private void PlayerInput()
        {
            if (player.GetButtonDown("Jump"))
                IsJumpPressed = true;
            else if (player.GetButtonUp("Jump"))
            {
                IsJumpPressed = false;
                DoesJumpNeedToBePressedAgain = false;
            }
        }

        // determines if the player is able to jump
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
            availableJumps--;
            PlayFX();

            // set values
            playerManager.SetGravity(playerManager.BaseGravityScale);
            ResetApexModifier();
            playerManager.PlayerMove.ResetMoveSpeed();
            DoesJumpNeedToBePressedAgain = true;
            HasJumped = true;
            if (HasApexModifier)
                apexModifierTimer = apexModifierTime;
        }

        private void PlayFX()
        {
            jumpFeedback.PlayFeedbacks();
        }

        private void HandleApexModifier()
        {
            if (HasApexModifier && !isApexModifierComplete && playerManager.Rb.velocity.y < 1.5 && HasJumped)
                ApexModifier();
        }

        private void ApexModifier()
        {
            // in apex modifier
            if (apexModifierTimer > 0 && !playerManager.PlayerMove.isAttackMoveSlowActive)
            {
                isInApexModifier = true;
                apexModifierTimer -= Time.deltaTime;
                playerManager.Rb.gravityScale = apexModifierGravityScale;
                playerManager.Rb.velocity = new Vector2(playerManager.Rb.velocity.x, 0);
                playerManager.PlayerMove.ChangeMoveSpeed(apexModifierMoveSpeed);
                isApexModifierComplete = false;
            }
            // out of apex modifier
            else if (apexModifierTimer < 0)
            {
                isInApexModifier = false;
                playerManager.Rb.gravityScale = playerManager.FallGravityScale;
                playerManager.Rb.velocity = new Vector2(playerManager.Rb.velocity.x, playerManager.Rb.velocity.y);
                playerManager.PlayerMove.ResetMoveSpeed();
                isApexModifierComplete = true;
            }
        }

        private void HandleJumpBuffer()
        {
            if (IsJumpPressed && !DoesJumpNeedToBePressedAgain)
            {
                JumpBufferTimer = jumpBufferDuration;
                WasJumpPressed = true;
                DoesJumpNeedToBePressedAgain = true;
            }

            if (WasJumpPressed)
                JumpBuffer();
        }

        private void JumpBuffer()
        {
            if (JumpBufferTimer > 0)
            {
                IsJumpBufferActive = true;
                JumpBufferTimer -= Time.deltaTime;
            }
            else if (JumpBufferTimer <= 0)
                IsJumpBufferActive = false;
        }

        private void HandleCoyoteTime()
        {
            if (!hasMultipleJumps() && CoyoteTimer > 0)
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

        private void ResetCoyoteTimer()
        {
            if (!hasMultipleJumps())
                CoyoteTimer = CoyoteTime;
        }

        private void ResetApexModifier()
        {
            apexModifierTimer = apexModifierTime;
            isApexModifierComplete = false;
        }
    }
}