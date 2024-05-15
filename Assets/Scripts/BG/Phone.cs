using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using UnityEngine;

public enum PhoneState
{
    AWAY, PEEKING, CHECKING
}

public class Phone : MonoBehaviour
{
    [SerializeField, MustBeAssigned]
    private BloodGlucoseSimulator bgSimulator;
    [SerializeField, MustBeAssigned]
    private Transform awayPosition;
    [SerializeField, MustBeAssigned]
    private Transform peekPosition;
    [SerializeField, PositiveValueOnly]
    private float timeToPeek = 1f;
    [SerializeField, MustBeAssigned]
    private Transform checkPosition;
    [SerializeField, PositiveValueOnly]
    private float timeToCheck = 3f;
    
    public PhoneState state { get; private set; } = PhoneState.AWAY;
    
    private Transform thisTransform;
    
    private void Start()
    {
        thisTransform = this.transform;
        state = PhoneState.AWAY;
        
        thisTransform.localPosition = awayPosition.localPosition;
        thisTransform.localRotation = awayPosition.localRotation;
    }
    
    public void Check()
    {
        float time = state == PhoneState.AWAY ? timeToCheck : timeToCheck - timeToPeek;
        thisTransform.DOKill();
        thisTransform.DOLocalMove(checkPosition.localPosition, time);
        thisTransform.DOLocalRotate(checkPosition.localEulerAngles, time);
        state = PhoneState.CHECKING;
        bgSimulator.SetToRealtime();
    }
    
    public void Peek()
    {
        thisTransform.DOKill();
        thisTransform.DOLocalMove(peekPosition.localPosition, timeToPeek);
        thisTransform.DOLocalRotate(peekPosition.localEulerAngles, timeToPeek);
        state = PhoneState.PEEKING;
        bgSimulator.SetToRealtime();
    }
    
    public void PutAway()
    {
        float time = state == PhoneState.CHECKING ? timeToCheck : timeToPeek;
        thisTransform.DOKill();
        thisTransform.DOLocalMove(awayPosition.localPosition, time);
        thisTransform.DOLocalRotate(awayPosition.localEulerAngles, time);
        state = PhoneState.AWAY;
        bgSimulator.SetToSimulatedTime();
    }
}
