using System.Collections;
using System.Collections.Generic;
using MyBox;
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
    [SerializeField, MustBeAssigned]
    private Graph graph;
    
    private void Start()
    {
        StartCoroutine(WaitForGlucoseReading());
    }
    
    private IEnumerator WaitForGlucoseReading()
    {
        float time = 0f;
        float lastReading = 150f;
        while (enabled)
        {
            float reading = Random.Range(lastReading + 1f, lastReading + 5f);
            lastReading = reading;
            graph.AddReading(new BloodGlucoseReading(time, reading));
            yield return new WaitForSeconds(realTimeBetweenReadings);
            time += simulatedTimeBetweenReadings;
        }
    }
}
