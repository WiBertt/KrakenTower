using UnityEngine;

namespace WibertStudio
{
    /// <summary>
    /// This handles all possible scripts that can be active in the in air state
    /// Be sure to add the proper functions for the scripts you add
    /// </summary>
    public class PlayerInAirState : PlayerBaseState
    {
        public override void EnterState(PlayerManager ctx)
        {
            context = ctx;
            context.PlayerJump.IsJumpBufferActive = false;

            context.PlayerMove.EnterState();
            context.PlayerMove.SetAirValues();
            context.PlayerJump.EnterState();
            context.PlayerWallSlide.EnterState();
            context.PlayerAnimator.StartCoroutine("HardLandingCoroutine");

        }

        public override void UpdateState()
        {
            context.PlayerMove.UpdateState();
            context.PlayerJump.UpdateState();
            context.PlayerWallSlide.UpdateState();

            HandleJumpBuffer();
            FallVelocityClamp();
            HandleCoyoteTime();
        }

        private void HandleCoyoteTime()
        {
            if (!context.PlayerJump.HasMultipleJumps && context.PlayerJump.CoyoteTimer > 0)
                CoyoteTimeClock();
            else if (context.PlayerJump.CoyoteTimer < 0)
                context.PlayerJump.CoyoteJump = false;
        }

        private void CoyoteTimeClock()
        {
            if (context.PlayerJump.CoyoteTimer > 0)
            {
                context.PlayerJump.CoyoteTimer -= Time.deltaTime;
                context.PlayerJump.CoyoteJump = true;
            }
        }

        private void HandleJumpBuffer()
        {
            if (context.PlayerJump.IsJumpPressed && !context.PlayerJump.DoesJumpNeedToBePressedAgain)
            {
                context.PlayerJump.JumpBufferTimer = context.PlayerJump.JumpBufferDuration;
                context.PlayerJump.WasJumpPressed = true;
                context.PlayerJump.DoesJumpNeedToBePressedAgain = true;
            }

            if (context.PlayerJump.WasJumpPressed)
            {
                JumpBuffer();
            }
        }

        private void JumpBuffer()
        {
            if (context.PlayerJump.JumpBufferTimer > 0)
            {
                context.PlayerJump.IsJumpBufferActive = true;
                context.PlayerJump.JumpBufferTimer -= Time.deltaTime;
            }
            else if (context.PlayerJump.JumpBufferTimer <= 0)
            {
                context.PlayerJump.IsJumpBufferActive = false;
            }
        }

        private void FallVelocityClamp()
        {
            if (context.Rb.velocity.y < context.MaxFallSpeed)
                context.Rb.velocity = new Vector2(context.Rb.velocity.x, context.MaxFallSpeed);
        }

        public override void FixedUpdateState()
        {
            context.PlayerMove.FixedUpdateState();
            context.PlayerJump.FixedUpdateState();
            context.PlayerWallSlide.FixedUpdateState();
            CheckYVelocity();
        }

        public override void ExitState()
        {
            context.PlayerMove.ExitState();
            context.PlayerJump.ExitState();
            context.PlayerWallSlide.ExitState();
        }

        public override void SwitchConditions()
        {
            if (context.IsGrounded)
                context.SwitchState(context.groundedState);
        }

        private void CheckYVelocity()
        {
            if (context.Rb.velocity.y < 0 || context.PlayerJump.IsJumpPressed && context.PlayerJump.HasJumped && context.PlayerJump.ApplyForceOnJumpRelease && !context.PlayerJump.HasApexModifier || !context.PlayerJump.IsJumpPressed && context.PlayerJump.HasJumped && context.PlayerJump.ApplyForceOnJumpRelease && !context.PlayerJump.IsApexModifierComplete)
                SetGravity("down");
        }

        private void SetGravity(string direction)
        {
            switch (direction)
            {
                case "up":
                    context.Rb.gravityScale = context.BaseGravityScale;
                    break;
                case "down":
                    context.Rb.gravityScale = context.FallGravityScale;
                    break;
            }
        }
    }
}