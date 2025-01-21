using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCameraMovement : MonoBehaviour
{
    public float movementSpeed = 10f; // Speed of movement
    public float rotationSpeed = 2f; // Sensitivity of mouse rotation
    public float smoothTime = 0.2f; // Smoothing for movement
    private Vector3 currentVelocity; // For smooth movement

    private Vector3 inputDirection;

    void Update()
    {
        if (Input.GetMouseButton(1)) 
        {
            HandleMovement();
            HandleRotation();
        }
    }

    /// <summary>
    /// Moving camera with w, s, a , d
    /// </summary>
    void HandleMovement()
    {
        // Get input for movement
        inputDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) inputDirection += transform.forward;
        if (Input.GetKey(KeyCode.S)) inputDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A)) inputDirection -= transform.right;
        if (Input.GetKey(KeyCode.D)) inputDirection += transform.right;

        // Smooth the movement
        Vector3 targetPosition = transform.position + inputDirection.normalized * movementSpeed * Time.deltaTime;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }
    /// <summary>
    // Rotating camera based on Mouse position
    /// </summary>
    void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // Rotate only when right mouse button is pressed
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

            // Rotate around the Y-axis (horizontal rotation)
            transform.Rotate(Vector3.up, mouseX, Space.World);

            // Rotate around the X-axis (vertical rotation)
            transform.Rotate(Vector3.right, -mouseY, Space.Self);
        }
    }
}

