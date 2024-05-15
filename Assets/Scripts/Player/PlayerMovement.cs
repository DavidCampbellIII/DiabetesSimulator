using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5.0f;
    [SerializeField]
    private float lookSensitivity = 2.0f;
    [SerializeField, 
        Tooltip("Maximum vertical angle the camera can achieve")]
    private float maxLookAngle = 45.0f;
    [SerializeField, MustBeAssigned]
    private Transform playerCamera;

    private Transform thisTransform;
    private float verticalLookRotation = 0f;

    private void Start()
    {
        thisTransform = transform;
   
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        MoveCharacter();
        LookAround();
    }

    private void MoveCharacter()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 forwardMovement = thisTransform.forward * verticalInput;
        Vector3 rightMovement = thisTransform.right * horizontalInput;

        Vector3 movement = (forwardMovement + rightMovement).normalized * (moveSpeed * Time.deltaTime);
        thisTransform.Translate(movement, Space.World);
    }

    private void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;
        
        thisTransform.Rotate(Vector3.up * mouseX);
        
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -maxLookAngle, maxLookAngle);
        playerCamera.localEulerAngles = Vector3.right * verticalLookRotation;
    }
}
