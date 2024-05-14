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
    [SerializeField, PositiveValueOnly,
        Tooltip("Real-time interval between glucose readings in seconds.")]
    private float realTimeBetweenReadings = 3f;
    [SerializeField, PositiveValueOnly,
        Tooltip("Simulated interval between glucose readings in seconds.")]
    private float simulatedTimeBetweenReadings = 300f;
    
    [SerializeField, PositiveValueOnly,
        Tooltip("1 unit of insulin will lower BG by this amount.")]
    private float insulinSensitivity = 25f;
    [SerializeField, PositiveValueOnly,
        Tooltip("Amount of time in seconds that insulin is active.")]
    private float insulinDuration = 18000f; // 5 hours
    
    [SerializeField, PositiveValueOnly,
        Tooltip("1g carb will raise BG by this amount.")]
    private float sugarSensitivity = 5f;
    [SerializeField, PositiveValueOnly,
        Tooltip("Delay in seconds before carbs begin to absorb")]
    private float sugarAbsorptionDelay = 1800f; // 30 minutes
    [SerializeField, PositiveValueOnly,
        Tooltip("Amount of time in seconds a single carb absorbs")]
    private float sugarDumpRate = 300f; // 5 minutes
    
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
    
    private Dictionary<float, float> insulinHistory = new Dictionary<float, float>();
    private Dictionary<float, float> sugarHistory = new Dictionary<float, float>();
    
    private void Start()
    {
        time = 0;
        StartCoroutine(WaitForGlucoseReading());
    }
    
    [Button("Add Insulin")]
    private void AddInsulin(float units)
    {
        insulinOnBoard += units;
        insulinHistory.Add(time, units);
    }
    
    [Button("Add Carbs")]
    private void AddCarbs(float grams)
    {
        sugarOnBoard += grams;
        sugarHistory.Add(time, grams);
    }
    
    private IEnumerator WaitForGlucoseReading()
    {
        while (enabled)
        {
            //TODO Calculate blood glucose reading
            graph.AddReading(new BloodGlucoseReading(time, reading));
            yield return new WaitForSeconds(realTimeBetweenReadings);
            time += simulatedTimeBetweenReadings;
        }
    }
}
