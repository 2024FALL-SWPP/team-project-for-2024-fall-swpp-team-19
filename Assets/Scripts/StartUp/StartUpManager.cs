using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class StartUpManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("The name of the next scene to load after startup.")]
    [SerializeField] private string nextSceneName = "TitleScene";

    [Header("NetworkManager Settings")]
    [Tooltip("The prefab for the NetworkManager.")]
    [SerializeField] private NetworkManager networkManagerPrefab;

    [Tooltip("The prefab for the PlayerDataManager.")]
    [SerializeField] private PlayerDataManager playerDataManagerPrefab;

    void Start()
    {
        InitializeNetworkManager();
        InitializePlayerDataManager();
        LoadNextScene();
    }

    private void InitializeNetworkManager()
    {
        if (NetworkManager.singleton == null)
        {
            if (networkManagerPrefab != null)
            {
                // Instantiate NetworkManager from prefab
                Instantiate(networkManagerPrefab);
                Debug.Log("NetworkManager initialized from prefab.");
            }
            else
            {
                Debug.LogError("NetworkManagerPrefab is not assigned in the Inspector!");
            }
        }
        else
        {
            Debug.Log("NetworkManager already exists.");
        }
    }

    private void InitializePlayerDataManager()
    {
        if (PlayerDataManager.Instance == null)
        {
            if (playerDataManagerPrefab != null)
            {
                // Instantiate PlayerDataManager from prefab
                Instantiate(playerDataManagerPrefab);
                Debug.Log("PlayerDataManager initialized from prefab.");
            }
            else
            {
                Debug.LogError("PlayerDataManagerPrefab is not assigned in the Inspector!");
            }
        }
        else
        {
            Debug.Log("PlayerDataManager already exists.");
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next scene name is not set in the Inspector!");
        }
    }
}
