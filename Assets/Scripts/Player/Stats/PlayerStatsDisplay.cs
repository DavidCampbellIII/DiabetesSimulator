using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerStatsDisplay : MonoBehaviour
{
    [FoldoutGroup("STAT BARS", expanded:true), SerializeField, MustBeAssigned]
    private RectTransform sleepBar;
    [FoldoutGroup("STAT BARS"), SerializeField, MustBeAssigned]
    private RectTransform hungerBar;
    [FoldoutGroup("STAT BARS"), SerializeField, MustBeAssigned]
    private RectTransform happinessBar;
    [FoldoutGroup("STAT BARS"), SerializeField, MustBeAssigned]
    private RectTransform socialPerceptionBar;
    [FoldoutGroup("STAT BARS"), SerializeField, PositiveValueOnly]
    private float barTransitionTime = 1f;
    
    [FoldoutGroup("PREVIEW SETTINGS"), SerializeField, MustBeAssigned]
    private RectTransform previewBar;
    [FoldoutGroup("PREVIEW SETTINGS"), SerializeField]
    private Color gainPreviewColor = Color.green.WithAlphaSetTo(0.5f);
    [FoldoutGroup("PREVIEW SETTINGS"), SerializeField]
    private Color lossPreviewColor = Color.red.WithAlphaSetTo(0.5f);
    [FoldoutGroup("PREVIEW SETTINGS"), SerializeField]
    private float previewFlashDuration = 0.5f;
    [FoldoutGroup("PREVIEW SETTINGS"), SerializeField]
    private Color previewFlashColor = Color.white.WithAlphaSetTo(0.5f);
    
    //TODO
    
    public void UpdateSleepBar(float value)
    {
        UpdateBar(sleepBar, value);
    }
    
    public void UpdateHungerBar(float value)
    {
        UpdateBar(hungerBar, value);
    }
    
    public void UpdateHappinessBar(float value)
    {
        UpdateBar(happinessBar, value);
    }
    
    public void UpdateSocialPerceptionBar(float value)
    {
        UpdateBar(socialPerceptionBar, value);
    }
    
    private void UpdateBar(RectTransform bar, float value)
    {
        bar.DOKill();
        bar.DOScaleX(value, barTransitionTime);
    }
}
