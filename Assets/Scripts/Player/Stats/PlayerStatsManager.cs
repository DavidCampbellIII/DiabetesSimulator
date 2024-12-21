using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerStatsManager : SingletonMonoBehaviour<PlayerStatsManager>
{
    #region Nested Structures
    
    [System.Serializable]
    private class PlayerStat
    {
        [SerializeField]
        private PlayerStatType _type;
        public PlayerStatType type => _type;
        
        [SerializeField, Range(0f, 1f)]
        private float _value;
        public float value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp01(value);
                onValueChanged?.Invoke(_value);
            }
        }
        
        private Action<float> onValueChanged;
        
        public void Init(Action<float> onValueChanged)
        {
            this.onValueChanged = onValueChanged;
            onValueChanged?.Invoke(value);
        }
        
        [Button]
        private void OnValueChanged()
        {
            onValueChanged?.Invoke(_value);
        }
    }
    
    #endregion
    
    [FoldoutGroup("REFERENCES", expanded:true), SerializeField, MustBeAssigned]
    private PlayerStatsDisplay _display;
    public static PlayerStatsDisplay display => instance._display;
    
    [FoldoutGroup("SETTINGS"), SerializeField]
    private float sleepDecayPerHour = 0.055f;
    [FoldoutGroup("SETTINGS"), SerializeField]
    private float hungerDecayPerHour = 0.1f;
    [FoldoutGroup("SETTINGS"), SerializeField]
    private float happinessDecayPerHour = 0.035f;
    [FoldoutGroup("SETTINGS"), SerializeField]
    private float socialDecayPerHour = 0.015f;
    
    [SerializeField, MustBeAssigned]
    private PlayerStat[] playerStats;

    protected override void Awake()
    {
        if(!InitializeSingleton(this))
        {
            return;
        }
    }

    private void Start()
    {
        if(playerStats.Length != Enum.GetValues(typeof(PlayerStatType)).Length)
        {
            Debug.LogError("Player stats array length does not match number of player stat types");
            return;
        }
        
        //make sure stats are ordered by enum value for easier access
        Array.Sort(playerStats, (a, b) => a.type.CompareTo(b.type));
        
        playerStats[(int)PlayerStatType.SLEEP].Init(OnSleepChanged);
        playerStats[(int)PlayerStatType.HUNGER].Init(OnHungerChanged);
        playerStats[(int)PlayerStatType.HAPPINESS].Init(OnHappinessChanged);
        playerStats[(int)PlayerStatType.SOCIAL_PERCEPTION].Init(OnSocialPerceptionChanged);
    }
    
    private void Update()
    {
        float delta = Time.deltaTime * TimeManager.timeScale;
        AdjustStatValue(PlayerStatType.SLEEP, -sleepDecayPerHour / TimeManager.SECONDS_IN_HOUR * delta);
        AdjustStatValue(PlayerStatType.HUNGER, -hungerDecayPerHour / TimeManager.SECONDS_IN_HOUR * delta);
        AdjustStatValue(PlayerStatType.HAPPINESS, -happinessDecayPerHour / TimeManager.SECONDS_IN_HOUR * delta);
        AdjustStatValue(PlayerStatType.SOCIAL_PERCEPTION, -socialDecayPerHour / TimeManager.SECONDS_IN_HOUR * delta);
    }
    
    public static float GetStatValue(PlayerStatType type)
    {
        return instance.playerStats[(int)type].value;
    }
    
    public static void AdjustStatValue(PlayerStatType type, float amount)
    {
        instance.playerStats[(int)type].value += amount;
    }
    
    #region Stat Change Callbacks
    
    private void OnSleepChanged(float value)
    {
        //TODO
        display.UpdateBar(PlayerStatType.SLEEP, value);
    }
    
    private void OnHungerChanged(float value)
    {
        //TODO
        display.UpdateBar(PlayerStatType.HUNGER, value);
    }
    
    private void OnHappinessChanged(float value)
    {
        //TODO
        display.UpdateBar(PlayerStatType.HAPPINESS, value);
    }
    
    private void OnSocialPerceptionChanged(float value)
    {
        //TODO
        display.UpdateBar(PlayerStatType.SOCIAL_PERCEPTION, value);
    }
    
    #endregion
}
