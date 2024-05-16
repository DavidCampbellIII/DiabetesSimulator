using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsDisplay : MonoBehaviour
{
    #region Nested Structures
    
    private class PreviewBarTracker
    {
        private readonly RectTransform bar;
        private readonly RectTransform parent;
        private readonly float value;
        
        private Tweener scaleTween;
        private float remainingDuration;
        
        public PreviewBarTracker(RectTransform bar, RectTransform parent, float value)
        {
            this.bar = bar;
            this.parent = parent;
            this.value = value;
            
            scaleTween = null;
        }
        
        public void Update(float previewGrowDuration)
        {
            remainingDuration -= Time.deltaTime;
            
            float parentScale = parent.localScale.x;
            float result = parentScale + value;
            float targetValue = result switch
            {
                < 0f => parentScale,
                > 1f => 1f - parentScale,
                _ => value
            };
            float targetScale = Mathf.Abs(targetValue) / parent.localScale.x;

            scaleTween?.ManualUpdate(Time.deltaTime, Time.unscaledDeltaTime);

            if(scaleTween != null && !scaleTween.IsComplete())
            {
                scaleTween.ChangeValues(bar.localScale, 
                    new Vector3(targetScale, 1f, 1f), remainingDuration);
                return;
            }
            
            scaleTween.Kill();
            scaleTween = bar.DOScaleX(targetScale, previewGrowDuration)
                .SetEase(Ease.Linear)
                .SetAutoKill(false)
                .SetUpdate(UpdateType.Manual);
            remainingDuration = previewGrowDuration;
        }
        
        public void DestroyBar()
        {
            bar.DOKill();
            bar.GetComponent<Image>().DOKill();
            Destroy(bar.gameObject);
        }
    }
    
    #endregion
    
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
    private RectTransform previewBarPrefab;
    [FoldoutGroup("PREVIEW SETTINGS"), SerializeField]
    private Color gainPreviewColor = Color.green.WithAlphaSetTo(0.5f);
    [FoldoutGroup("PREVIEW SETTINGS"), SerializeField]
    private Color lossPreviewColor = Color.red.WithAlphaSetTo(0.5f);
    [FoldoutGroup("PREVIEW SETTINGS"), SerializeField, PositiveValueOnly]
    private float previewGrowDuration = 0.25f;
    [FoldoutGroup("PREVIEW SETTINGS"), SerializeField, PositiveValueOnly]
    private float previewFlashDuration = 0.5f;
    [FoldoutGroup("PREVIEW SETTINGS"), SerializeField]
    private Color previewFlashColor = Color.white.WithAlphaSetTo(0.5f);
    
    private readonly List<PreviewBarTracker> previewBars = new List<PreviewBarTracker>();
    
    #region Preview Bars

    private void LateUpdate()
    {
        foreach(PreviewBarTracker tracker in previewBars)
        {
            tracker.Update(previewGrowDuration);
        }
    }

    [Button]
    public void ClearPreviewBars()
    {
        foreach(PreviewBarTracker tracker in previewBars)
        {
            tracker.DestroyBar();
        }
        previewBars.Clear();
    }
    
    [Button]
    public void PreviewSleepBar(float deltaValue)
    {
        PreviewBar(sleepBar, deltaValue);
    }

    private void PreviewBar(RectTransform bar, float value)
    {
        RectTransform previewBar = Instantiate(previewBarPrefab, bar);
        Image barImage = previewBar.GetComponent<Image>();
        
        bool isGain = value > 0f;
        Color targetColor = isGain ? gainPreviewColor : lossPreviewColor;
        barImage.color = targetColor;
        barImage.DOColor(previewFlashColor, previewFlashDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        
        float xPivot = isGain ? 0f : 1f;
        previewBar.pivot = new Vector2(xPivot, 0.5f);
        previewBar.anchoredPosition = Vector2.zero;
        //start at a scale of 0 so we can see the preview bar grow in either direction
        previewBar.localScale = new Vector3(0f, 1f, 1f);
        
        previewBars.Add(new PreviewBarTracker(previewBar, bar, value));
    }
    
    #endregion
    
    #region Update Bars
    
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
    
    #endregion
}
