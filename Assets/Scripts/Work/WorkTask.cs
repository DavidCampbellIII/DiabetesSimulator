using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public abstract class WorkTask : MonoBehaviour
{
    [SerializeField, PositiveValueOnly,
        Tooltip("Time in seconds this task is expected to take")]
    private int expectedTimeToComplete = 30;
    
    private Action<WorkTaskCompletionData> onTaskCompleted;
    
    public void Init(Action<WorkTaskCompletionData> onTaskCompleted)
    {
        this.onTaskCompleted = onTaskCompleted;
    }
    
    public abstract void SetupTask();
    
    protected void FinishTask(WorkTaskCompletionData taskCompletionData)
    {
        onTaskCompleted.Invoke(taskCompletionData);
    }
}
