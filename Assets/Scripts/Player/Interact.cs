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
        if (Input.GetKeyDown(KeyCode.E) && 
            Physics.Raycast(origin.position, origin.forward, out RaycastHit hit, interactionDistance, interactableLayers))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            interactable?.Interact();
        }
    }
}
