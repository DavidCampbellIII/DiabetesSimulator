using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;

public class Treadmill : MonoBehaviour, IInteractable
{
    [SerializeField, MustBeAssigned]
    private BloodGlucoseSimulator bgSimulator;
    [SerializeField, MustBeAssigned]
    private Transform player;
    [SerializeField, MustBeAssigned]
    private Transform playerWalkingPoint;
    [SerializeField, MustBeAssigned]
    private Transform dismountPoint;
    [SerializeField, MustBeAssigned]
    private TextMeshPro timerText;
    
    [SerializeField, PositiveValueOnly]
    private float timeBetweenSteps = 1.0f;
    [SerializeField, PositiveValueOnly]
    private float insulinSensitivityIncrease = 3f;
    
    private bool isOnTreadmill = false;
    private float timeStarted = 0.0f;
    private float lastStepTime = 0.0f;
    
    private void Update()
    {
        if(!isOnTreadmill)
        {
            return;
        }
        
        if(Input.GetButtonDown("Cancel"))
        {
            isOnTreadmill = false;
            player.position = dismountPoint.position;
            player.rotation = dismountPoint.rotation;
            player.GetComponent<PlayerActionManager>().ToggleInActivity(false);
            return;
        }
        
        timerText.text = TimeOfDay.FromSeconds((int)(TimeManager.time - timeStarted)).ToString()[..^2];
        
        //TODO prompt the user to press a key to take a step
        //TODO slow down simulation time for BG simulator
        //TODO tie exercise insulin sensitivity to BPM increased from treadmill via PlayerVitals new script?
        if(TimeManager.time - lastStepTime >= timeBetweenSteps)
        {
            lastStepTime = TimeManager.time;
            bgSimulator.AddToExerciseInsulinSensitivity(insulinSensitivityIncrease);
        }
    }

    public void Interact()
    {
        isOnTreadmill = true;
        timeStarted = TimeManager.time;
        
        player.position = playerWalkingPoint.position;
        player.rotation = playerWalkingPoint.rotation;
        player.GetComponent<PlayerActionManager>().ToggleInActivity(true);
    }
}
