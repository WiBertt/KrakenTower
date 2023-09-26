using Com.LuisPedroFonseca.ProCamera2D.Platformer;
using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WibertStudio
{
    public class PlayerMeleeAttack : MonoBehaviour
    {
        private Player player;
        private PlayerAnimator playerAnimator;

        [SerializeField] private float xDeadZone;
        [SerializeField] private float yDeadZone;
        [SerializeField] private float attackCoolDown;
        [SerializeField] private float airAttackGravityRemovalTime;
        private bool isAttackCoolDownActive;
        private bool wasAttackPressedWhileInCoolDown;
        private bool isAirAttackTimerActive;
        public bool CanPlayerAttack { get; set; } = true;

        private void Start()
        {
            player = ReInput.players.GetPlayer(0);
            playerAnimator = GetComponentInParent<PlayerAnimator>();
        }
        private void Update()
        {
            PlayerInput();
            AirAttackGravity();
        }

        private void AirAttackGravity()
        {
            if (isAirAttackTimerActive)
                PlayerManager.instance.Rb.velocity = Vector2.zero;
        }

        private void PlayerInput()
        {
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


                if (!PlayerManager.instance.IsGrounded)
                {
                    StartCoroutine(AirHitGravityTimer());
                }
            }
        }

        private IEnumerator AirHitGravityTimer()
        {
            isAirAttackTimerActive = true;
            yield return new WaitForSecondsRealtime(airAttackGravityRemovalTime);
            isAirAttackTimerActive = false;
        }
    }
}