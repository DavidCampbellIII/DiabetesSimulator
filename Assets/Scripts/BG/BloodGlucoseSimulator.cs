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
    
    private const float SECONDS_IN_DAY = 24f * 60f * 60f;
    private const float SECONDS_IN_HOUR = 60f * 60f;
    
    [FoldoutGroup("SETTINGS", expanded:true), SerializeField, PositiveValueOnly,
        Tooltip("Simulated interval between glucose readings in seconds.")]
    private float simulatedTimeBetweenReadings = 300f; //5 minutes
    [FoldoutGroup("SETTINGS"), SerializeField, Range(20f, 400f), 
        Tooltip("Initial blood glucose reading")]
    private float initialReading = 100f;
    
    [FoldoutGroup("INSULIN", expanded:true), SerializeField, PositiveValueOnly,
        Tooltip("1 unit of insulin will lower BG by this amount")]
    private float insulinSensitivity = 25f;
    [FoldoutGroup("INSULIN"), SerializeField, PositiveValueOnly,
        Tooltip("Amount that exercise related insulin sensitivity decays per hour")]
    private float exerciseInsulinDecay = 10f;
    [FoldoutGroup("INSULIN"), SerializeField, PositiveValueOnly,
        Tooltip("Max amount of additional insulin sensitivity from exercise")]
    private float maxExerciseInsulinSensitivity = 50f;
    [FoldoutGroup("INSULIN"), SerializeField, PositiveValueOnly,
        Tooltip("Delay in seconds before insulin begins to lower BG")]
    private float insulinAbsorptionDelay = 1800f; // 30 minutes
    [FoldoutGroup("INSULIN"), SerializeField, PositiveValueOnly,
        Tooltip("Amount of time in seconds that insulin is active")]
    private float insulinDuration = 18000f; // 5 hours
    [FoldoutGroup("INSULIN"), SerializeField, Range(0.01f, 1f),
        Tooltip("Max multiplier for insulin sensitivity when BG is above target")]
    private float insulinResistanceMulti = 0.333f;
    [FoldoutGroup("INSULIN"), SerializeField]
    private bool useBasal = true;
    [FoldoutGroup("INSULIN"), SerializeField, PositiveValueOnly, ShowIf(nameof(useBasal)),
        Tooltip("Amount of basal insulin in units per hour")]
    private float basalInsulin = 1f;
    [FoldoutGroup("INSULIN"), SerializeField, ShowIf(nameof(useBasal)),
        Tooltip("Should the basal rate adjust automatically to maintain a target BG?")]
    private bool autoAdjustBasal = true;
    [FoldoutGroup("INSULIN"), SerializeField, PositiveValueOnly, ShowIf("@ShowAutoAdjustBasal()")]
    private float targetBloodGlucose = 110f;
    [FoldoutGroup("INSULIN"), SerializeField, PositiveValueOnly, ShowIf("@ShowAutoAdjustBasal()"),
        Tooltip("Maximum multiplier for auto-adjusting basal insulin")]
    private float maxBasalMulti = 2f;
    
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
    [FoldoutGroup("CURVES"), SerializeField, MustBeAssigned]
    private AnimationCurve exerciseDecayCurve;
    
    [FoldoutGroup("REFERENCES"), SerializeField, MustBeAssigned]
    private Graph graph;
    
    [FoldoutGroup("SITUATIONS", expanded:true), SerializeField]
    private bool siteFailure = false;
    //TODO move CGM related issues to it's own CGM class, route BG to that class, then route that to graph class
    [FoldoutGroup("SITUATIONS"), SerializeField, Range(0f, 1f)]
    private float cgmFailureRate = 0.05f;
    [FoldoutGroup("SITUATIONS"), SerializeField]
    private MinMaxFloat minMaxCgmFailureSwing = new MinMaxFloat(3, 20f);
    
    private float nextTimeToRead;
    private float reading;
    private float insulinOnBoard;
    private float sugarOnBoard;
    
    [SerializeField, Sirenix.OdinInspector.ReadOnly]
    private float exerciseInsulinSensitivity;
    
    private readonly List<InsulinDose> insulinHistory = new List<InsulinDose>();
    private readonly List<SugarDose> sugarHistory = new List<SugarDose>();
    
    #region Inspector Methods
    
    bool ShowAutoAdjustBasal()
    {
        return useBasal && autoAdjustBasal;
    }
    
    #endregion
    
    private void Start()
    {
        nextTimeToRead = 0;
        reading = initialReading;
    }
    
    [Button("Add Insulin")]
    private void AddInsulin(float units)
    {
        if(siteFailure)
        {
            return;
        }
        
        insulinOnBoard += units;
        insulinHistory.Add(new InsulinDose(units, TimeManager.time));
    }
    
    [Button("Add Carbs")]
    private void AddCarbs(float grams, float glycemicIndex = 1f)
    {
        sugarOnBoard += grams;
        sugarHistory.Add(new SugarDose(grams, glycemicIndex, TimeManager.time));
    }
    
    public void AddToExerciseInsulinSensitivity(float amount)
    {
        exerciseInsulinSensitivity += amount;
        exerciseInsulinSensitivity = Mathf.Min(exerciseInsulinSensitivity, maxExerciseInsulinSensitivity);
    }
    
    private void Update()
    {
        if(TimeManager.time >= nextTimeToRead)
        {
            TickBloodGlucose();
            nextTimeToRead = TimeManager.time + simulatedTimeBetweenReadings;
        }
    }
    
    private void TickBloodGlucose()
    {
        HandleBasal();
        HandleLiverDump();
        
        float sugar = ApplySugar();
        float insulin = ApplyInsulin();
        
        HandleExerciseInsulinSensitivityDecay();
        
        float lastReading = reading;
        reading += sugar - insulin;
        float delta = Mathf.Floor(reading) - Mathf.Floor(lastReading);
        
        float cgmReading = reading;
        if(Random.value < cgmFailureRate)
        {
            cgmReading += minMaxCgmFailureSwing.RandomInRange() * (Random.value > 0.5f ? 1 : -1);
            Debug.Log($"CGM error! Reading: {reading} CGM: {cgmReading} Diff: {cgmReading - reading:+#;-#;0}");
        }
        graph.AddReading(new BloodGlucoseReading(TimeManager.time, cgmReading));
        graph.UpdateStats(delta, insulinOnBoard, sugarOnBoard);
            
        #region Local Methods
        
        void HandleBasal()
        {
            if (!useBasal)
            {
                return;
            }
            
            float basal = basalInsulin;
            if(autoAdjustBasal)
            {
                if(reading < 80 || reading < targetBloodGlucose && insulinOnBoard > 1.25f)
                {
                    basal = 0;
                }
                else
                {
                    float targetRatio = reading / targetBloodGlucose;
                    basal *= Mathf.Clamp(targetRatio, 0, maxBasalMulti);
                    if(reading < targetBloodGlucose)
                    {
                        basal *= targetRatio;
                    }
                }
            }
            graph.UpdateBasalRate(basal);
            AddInsulin(basal * simulatedTimeBetweenReadings / SECONDS_IN_HOUR);
        }
        
        void HandleLiverDump()
        {
            if (!useLiverDump)
            {
                return;
            }
            
            float liverDump = liverDumpRate;
            if(reading > 140)
            {
                liverDump /= 1.5f;
            }
            else if(reading < 70)
            {
                liverDump *= 1.5f;
            }
            AddCarbs(liverDump * simulatedTimeBetweenReadings / SECONDS_IN_HOUR);
        }
        
        void HandleExerciseInsulinSensitivityDecay()
        {
            if(exerciseInsulinSensitivity <= 0)
            {
                return;
            }
            
            float decayRate = exerciseDecayCurve.Evaluate(exerciseInsulinSensitivity / maxExerciseInsulinSensitivity);
            float exerciseDecay = exerciseInsulinDecay * decayRate * simulatedTimeBetweenReadings / SECONDS_IN_HOUR;
            exerciseInsulinSensitivity = Mathf.Max(0, exerciseInsulinSensitivity - exerciseDecay);
        }
        
        #endregion
    }
    
    private float ApplySugar()
    {
        float totalBgEffect = 0;
        foreach(SugarDose dose in sugarHistory)
        {
            float elapsed = TimeManager.time - dose.time;
            if(elapsed < sugarAbsorptionDelay || dose.currentGrams <= 0)
            {
                //Debug.Log($"Cannot absorb sugar yet. Elapsed: {elapsed} grams: {dose.grams}.");
                continue;
            }
            
            float normalizedTime = (elapsed - sugarAbsorptionDelay) / (sugarDumpRate * 1.05f * dose.grams);
            float sugarAbsorbed = simulatedTimeBetweenReadings / sugarDumpRate * dose.glycemicIndex;
            //Debug.Log($"Absorbed {sugarAbsorbed} grams of sugar.");
            dose.Absorb(sugarAbsorbed);
            sugarOnBoard -= sugarAbsorbed;
            sugarOnBoard = Mathf.Max(0, sugarOnBoard);
            
            totalBgEffect += sugarAbsorbed * sugarSensitivity * sugarCurve.Evaluate(normalizedTime);
        }
        RemoveInactiveSugarDoses();
        //Debug.Log($"Total SUGAR BG Effect: {totalBgEffect}");
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
            float elapsed = TimeManager.time - dose.time;
            if(elapsed < insulinAbsorptionDelay || dose.currentUnits <= 0)
            {
                //Debug.Log($"Cannot absorb insulin yet. Elapsed: {elapsed} units: {dose.units}.");
                continue;
            }
            
            float normalizedTime = (elapsed - insulinAbsorptionDelay) / insulinDuration;
            float insulinAbsorbed = simulatedTimeBetweenReadings / insulinDuration * dose.units;
            //Debug.Log($"Absorbed {insulinAbsorbed} units of insulin, normalized time: {normalizedTime}.");
            dose.Decay(insulinAbsorbed);
            insulinOnBoard -= insulinAbsorbed;
            insulinOnBoard = Mathf.Max(0, insulinOnBoard);
            //Debug.Log($"Remaining units: {dose.currentUnits}.");
            
            float targetRatio = reading / targetBloodGlucose;
            float insulinResistance = Mathf.Clamp(targetRatio - 1f, insulinResistanceMulti, 0.8f);
            float sensitivity = insulinSensitivity * (1f - insulinResistance) + exerciseInsulinSensitivity;
            totalBgEffect += insulinAbsorbed * (sensitivity / (insulinDuration / 60) * simulatedTimeBetweenReadings) * insulinCurve.Evaluate(normalizedTime);
        }
        RemoveInactiveInsulinDoses();
        //Debug.Log($"Total INSULIN BG Effect: {totalBgEffect}");
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
