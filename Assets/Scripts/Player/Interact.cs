using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField]
    private float interactionDistance = 5.0f;
    [SerializeField]
    private LayerMask interactableLayers;
    [SerializeField, MustBeAssigned]
    private Transform origin;
    
    private Interactable lookingAtInteractable;
    
    private void Update()
    {
        if (!Physics.Raycast(origin.position, origin.forward, out RaycastHit hit, interactionDistance,
                interactableLayers))
        {
            if(lookingAtInteractable)
            {
                lookingAtInteractable.StopLookingAt();
                StatusUI.HideInteractionText();
                lookingAtInteractable = null;
            }
            return;
        }
        
        Interactable interactable = hit.collider.GetComponent<Interactable>();
        
        // If we were looking at an interactable, but it is not the same as the one we are currently looking at
        if(lookingAtInteractable && lookingAtInteractable != interactable)
        {
            lookingAtInteractable.StopLookingAt();
        }
        
        if (!interactable)
        {
            lookingAtInteractable = null;
            StatusUI.HideInteractionText();
            return;
        }
        
        //if looking at a new interactable
        if(lookingAtInteractable != interactable)
        {
            lookingAtInteractable = interactable;
            lookingAtInteractable.StartLookingAt();
            StatusUI.ShowInteractionText($"Use {lookingAtInteractable.interactableName}");
        }
        
        if(Input.GetKeyDown(KeyCode.E))
        {
            lookingAtInteractable.StopLookingAt();
            lookingAtInteractable.Interact();
            StatusUI.HideInteractionText();
            lookingAtInteractable = null;
        }
    }
}
