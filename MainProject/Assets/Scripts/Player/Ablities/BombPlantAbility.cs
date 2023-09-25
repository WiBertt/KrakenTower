using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPlantAbility : AbilityBase
{
    public GameObject bombPrefab;
    public override void ActivateAbility()
    {
        Instantiate(bombPrefab, transform.position, Quaternion.identity);
        print("Activating ability");
    }
}
