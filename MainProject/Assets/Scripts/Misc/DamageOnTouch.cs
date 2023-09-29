using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WibertStudio
{
    public class DamageOnTouch : MonoBehaviour
    {
        [SerializeField] private LayerMask damageableLayers;
        [SerializeField] private Vector2 collisionSize;
        [SerializeField] private float damageAmt;
        [SerializeField] private UnityEvent hitEvent;

        private void Update()
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, collisionSize, 0, Vector2.zero, 0, damageableLayers);
            if (hit == true)
                Hit(hit);
        }

        private void Hit(RaycastHit2D hit)
        {

            if (hit.transform.GetComponent<Damageable>() != null)
            {
                Damageable damageable = hit.transform.GetComponent<Damageable>();
                damageable.TakeDamage(damageAmt);
            }

            print("Hit " + hit.transform.name);

            hitEvent?.Invoke();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(transform.position, collisionSize);
        }
    }
}