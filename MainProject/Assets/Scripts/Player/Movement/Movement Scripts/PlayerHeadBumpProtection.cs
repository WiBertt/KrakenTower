using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace WibertStudio
{
    public class PlayerHeadBumpProtection : MonoBehaviour
    {
        [PropertyTooltip("How much the player is moved when trying to avoid hitting their head")]
        [SerializeField] private Vector2 moveOffest;

        private void Update()
        {
            if (PlayerManager.instance.IsOnCeiling)
                CheckCeilingCollision();
        }

        private void CheckCeilingCollision()
        {
            var isOnLeftCeiling = PlayerManager.instance.IsOnLeftCeiling;
            var isOnMiddleCeiling = PlayerManager.instance.IsOnMiddleCeiling;
            var isOnRightCeiling = PlayerManager.instance.IsOnRightCeiling;

            if (isOnLeftCeiling && isOnMiddleCeiling && isOnRightCeiling)
                return;

            // determines which way to move the player
            if (isOnLeftCeiling && !isOnMiddleCeiling && !isOnRightCeiling)
                MovePlayer("Right");
            else if (!isOnLeftCeiling && !isOnMiddleCeiling && isOnRightCeiling)
                MovePlayer("Left");
        }

        private void MovePlayer(string dir)
        {
            if (dir == "Right")
                transform.position = new Vector2(transform.position.x + moveOffest.x, transform.position.y + moveOffest.y);
            else if (dir == "Left")
                transform.position = new Vector2(transform.position.x - moveOffest.x, transform.position.y + moveOffest.y);
        }
    }
}