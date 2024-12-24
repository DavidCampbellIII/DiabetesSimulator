using System.Collections;
using System.Collections.Generic;
using MyBox;
using Sirenix.OdinInspector;
using UnityEngine;

public class CentralDataHub : SingletonMonoBehaviour<CentralDataHub>
{
    [FoldoutGroup("REFERENCES", expanded:true), SerializeField, MustBeAssigned]
    private WorkTaskManager _workTaskManager;
    public static WorkTaskManager workTaskManager => instance._workTaskManager;
    
    protected override void Awake()
    {
        if(!InitializeSingleton(this))
        {
            return;
        }
        
        Init();
    }
    
    private void Init()
    {
        workTaskManager.Init();
    }
}
