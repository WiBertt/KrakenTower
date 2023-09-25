using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WibertStudio
{
    public class Damageable : MonoBehaviour
    {
        private Rigidbody2D rb;

        [SerializeField] private float maxHealth;
        [SerializeField] private float invicibilityTime;
        [SerializeField] private bool recievesKnockback;
        [SerializeField] private Vector2 knockbackAmt;

        // Events
        [SerializeField] private UnityEvent tookDamageEvent;
        [SerializeField] private UnityEvent deathEvent;

        private float currentHealth;
        private bool isInvincible;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            currentHealth = maxHealth;
        }
        public void TakeDamage(float damage)
        {
            if (isInvincible)
                return;

            currentHealth -= damage;
            if (currentHealth <= 0)
                Death();
            else
            {
                tookDamageEvent?.Invoke();
                StartCoroutine(InvincibilityTimer());

                if (GetComponent<PatrolAI>() != null)
                {
                    PatrolAI ai = GetComponent<PatrolAI>();
                    ai.ResetCoroutines();
                    ai.StartCoroutine("RemoveMovement");
                }

                if (recievesKnockback)
                    ApplyKnockback();

                
                  
            }
        }

        private IEnumerator InvincibilityTimer()
        {
            isInvincible = true;
            yield return new WaitForSecondsRealtime(invicibilityTime);
            isInvincible = false;
        }

        private void ApplyKnockback()
        {
            if (PlayerManager.instance.IsFacingRight)
                rb.AddForce(new Vector2(knockbackAmt.x, knockbackAmt.y), ForceMode2D.Impulse);
            else
                rb.AddForce(new Vector2(-knockbackAmt.x, knockbackAmt.y), ForceMode2D.Impulse);
        }

        public void Death()
        {
            deathEvent?.Invoke();
            print("Dead");
        }
    }
}