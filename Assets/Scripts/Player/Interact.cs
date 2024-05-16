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
    
    private void Update()
    {
        if (!Physics.Raycast(origin.position, origin.forward, out RaycastHit hit, interactionDistance,
                interactableLayers))
        {
            StatusUI.HideInteractionText();
            return;
        }
        
        IInteractable interactable = hit.collider.GetComponent<IInteractable>();
        if (interactable == null)
        {
            return;
        }
            
        StatusUI.ShowInteractionText($"Use {interactable.interactableName}");
        if(Input.GetKeyDown(KeyCode.E))
        {
            interactable.Interact();
        }
    }
}
