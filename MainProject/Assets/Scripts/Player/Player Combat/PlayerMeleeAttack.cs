using Rewired;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WibertStudio
{
    public class PlayerMeleeAttack : MonoBehaviour
    {
        private Player player;
        private PlayerAnimator playerAnimator;

        // base parameters
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("Where the x input is taken into account")]
        [SerializeField] private float xDeadZone;
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("Where the y input is taken into account")]
        [SerializeField] private float yDeadZone;
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("How long is the attack cooldown")]
        [SerializeField] private float attackCoolDown;
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("how long is gravity removed from the player when landing an attack in the air")]
        [SerializeField] private float airAttackGravityRemovalTime;
        [FoldoutGroup("Events")]
        [PropertyTooltip("Events that are called when the player attacks")]
        [SerializeField] private UnityEvent attackedEvent;
        [FoldoutGroup("Events")]
        [PropertyTooltip("Events that are called when the player hits an enemy")]
        [SerializeField] private UnityEvent hitEnemyEvent;

        [FoldoutGroup("Debug")]
        [ReadOnly]
        [ShowInInspector]
        private bool isAttackCoolDownActive;
        [FoldoutGroup("Debug")]
        [ReadOnly]
        [ShowInInspector]
        private bool wasAttackPressedWhileInCoolDown;
        [FoldoutGroup("Debug")]
        [ReadOnly]
        [ShowInInspector]
        private bool isAirAttackTimerActive;
        [FoldoutGroup("Debug")]
        [ReadOnly]
        [ShowInInspector]
        public bool CanPlayerAttack = true;

        private void Start()
        {
            player = PlayerManager.instance.Player;
            playerAnimator = GetComponentInParent<PlayerAnimator>();
        }

        private void Update()
        {
            PlayerInput();
            AirAttackGravity();
        }

        private void PlayerInput()
        {
            if (PlayerManager.instance.PlayerWallSlide.isSlidingOnWall || PlayerManager.instance.PlayerDash.IsDashing)
                return;

            if (player.GetButtonDown("Attack") && CanPlayerAttack)
                Attack();
            else if (player.GetButtonDown("Attack") && isAttackCoolDownActive)
                wasAttackPressedWhileInCoolDown = true;
            else if (!isAttackCoolDownActive && CanPlayerAttack && wasAttackPressedWhileInCoolDown)
                Attack();

        }

        private void Attack()
        {
            StopAllCoroutines();
            StartCoroutine(AttackCoolDown());
            float xAxis = player.GetAxis("Move Horizontal");
            float yAxis = player.GetAxis("Y Axis");

            isAirAttackTimerActive = false;
            wasAttackPressedWhileInCoolDown = false;

            // directional attack logic goes here

            attackedEvent?.Invoke();
            playerAnimator.SetAttackAnimation();
            PlayerManager.instance.PlayerMove.StartCoroutine("AttackSlow");
        }

        private IEnumerator AttackCoolDown()
        {
            CanPlayerAttack = false;
            isAttackCoolDownActive = true;
            yield return new WaitForSecondsRealtime(attackCoolDown);
            isAttackCoolDownActive = false;
            CanPlayerAttack = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Damageable damageable;
            if (collision.gameObject.GetComponent<Damageable>() != null)
            {
                damageable = collision.gameObject.GetComponent<Damageable>();
                damageable.TakeDamage(5);
                hitEnemyEvent?.Invoke();

                if (!PlayerManager.instance.IsGrounded)
                {
                    StartCoroutine(AirHitGravityTimer());
                }
            }
        }

        private void AirAttackGravity()
        {
            if (isAirAttackTimerActive)
                PlayerManager.instance.Rb.velocity = Vector2.zero;
        }

        private IEnumerator AirHitGravityTimer()
        {
            isAirAttackTimerActive = true;
            yield return new WaitForSecondsRealtime(airAttackGravityRemovalTime);
            isAirAttackTimerActive = false;
        }
    }
}