using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WibertStudio {
    public class PlayerVFX : MonoBehaviour
    {
        Animator animator;
        AnimationClip[] clipInfo;
        SpriteRenderer spriteRenderer;
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            if (!PlayerManager.instance.IsFacingRight)
                spriteRenderer.flipX = true;

            transform.parent = null;
          
            clipInfo = animator.runtimeAnimatorController.animationClips;
            Destroy(gameObject, clipInfo[0].length);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}