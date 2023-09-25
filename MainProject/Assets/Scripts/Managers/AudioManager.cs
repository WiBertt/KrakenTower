using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WibertStudio;

public class AudioManager : MonoBehaviour
{
    private void Awake()
    {
        PlayerJump.PlayerJumpAction += JumpSFX;
    }

    private void JumpSFX()
    {
        MasterAudio.PlaySound("Jump");
    }

   public void AttackHitSFX()
    {
        MasterAudio.PlaySound("SwordImpact");
    }
}
