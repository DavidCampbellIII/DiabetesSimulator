using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using Sirenix.OdinInspector;
using UnityEngine;

public class Phone : MonoBehaviour
{
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
    
    [FoldoutGroup("DEBUG"), SerializeField,
        Tooltip("Makes things like checking phone faster and allows for simulated time while phone is not away")]
    private bool _debugMode = false;
    public bool debugMode => _debugMode;
    
    public PhoneState state { get; private set; } = PhoneState.AWAY;
    
    private Transform thisTransform;
    
    private void Start()
    {
        thisTransform = this.transform;
        state = PhoneState.AWAY;
        Utilities.ToggleCursorLock(true);
        
        thisTransform.localPosition = awayPosition.localPosition;
        thisTransform.localRotation = awayPosition.localRotation;
    }
    
    public void Check()
    {
        float time = state == PhoneState.AWAY ? timeToCheck : timeToCheck - timeToPeek;
        if(debugMode)
        {
            time = 0.1f;
        }
        
        thisTransform.DOKill();
        thisTransform.DOLocalMove(checkPosition.localPosition, time);
        thisTransform.DOLocalRotate(checkPosition.localEulerAngles, time);
        
        state = PhoneState.CHECKING;
        Utilities.ToggleCursorLock(false);
        
        if(!debugMode)
        {
            TimeManager.SetRealtime();
        }
    }
    
    public void Peek()
    {
        float time = debugMode ? 0.1f : timeToPeek;
        
        thisTransform.DOKill();
        thisTransform.DOLocalMove(peekPosition.localPosition, time);
        thisTransform.DOLocalRotate(peekPosition.localEulerAngles, time);
        
        state = PhoneState.PEEKING;
        Utilities.ToggleCursorLock(true);
        
        if(!debugMode)
        {
            TimeManager.SetRealtime();
        }
    }
    
    public void PutAway()
    {
        float time = state == PhoneState.CHECKING ? timeToCheck : timeToPeek;
        if(debugMode)
        {
            time = 0.1f;
        }
        
        thisTransform.DOKill();
        thisTransform.DOLocalMove(awayPosition.localPosition, time);
        thisTransform.DOLocalRotate(awayPosition.localEulerAngles, time);
        
        state = PhoneState.AWAY;
        Utilities.ToggleCursorLock(true);
    }
}
