using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CocoonAbility : AbilityBase
{
    public bool IsCocoonActive { get; private set; }
    [SerializeField] private float cocoonLength;

    public override void ActivateAbility()
    {
        print("Activated cocoon");
        IsCocoonActive = true;
        StartCoroutine(CocoonTimer());
    }

    private IEnumerator CocoonTimer()
    {
        yield return new WaitForSecondsRealtime(cocoonLength);
        IsCocoonActive = false;
    }
}
