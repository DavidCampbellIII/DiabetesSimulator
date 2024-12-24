using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WorkTaskCompletionData
{
    public float accuracy { get; }
    /// <summary>
    /// Ratio between amount of time taken for the task and the expected completion time.
    /// Less than 1 means the task was completed faster than expected.
    /// </summary>
    public float timeCompletionRatio { get; }
    
    public WorkTaskCompletionData(float accuracy, float timeCompletionRatio)
    {
        this.accuracy = accuracy;
        this.timeCompletionRatio = timeCompletionRatio;
    }
}
