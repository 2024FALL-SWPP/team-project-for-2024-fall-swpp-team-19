// using UnityEngine;

// public class MouseLook : MonoBehaviour
// {
//     public Transform playerBody;
//     public Settings settings;

//     private float xRotation = 0f;

//     void Start()
//     {
//         Cursor.lockState = CursorLockMode.Locked;
//     }

//     void Update()
//     {
//         float mouseX = Input.GetAxis("Mouse X") * settings.GetMouseSensitivity();
//         float mouseY = Input.GetAxis("Mouse Y") * settings.GetMouseSensitivity();

//         xRotation -= mouseY;
//         xRotation = Mathf.Clamp(xRotation, -90f, 90f);

//         transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
//         playerBody.Rotate(Vector3.up * mouseX);
//     }
// }