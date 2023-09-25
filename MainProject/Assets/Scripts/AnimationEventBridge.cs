using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventBridge : MonoBehaviour
{
    [SerializeField] private UnityEvent PlayerStepEvent;
    [SerializeField] private UnityEvent PlayerWallJumpCompleteEvent;
    [SerializeField] private UnityEvent PlayerTurnStartEvent;
    [SerializeField] private UnityEvent PlayerTurnCompleteEvent;
    [SerializeField] private UnityEvent PlayerRunStartEvent;
    [SerializeField] private UnityEvent PlayerRunStopStartEvent;
    [SerializeField] private UnityEvent PlayerRunStopCompleteEvent;


    public void PlayerStep()
    {
        PlayerStepEvent?.Invoke();
    }

    public void PlayerWallJumpComplete()
    {
        PlayerWallJumpCompleteEvent?.Invoke();
    }
    public void PlayerTurnStart()
    {
        PlayerTurnStartEvent?.Invoke();
    }
    public void PlayerTurnComplete()
    {
        PlayerTurnCompleteEvent?.Invoke();
    }

    public void PlayerRunStopStart()
    {
        PlayerRunStopStartEvent?.Invoke();
    }


    public void PlayerRunStopComplete()
    {
        PlayerRunStopCompleteEvent?.Invoke();
    }
}
