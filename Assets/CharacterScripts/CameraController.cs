using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 100.0f;
    private Transform playerBody;
    private float xRotation = 0f;

    void Start()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;

        // Reference the parent object, which should be the player’s body
        playerBody = transform.parent;
    }

    void Update()
    {
        // Get the mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Vertical rotation: Adjusts camera pitch
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp to prevent flipping
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal rotation: Rotates the player body horizontally
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
