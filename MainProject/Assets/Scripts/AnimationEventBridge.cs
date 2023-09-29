using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventBridge : MonoBehaviour
{
    [FoldoutGroup("Movement Events")]
    [SerializeField] private UnityEvent PlayerStepEvent;
    [FoldoutGroup("Movement Events")]
    [SerializeField] private UnityEvent PlayerWallJumpCompleteEvent;
    [FoldoutGroup("Movement Events")]
    [SerializeField] private UnityEvent PlayerTurnStartEvent;
    [FoldoutGroup("Movement Events")]
    [SerializeField] private UnityEvent PlayerTurnCompleteEvent;
    [FoldoutGroup("Movement Events")]
    [SerializeField] private UnityEvent PlayerRunStartEvent;
    [FoldoutGroup("Movement Events")]
    [SerializeField] private UnityEvent PlayerRunStopStartEvent;
    [FoldoutGroup("Movement Events")]
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
