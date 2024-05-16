using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class TimeManager : SingletonMonoBehaviour<TimeManager>
{
    public const float SECONDS_IN_DAY = 24f * 60f * 60f;
    public const float SECONDS_IN_HOUR = 60f * 60f;
    
    [SerializeField,
        Tooltip("When in \"realtime mode\", this is the number of simulated seconds that pass per real second")]
    private float realtimeTimeScale = 1f;
    [SerializeField,
        Tooltip("When in \"simulated time mode\", this is the number of simulated seconds that pass per real second")]
    private float simulatedTimeScale = 240f; //this is also 4 hours per real minute, or 24 hours in 6 real minutes
    
    [FoldoutGroup("REFERENCES", expanded:true), SerializeField, MustBeAssigned]
    private Phone phone;
    
    [FoldoutGroup("UI", expanded:true), SerializeField, MustBeAssigned]
    private TextMeshProUGUI timeText;
    [FoldoutGroup("UI"), SerializeField, MustBeAssigned]
    private TextMeshProUGUI timeModeText;
    
    public static float time { get; private set; } = 0f;
    public static TimeMode timeMode { get; private set; } = TimeMode.REALTIME;
    public static float timeScale => timeMode == TimeMode.REALTIME ? instance.realtimeTimeScale : instance.simulatedTimeScale;
    
    protected override void Awake()
    {
        if(!InitializeSingleton(this))
        {
            return;
        }
    }

    private void Start()
    {
        SetRealtime();
    }

    public static void SetRealtime()
    {
        timeMode = TimeMode.REALTIME;
        instance.timeModeText.text = TimeMode.REALTIME.ToString();
    }
    
    public static void SetSimulatedTime()
    {
        timeMode = TimeMode.SIMULATED;
        instance.timeModeText.text = TimeMode.SIMULATED.ToString();
    }

    private void Update()
    {
        HandleTimeModeChange();
        
        time += Time.deltaTime * timeScale;
        timeText.text = TimeOfDay.FromSeconds((int)time).ToString();
        
        #region Local Methods
        
        void HandleTimeModeChange()
        {
            if (!Input.GetKeyDown(KeyCode.Space))
            {
                return;
            }
            
            if(timeMode == TimeMode.REALTIME)
            {
                if(phone.state != PhoneState.AWAY && !phone.debugMode)
                {
                    //TODO show a message that you can't change time while phone is not away
                    return;
                }
                SetSimulatedTime();
            }
            else
            {
                SetRealtime();
            }
        }
        
        #endregion
    }
}
