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
        [SerializeField] private float timeToRemovePlayerInputAfterHit;
        [SerializeField] private Vector2 fwdKnockBackAmt;
        [SerializeField] private Vector2 upKnockBackAmt;
        [SerializeField] private Vector2 downKnockBackAmt;
        [SerializeField] private LayerMask hittableLayers;
        private bool isAttackCoolDownActive;
        private bool wasAttackPressedWhileInCoolDown;
        public bool CanPlayerAttack { get; set; } = true;

        private enum AttackDir
        {
            None,
            Fwd,
            Down,
            Up
        }

        private AttackDir attackDir;
        private void Start()
        {
            player = ReInput.players.GetPlayer(0);
            playerAnimator = GetComponentInParent<PlayerAnimator>();
        }
        private void Update()
        {
            PlayerInput();
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
            StartCoroutine(AttackCoolDown());
            float xAxis = player.GetAxis("Move Horizontal");
            float yAxis = player.GetAxis("Y Axis");

            wasAttackPressedWhileInCoolDown = false;
            // No input
            if (xAxis == 0 && yAxis == 0)
            {
                playerAnimator.SetAttackAnimation("fwd");
                attackDir = AttackDir.Fwd;
                return;
            }

            // Horizontal
            if (yAxis > -yDeadZone && yAxis < yDeadZone)
                if (xAxis > xDeadZone)
                {
                    playerAnimator.SetAttackAnimation("fwd");
                    attackDir = AttackDir.Fwd;
                    return;
                }
                else if (xAxis < -xDeadZone)
                {
                    playerAnimator.SetAttackAnimation("fwd");
                    attackDir = AttackDir.Fwd;
                    return;
                }

            // Vertical
            if (yAxis > yDeadZone)
            {
                playerAnimator.SetAttackAnimation("up");
                attackDir = AttackDir.Up;
                return;
            }
            else if (yAxis < -yDeadZone && !PlayerManager.instance.IsGrounded)
            {
                playerAnimator.SetAttackAnimation("down");
                attackDir = AttackDir.Down;
                return;
            }
        }

        private IEnumerator AttackCoolDown()
        {
            CanPlayerAttack = false;
            isAttackCoolDownActive = true;
            yield return new WaitForSecondsRealtime(attackCoolDown);
            attackDir = AttackDir.None;
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
                StartCoroutine(SetVelocity());
                ApplyKnockback();
            }
            else if (collision.gameObject.layer == 6)
            {
                StartCoroutine(SetVelocity());
                if (attackDir == AttackDir.Fwd)
                    ApplyKnockback();
            }

        }

        private IEnumerator SetVelocity()
        {
            PlayerManager.instance.Rb.velocity = Vector2.zero;
            PlayerManager.instance.DoesPlayerHaveControl = false;
            yield return new WaitForSecondsRealtime(timeToRemovePlayerInputAfterHit);
            PlayerManager.instance.DoesPlayerHaveControl = true;
        }
        private void ApplyKnockback()
        {
            PlayerManager.instance.Rb.velocity = Vector2.zero;
            switch (attackDir)
            {
                case AttackDir.None:
                    print("Error no dir");
                    break;
                case AttackDir.Fwd:
                    if (PlayerManager.instance.IsFacingRight)
                        PlayerManager.instance.Rb.AddForce(new Vector2(-fwdKnockBackAmt.x, fwdKnockBackAmt.y), ForceMode2D.Impulse);
                    else
                        PlayerManager.instance.Rb.AddForce(new Vector2(fwdKnockBackAmt.x, fwdKnockBackAmt.y), ForceMode2D.Impulse);
                    break;
                case AttackDir.Up:

                    PlayerManager.instance.Rb.AddForce(new Vector2(upKnockBackAmt.x, upKnockBackAmt.y), ForceMode2D.Impulse);
                    break;
                case AttackDir.Down:
                    PlayerManager.instance.Rb.AddForce(new Vector2(downKnockBackAmt.x, downKnockBackAmt.y), ForceMode2D.Impulse);
                    break;
            }
        }

    }
}