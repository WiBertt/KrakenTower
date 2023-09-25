using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace WibertStudio
{
    public class PlayerHeadBumpProtection : MonoBehaviour
    {
        [SerializeField] private Vector2 moveOffest;
        private void Start()
        {

        }
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