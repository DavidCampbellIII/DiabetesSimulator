using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class PlayerActionManager : MonoBehaviour
{
    [SerializeField, MustBeAssigned]
    private PlayerMovement movement;
    [SerializeField, MustBeAssigned]
    private Interact interact;
    [SerializeField, MustBeAssigned]
    private Phone phone;
    
    public void ToggleInActivity(bool isActive)
    {
        movement.enabled = !isActive;
        interact.enabled = !isActive;
    }

    private void Update()
    {
        if(Input.GetButtonDown("PeekBG"))
        {
            if(phone.state == PhoneState.AWAY)
            {
                phone.Peek();
            }
            else if(phone.state == PhoneState.PEEKING)
            {
                phone.PutAway();
            }
        }
        else if(Input.GetButtonDown("CheckBG"))
        {
            if(phone.state == PhoneState.AWAY || phone.state == PhoneState.PEEKING)
            {
                phone.Check();
            }
            else if(phone.state == PhoneState.CHECKING)
            {
                phone.PutAway();
            }
        }
    }
}
