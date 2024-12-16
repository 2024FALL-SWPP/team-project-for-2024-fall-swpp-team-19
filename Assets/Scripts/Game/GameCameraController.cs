using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Mirror.Examples.Basic;

public class GameCameraController : MonoBehaviour
{
    public float mouseSensitivity = 100.0f;

    public Vector3 cameraOffset = new Vector3(0, 2, -4);

    private Transform gamePlayerTransform;
    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    void Update()
    {
        EnsureGamePlayer();

        if (gamePlayerTransform == null) return;
        mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 100.0f);
       
        HandleMouseInput();
        UpdateCameraPositionAndRotation();
    }

    private void HandleMouseInput()
    {
        Debug.Log("mouseSensitivity: " + mouseSensitivity);
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        gamePlayerTransform.Rotate(Vector3.up * mouseX);
    }

    private void UpdateCameraPositionAndRotation()
    {
        transform.position = gamePlayerTransform.position + gamePlayerTransform.TransformDirection(cameraOffset);
        transform.rotation = gamePlayerTransform.rotation * Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void EnsureGamePlayer()
    {
        var roomManager = NetworkManager.singleton as NetworkRoomManager;

        if (roomManager == null)
        {
            Debug.LogError("EnsureGamePlayer: NetworkManager is not a NetworkRoomManager.");
            return;
        }

        string activeSceneName = SceneManager.GetActiveScene().name;
        string gameplaySceneName = System.IO.Path.GetFileNameWithoutExtension(roomManager.GameplayScene);

        if (activeSceneName != gameplaySceneName) return;

        if (NetworkClient.localPlayer == null) return;

        if (gamePlayerTransform != NetworkClient.localPlayer.transform)
        {
            gamePlayerTransform = NetworkClient.localPlayer.transform;
        }
    }
}
