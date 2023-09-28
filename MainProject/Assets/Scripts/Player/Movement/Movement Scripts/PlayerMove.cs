using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Unity.VisualScripting;

namespace WibertStudio
{
    [RequireComponent(typeof(PlayerManager))]
    public class PlayerMove : MonoBehaviour
    {
        private PlayerManager playerManager;
        private PlayerAnimator animator;
        private Player player;

        [SerializeField] private ParticleSystem playerDustParticles;


        #region Movement variables
        [Header("Movement Variables")]
        [Tooltip("Base move speed of the player")]
        [SerializeField] private float baseMoveSpeed;
        [SerializeField] private float moveAttackSpeed;
        [SerializeField] private float moveSpeedAttackSlowDuration;
        [SerializeField] private float acceleration;
        [SerializeField] private float groundDecceleration;
        [SerializeField] private float airDecceleration;
        [SerializeField] private float velPower;
        [SerializeField] private float groundFriction;
        [SerializeField] private float airFriction;

        public float initialMoveSpeed;
        private float deceleration;
        private float friction;
        public float horizontalInput { get; set; }
        public float moveSpeed { get; set; }
        #endregion
        public bool isAttackMoveSlowActive { get; set; }
        public bool IsCoroutineActive { get; set; }

        private void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            animator = GetComponent<PlayerAnimator>();

            player =  playerManager.Player;

            moveSpeed = baseMoveSpeed;
            initialMoveSpeed = baseMoveSpeed;
        }
        public void SetGroundValues()
        {
            deceleration = groundDecceleration;
            friction = groundFriction;
        }
        public void SetAirValues()
        {
            deceleration = airDecceleration;
            friction = airFriction;
        }
        public void Update()
        {
            PlayerInput();
            //Particles
            if (playerManager.Rb.velocity.x > 0 && playerManager.Rb.velocity.x < .5f && playerManager.IsGrounded)
                playerDustParticles.Play();
        }

        private void PlayerInput()
        {
            if (!playerManager.DoesPlayerHaveControl)
                return;

            horizontalInput = player.GetAxis("Move Horizontal");
        }

        public void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            if (!playerManager.DoesPlayerHaveControl)
                return;

            float targetSpeed = horizontalInput * moveSpeed;
            float speedDif = targetSpeed - playerManager.Rb.velocity.x;

            float accelRate = (Mathf.Abs(targetSpeed) > .01f) ? acceleration : deceleration;

            float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

            playerManager.Rb.AddForce(movement * Vector2.right);

            if (playerManager.Rb.velocity.x > .01f || playerManager.Rb.velocity.x < -.01f && playerManager.IsGrounded)
            {
                if (!IsCoroutineActive)
                {
                    IsCoroutineActive = true;
                }
            }

            // Friction
            if (Mathf.Abs(horizontalInput) == 0)
            {
                float amount = Mathf.Min(Mathf.Abs(playerManager.Rb.velocity.x), Mathf.Abs(friction));

                amount *= Mathf.Sign(playerManager.Rb.velocity.x);

                playerManager.Rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
            }
        }

        public void ChangeMoveSpeed(float newMoveSpeed)
        {
            baseMoveSpeed = newMoveSpeed;
        }

        public IEnumerator AttackSlow()
        {
            if (!playerManager.IsGrounded)
                yield break;

            isAttackMoveSlowActive = true;
            print("here");
            moveSpeed = moveAttackSpeed;
            yield return new WaitForSecondsRealtime(moveSpeedAttackSlowDuration);
            moveSpeed = baseMoveSpeed;
            isAttackMoveSlowActive = false;
        }

        public void ResetMoveSpeed()
        {
            baseMoveSpeed = initialMoveSpeed;
        }

    }
}
