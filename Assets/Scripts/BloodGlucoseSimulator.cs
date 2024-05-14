using System.Collections;
using System.Collections.Generic;
using MyBox;
using Sirenix.OdinInspector;
using UnityEngine;

public struct BloodGlucoseReading
{
    public float time { get; }
    public float reading { get; }
    
    public BloodGlucoseReading(float time, float reading)
    {
        this.time = time;
        this.reading = reading;
    }
}

public class BloodGlucoseSimulator : MonoBehaviour
{
    #region Nested Structures
    
    private class InsulinDose
    {
        public float units { get; }
        public float currentUnits { get; private set; }
        public float time { get; }
        
        public InsulinDose(float units, float time)
        {
            this.units = units;
            this.time = time;
            
            currentUnits = units;
        }
        
        public void Decay(float amount)
        {
            currentUnits -= amount;
        }
    }
    
    private class SugarDose
    {
        public float grams { get; }
        public float currentGrams { get; private set; }
        public float glycemicIndex { get; }
        public float time { get; }
        
        public SugarDose(float grams, float glycemicIndex, float time)
        {
            this.grams = grams;
            this.glycemicIndex = glycemicIndex;
            this.time = time;
            
            currentGrams = grams;
        }
        
        public void Absorb(float amount)
        {
            currentGrams -= amount;
        }
    }
    
    #endregion
    
    [SerializeField, PositiveValueOnly,
        Tooltip("Real-time interval between glucose readings in seconds.")]
    private float realTimeBetweenReadings = 3f;
    [SerializeField, PositiveValueOnly,
        Tooltip("Simulated interval between glucose readings in seconds.")]
    private float simulatedTimeBetweenReadings = 300f; //5 minutes
    
    [SerializeField, Range(20f, 400f), 
        Tooltip("Initial blood glucose reading.")]
    private float initialReading = 100f;
    
    [FoldoutGroup("INSULIN", expanded:true), SerializeField, PositiveValueOnly,
        Tooltip("1 unit of insulin will lower BG by this amount.")]
    private float insulinSensitivity = 25f;
    [FoldoutGroup("INSULIN"), SerializeField, PositiveValueOnly,
        Tooltip("Delay in seconds before insulin begins to lower BG")]
    private float insulinAbsorptionDelay = 1800f; // 30 minutes
    [FoldoutGroup("INSULIN"), SerializeField, PositiveValueOnly,
        Tooltip("Amount of time in seconds that insulin is active.")]
    private float insulinDuration = 18000f; // 5 hours
    [FoldoutGroup("INSULIN"), SerializeField]
    private bool useBasal = true;
    [FoldoutGroup("INSULIN"), SerializeField, PositiveValueOnly, ShowIf(nameof(useBasal)),
        Tooltip("Amount of basal insulin in units per hour.")]
    private float basalInsulin = 1f;
    
    [FoldoutGroup("SUGAR", expanded:true), SerializeField, PositiveValueOnly,
        Tooltip("1g carb will raise BG by this amount.")]
    private float sugarSensitivity = 5f;
    [FoldoutGroup("SUGAR"), SerializeField, PositiveValueOnly,
        Tooltip("Delay in seconds before carbs begin to absorb")]
    private float sugarAbsorptionDelay = 1800f; // 30 minutes
    [FoldoutGroup("SUGAR"), SerializeField, PositiveValueOnly,
        Tooltip("Amount of time in seconds a single carb absorbs")]
    private float sugarDumpRate = 300f; // 5 minutes
    [FoldoutGroup("SUGAR"), SerializeField]
    private bool useLiverDump = true;
    [FoldoutGroup("SUGAR"), SerializeField, PositiveValueOnly, ShowIf(nameof(useLiverDump)),
        Tooltip("Amount of carbs in grams per hour that the liver releases.")]
    private float liverDumpRate = 10f;
    
    [FoldoutGroup("CURVES"), SerializeField, MustBeAssigned]
    private AnimationCurve insulinCurve;
    [FoldoutGroup("CURVES"), SerializeField, MustBeAssigned]
    private AnimationCurve sugarCurve;
    
    [FoldoutGroup("REFERENCES"), SerializeField, MustBeAssigned]
    private Graph graph;
    
    private float time;
    private float reading;
    private float insulinOnBoard;
    private float sugarOnBoard;
    
