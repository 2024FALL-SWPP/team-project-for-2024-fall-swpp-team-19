using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUpManager : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "TitleScene";

    void Start()
    {
        InitializeGame();
        LoadNextScene();
    }

    private void InitializeGame()
    {
        LoadNetworkManager();
        LoadGameSettings();
    }

    private void LoadNetworkManager()
    {
        if (CustomRoomManager.singleton == null)
        {
            Debug.LogError("CustomRoomManager not found.");
        }
    }

    private void LoadGameSettings()
    {
        Debug.Log("Game settings loaded.");
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
