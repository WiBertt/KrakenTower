using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class AbilityBase : MonoBehaviour 
{
    private Player player;

    public string Name;
    public string Description;
    public float CoolDownTime;
    public enum AbilityState
    {
        Ready,
        Active,
        CoolDown
    }
    public AbilityState State;

    private void Awake()
    {
        player = ReInput.players.GetPlayer(0);

        State = AbilityState.Ready;
    }

    private void Update()
    {
        if(player.GetButtonDown("Ability Left"))
            CheckAbility();
    }
    public virtual void CheckAbility()
    {
        if (State == AbilityState.Ready)
        {
            Debug.Log("Ready");
            ActivateAbility();
            StartCoroutine(StartAbilityCoolDown());
        }
        if (State == AbilityState.Active)
        {
            Debug.Log("Active");
        }
        if (State == AbilityState.CoolDown)
        {
            Debug.Log("CoolDown");
        }
    }

    private IEnumerator StartAbilityCoolDown()
    {
        State = AbilityState.CoolDown;
        yield return new WaitForSecondsRealtime(CoolDownTime);
        State = AbilityState.Ready;
    }

    public virtual void ActivateAbility() { }
}
