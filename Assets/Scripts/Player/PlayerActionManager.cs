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
    
    public void ToggleInActivity(bool isActive)
    {
        movement.enabled = !isActive;
        interact.enabled = !isActive;
    }
}
