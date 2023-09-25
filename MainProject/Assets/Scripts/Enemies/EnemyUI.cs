using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles UI for enemies such as target lock sprite
/// </summary>
public class EnemyUI : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetLockSprite;
    public void EnableLockUI()
    {
        targetLockSprite.enabled = true;
    }

    public void DisableLockUI()
    {
        targetLockSprite.enabled = false;
    }
}
