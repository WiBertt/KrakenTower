using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanable : MonoBehaviour 
{
    [SerializeField] SpriteRenderer lockUI;
    public void EnableLockUI()
    {
        lockUI.enabled = true;
    }

    public  void DisableLockUI()
    {
        lockUI.enabled = false;
    }
}
