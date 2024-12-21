using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorkTask : MonoBehaviour
{
    //TODO
    
    public abstract void SetupTask();
    
    protected void FinishTask()
    {
        //TODO invoke callback to WorkTaskManager so
        //next task can be started and stats can be updated
        //send back info about accuracy and time taken for task
        //so money stats and work performance stats (for firing and promotions)
        //can be updated correctly
    }
}
