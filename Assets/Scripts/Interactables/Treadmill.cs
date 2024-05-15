using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
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
    
    [SerializeField, PositiveValueOnly]
    private float timeBetweenSteps = 1.0f;
    [SerializeField, PositiveValueOnly]
    private float insulinSensitivityIncrease = 3f;
    
    private bool isOnTreadmill = false;
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
        
        //TODO prompt the user to press a key to take a step
        //TODO slow down simulation time for BG simulator
        //TODO tie exercise insulin sensitivity to BPM increased from treadmill via PlayerVitals new script?
        if(Time.time - lastStepTime >= timeBetweenSteps)
        {
            lastStepTime = Time.time;
            bgSimulator.AddToExerciseInsulinSensitivity(insulinSensitivityIncrease);
        }
    }

    public void Interact()
    {
        isOnTreadmill = true;
        
        player.position = playerWalkingPoint.position;
        player.rotation = playerWalkingPoint.rotation;
        player.GetComponent<PlayerActionManager>().ToggleInActivity(true);
    }
}
