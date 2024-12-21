using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class WorkTaskManager : MonoBehaviour
{
    [SerializeField, MustBeAssigned]
    private WorkTask[] workTasks;
    
    private int lastTaskIndex = -1;
    
    public void StartWork()
    {
        WorkTask task = PickRandomTask();
        task.SetupTask();
    }
    
    private WorkTask PickRandomTask()
    {
        int randomIndex = Random.Range(0, workTasks.Length);
        if (randomIndex == lastTaskIndex)
        {
            randomIndex = (randomIndex + 1) % workTasks.Length;
        }
        lastTaskIndex = randomIndex;
        return workTasks[randomIndex];
    }
}
