using UnityEngine;

namespace WibertStudio
{
    /// <summary>
    /// This handles all possible scripts that can be active in the grounded state
    /// Be sure to add the proper functions for the scripts you add
    /// </summary>
    public class PlayerGroundedState : PlayerBaseState
    {
        public override void EnterState(PlayerManager ctx)
        {
            context = ctx;

            context.PlayerMove.EnterState();
            context.PlayerMove.SetGroundValues();
            context.PlayerJump.EnterState();
            context.PlayerAnimator.StopCoroutine("HardLandingCoroutine");
            context.PlayerAnimator.CheckLandAnimation();

            //Things that need to be reset when first entering grounded state
            context.PlayerJump.EnterState();
            context.PlayerJump.ResetJumpAttributes();
            context.PlayerJump.CoyoteJump = true;

            SetGravity("up");
            ResetCoyoteTimer();

            if (context.PlayerJump.IsJumpBufferActive && context.PlayerJump.AvailableJumps > 0)
                context.PlayerJump.Jump();
        }

        private void ResetCoyoteTimer()
        {
            if (!context.PlayerJump.HasMultipleJumps)
                context.PlayerJump.CoyoteTimer = context.PlayerJump.CoyoteTime;
        }

        public override void UpdateState()
        {
            context.PlayerMove.UpdateState();
            context.PlayerJump.UpdateState();
        }

        public override void FixedUpdateState()
        {
            context.PlayerMove.FixedUpdateState();
            context.PlayerJump.FixedUpdateState();
        }

        public override void ExitState()
        {
            context.PlayerMove.ExitState();
            context.PlayerJump.ExitState();
        }

        public override void SwitchConditions()
        {
            if (!context.IsGrounded)
                context.SwitchState(context.inAirState);
        }

        private void SetGravity(string direction)
        {
            switch (direction)
            {
                case ("up"):
                    context.Rb.gravityScale = context.BaseGravityScale;
                    break;
                case ("down"):
                    context.Rb.gravityScale = context.FallGravityScale;
                    break;
            }

        }
    }
}