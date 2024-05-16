using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using TMPro;
using UnityEngine;

public class StatusUI : SingletonMonoBehaviour<StatusUI>
{
    [SerializeField, MustBeAssigned]
    private TextMeshProUGUI interactText;
    [SerializeField, PositiveValueOnly]
    private float interactionTextFadeTime = 0.5f;
    
    [SerializeField, MustBeAssigned]
    private TextMeshProUGUI statusText;
    
    //TODO status text fade and stuff
    
    private bool isFadingInInteractionText = false;

    protected override void Awake()
    {
        if(!InitializeSingleton(this))
        {
            return;
        }
    }

    private void Start()
    {
        interactText.alpha = 0f;
        statusText.alpha = 0f;
    }

    public static void ShowInteractionText(string text)
    {
        if(instance.isFadingInInteractionText)
        {
            return;
        }
        
        instance.interactText.DOKill();
        instance.interactText.DOFade(1f, instance.interactionTextFadeTime);
        instance.interactText.text = text;
        instance.isFadingInInteractionText = true;
    }
    
    public static void HideInteractionText()
    {
        if(!instance.isFadingInInteractionText)
        {
            return;
        }
        
        instance.interactText.DOKill();
        instance.interactText.DOFade(0f, instance.interactionTextFadeTime);
        instance.isFadingInInteractionText = false;
    }
}
