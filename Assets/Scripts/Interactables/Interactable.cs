using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    #region Nested Structures
    
    [System.Serializable]
    private struct PlayerStatAdjustment
    {
        [SerializeField]
        private PlayerStatType _statType;
        public PlayerStatType statType => _statType;
        
        [SerializeField, Range(-1f, 1f)]
        private float _adjustment;
        public float adjustment => _adjustment;
    }
    
    #endregion
    
    [SerializeField, MustBeAssigned]
    private string _interactableName;
    public string interactableName => _interactableName;
    
    [SerializeField, MustBeAssigned]
    private PlayerStatAdjustment[] statAdjustments;
    
    public void StartLookingAt()
    {
        foreach(PlayerStatAdjustment statAdjustment in statAdjustments)
        {
            PlayerStatsManager.display.PreviewBar(statAdjustment.statType, statAdjustment.adjustment);
        }
    }

    public void StopLookingAt()
    {
        PlayerStatsManager.display.ClearPreviewBars();
    }
    
    public void Interact()
    {
        foreach(PlayerStatAdjustment statAdjustment in statAdjustments)
        {
            PlayerStatsManager.AdjustStatValue(statAdjustment.statType, statAdjustment.adjustment);
        }
        Interact_Internal();
    }
    
    protected abstract void Interact_Internal();
}
