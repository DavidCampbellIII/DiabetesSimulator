using System.Collections;
using System.Collections.Generic;
using MyBox;
using Sirenix.OdinInspector;
using UnityEngine;

public enum EmploymentStatusChange
{
    NONE,
    PROMOTED,
    FIRED,
    LAID_OFF
}

public class WorkPerformanceMonitor : MonoBehaviour
{
    [FoldoutGroup("GENERAL", expanded:true), SerializeField, PositiveValueOnly,
        Tooltip("Number of previous tasks that are considered in total task accuracy and time calculations")]
    private int tasksToConsider = 5;
    
    [FoldoutGroup("FIRING", expanded:true), SerializeField, Range(0f, 1f),
        Tooltip("Min accuracy for last X number of tasks to consider to prevent firing")]
    private float minAccuracyPreventFiring = 0.6f;
    [FoldoutGroup("FIRING"), SerializeField, Range(0f, 2f),
        Tooltip("Max time completion ratio for last X number of tasks to consider to prevent firing")]
    private float maxTimeRatioPreventFiring = 1.33f;
    [FoldoutGroup("FIRING"), SerializeField, Range(0f, 1f),
        Tooltip("Chance each day that player can be fired if below the min accuracy or above the max time ratio")]
    private float firedChance = 0.5f;
    [FoldoutGroup("FIRING"), SerializeField, Range(0f, 1f),
        Tooltip("Chance each day that player can be laid off, regardless of work performance")]
    private float laidOffChance = 0.01f;
    
    [FoldoutGroup("PROMOTION", expanded:true), SerializeField, Range(0f, 1f),
        Tooltip("Min accuracy for last X number of tasks to consider to make eligible for promotion")]
    private float minAccuracyForPromotion = 0.9f;
    [FoldoutGroup("PROMOTION"), SerializeField, Range(0f, 2f),
        Tooltip("Min time completion ratio for last X number of tasks to consider to make eligible for promotion")]
    private float minTimeRatioForPromotion = 0.8f;
    [FoldoutGroup("PROMOTION"), SerializeField, Range(0f, 1f),
        Tooltip("Chance each day that player can be promoted if above the min accuracy and below the max time ratio")]
    private float promotionChance = 0.5f;
    
    private readonly List<WorkTaskCompletionData> taskCompletionData = new List<WorkTaskCompletionData>();
    
    public void AddTaskCompletionData(WorkTaskCompletionData data)
    {
        taskCompletionData.Add(data);
    }
    
    public EmploymentStatusChange CheckEmploymentStatusChange()
    {
        if (taskCompletionData.Count < tasksToConsider)
        {
            return EmploymentStatusChange.NONE;
        }
        
        //make sure to never lay off player if they haven't completed enough tasks to consider
        //because unlike life, this game will at least be a little bit fair
        if(laidOffChance.Chance())
        {
            return EmploymentStatusChange.LAID_OFF;
        }
        
        float totalAccuracy = 0f;
        float totalTimeRatio = 0f;
        for (int i = 1; i <= tasksToConsider; i++)
        {
            WorkTaskCompletionData data = taskCompletionData[^i];
            totalAccuracy += data.accuracy;
            totalTimeRatio += data.timeCompletionRatio;
        }
        float averageAccuracy = totalAccuracy / tasksToConsider;
        float averageTimeRatio = totalTimeRatio / tasksToConsider;
        
        bool canFire = averageAccuracy < minAccuracyPreventFiring ||
                       averageTimeRatio >= maxTimeRatioPreventFiring;
        if (canFire &&
            firedChance.Chance())
        {
            return EmploymentStatusChange.FIRED;
        }
        
        bool canPromote = averageAccuracy >= minAccuracyForPromotion &&
                          averageTimeRatio <= minTimeRatioForPromotion;
        if (canPromote &&
            promotionChance.Chance())
        {
            return EmploymentStatusChange.PROMOTED;
        }
        
        return EmploymentStatusChange.NONE;
    }
}