    private readonly List<InsulinDose> insulinHistory = new List<InsulinDose>();
    private readonly List<SugarDose> sugarHistory = new List<SugarDose>();
    
    private void Start()
    {
        time = 0;
        reading = initialReading;
        StartCoroutine(WaitForGlucoseReading());
    }
    
    [Button("Add Insulin")]
    private void AddInsulin(float units)
    {
        insulinOnBoard += units;
        insulinHistory.Add(new InsulinDose(units, time));
    }
    
    [Button("Add Carbs")]
    private void AddCarbs(float grams, float glycemicIndex = 1f)
    {
        sugarOnBoard += grams;
        sugarHistory.Add(new SugarDose(grams, glycemicIndex, time));
    }
    
    private IEnumerator WaitForGlucoseReading()
    {
        while (enabled)
        {
            if(useBasal)
            {
                AddInsulin(basalInsulin * simulatedTimeBetweenReadings / 3600);
            }
            
            if(useLiverDump)
            {
                AddCarbs(liverDumpRate * simulatedTimeBetweenReadings / 3600);
            }
            
            float sugar = ApplySugar();
            float insulin = ApplyInsulin();
            
            reading += sugar - insulin;
            graph.AddReading(new BloodGlucoseReading(time, reading));
            yield return new WaitForSeconds(realTimeBetweenReadings);
            time += simulatedTimeBetweenReadings;
        }
    }
    
    private float ApplySugar()
    {
        float totalBgEffect = 0;
        foreach(SugarDose dose in sugarHistory)
        {
            float elapsed = time - dose.time;
            if(elapsed < sugarAbsorptionDelay || dose.currentGrams <= 0)
            {
                //Debug.Log($"Cannot absorb sugar yet. Elapsed: {elapsed} grams: {dose.grams}.");
                continue;
            }
            
            float normalizedTime = (elapsed - sugarAbsorptionDelay) / (sugarDumpRate * dose.grams);
            float sugarAbsorbed = simulatedTimeBetweenReadings / sugarDumpRate * dose.glycemicIndex;
            Debug.Log($"Absorbed {sugarAbsorbed} grams of sugar.");
            dose.Absorb(sugarAbsorbed);
            
            totalBgEffect += sugarAbsorbed * sugarSensitivity * sugarCurve.Evaluate(normalizedTime);
        }
        RemoveInactiveSugarDoses();
        Debug.Log($"Total SUGAR BG Effect: {totalBgEffect}");
        return totalBgEffect;
        
        #region Local Methods
        
        void RemoveInactiveSugarDoses()
        {
            for (int i = sugarHistory.Count - 1; i >= 0; i--)
            {
                if (sugarHistory[i].currentGrams <= 0)
                {
                    sugarHistory.RemoveAt(i);
                }
            }
        }
        
        #endregion
    }
    
    private float ApplyInsulin()
    {
        float totalBgEffect = 0;
        foreach(InsulinDose dose in insulinHistory)
        {
            float elapsed = time - dose.time;
            if(elapsed < insulinAbsorptionDelay || dose.currentUnits <= 0)
            {
                //Debug.Log($"Cannot absorb insulin yet. Elapsed: {elapsed} units: {dose.units}.");
                continue;
            }
            
            float normalizedTime = (elapsed - insulinAbsorptionDelay) / insulinDuration;
            float insulinAbsorbed = simulatedTimeBetweenReadings / insulinDuration * dose.units;
            //Debug.Log($"Absorbed {insulinAbsorbed} units of insulin, normalized time: {normalizedTime}.");
            dose.Decay(insulinAbsorbed);
            //Debug.Log($"Remaining units: {dose.currentUnits}.");
            
            totalBgEffect += insulinAbsorbed * (insulinSensitivity / (insulinDuration / 60) * simulatedTimeBetweenReadings) * insulinCurve.Evaluate(normalizedTime);
        }
        RemoveInactiveInsulinDoses();
        Debug.Log($"Total INSULIN BG Effect: {totalBgEffect}");
        return totalBgEffect;
        
        #region Local Methods
        
        void RemoveInactiveInsulinDoses()
        {
            for (int i = insulinHistory.Count - 1; i >= 0; i--)
            {
                if (insulinHistory[i].currentUnits <= 0)
                {
                    insulinHistory.RemoveAt(i);
                }
            }
        }
        
        #endregion
    }
}
