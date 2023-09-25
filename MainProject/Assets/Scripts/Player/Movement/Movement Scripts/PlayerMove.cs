using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace WibertStudio
{
    [RequireComponent(typeof(PlayerManager))]
    public class PlayerMove : MonoBehaviour, IStateAccessable
    {
        private PlayerManager playerManager;
        private PlayerAnimator animator;

        [SerializeField] private ParticleSystem playerDustParticles;

        // Rewired
        [SerializeField] private int playerID = 0;
        [SerializeField] private Player player;

        #region Movement variables
        [Header("Movement Variables")]
        [Tooltip("Base move speed of the player")]
        [SerializeField] private float baseMoveSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private float groundDecceleration;
        [SerializeField] private float airDecceleration;
        [SerializeField] private float velPower;
        [SerializeField] private float groundFriction;
        [SerializeField] private float airFriction;
       
        [HideInInspector] public float initialMoveSpeed;
        private float deceleration;
        private float friction;
        public float horizontalInput { get; set; }
        public float moveSpeed { get; set; }
        #endregion

        public bool IsCoroutineActive { get; set; }
        
        private void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            animator = GetComponent<PlayerAnimator>();

            player = ReInput.players.GetPlayer(playerID);

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
        public void EnterState()
        {

        }

        public void UpdateState()
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

        public void FixedUpdateState()
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

        public void ResetMoveSpeed()
        {
            baseMoveSpeed = initialMoveSpeed;
        }

        public void ExitState()
        {

        }
    }
}
