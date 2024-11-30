using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    public GameObject gameButtonGroup;
    public GameObject hostInputField;
    public GameObject controlButtonGroup;
    public GameObject backButton;

    private NetworkManager networkManager;
    private TMP_InputField inputField;

    private bool isConnecting = false;
    private string serverRoomCode = "";

    void Start()
    {
        gameButtonGroup.SetActive(false);
        hostInputField.SetActive(false);
        networkManager = CustomRoomManager.singleton;

        inputField = hostInputField.GetComponent<TMP_InputField>();
        if (inputField != null)
        {
            inputField.onEndEdit.AddListener(OnEndEditInputField);
        }

        NetworkClient.OnConnectedEvent += OnClientConnected;
        NetworkClient.OnDisconnectedEvent += OnClientDisconnected;
    }

    public void PlayGameButton()
    {
        gameButtonGroup.SetActive(true);
        controlButtonGroup.SetActive(false);
        hostInputField.SetActive(false);
        backButton.SetActive(false);
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void CreateLobbyButton()
    {
        if (networkManager != null)
        {
            networkManager.StartHost();

            Debug.Log("Host started. NetworkRoomManager will handle scene transition.");
        }
        else
        {
            Debug.LogError("NetworkManager is not found.");
        }
    }

    public void JoinButton()
    {
        hostInputField.SetActive(true);
        gameButtonGroup.SetActive(false);
        backButton.SetActive(true);
        if (inputField != null)
        {
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    private void OnEndEditInputField(string roomCode)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ConnectToServer();
        }
    }

    public void ConnectToServer()
    {
        if (networkManager != null && !isConnecting)
        {
            if (!string.IsNullOrEmpty(inputField.text))
            {
                string roomCode = inputField.text;
                string decoded = DecodeRoomCode(roomCode);

                if (!string.IsNullOrEmpty(decoded))
                {
                    string[] splitData = decoded.Split(':');
                    if (splitData.Length == 2)
                    {
                        string ipAddress = splitData[0];
                        int port;
                        if (int.TryParse(splitData[1], out port) && port >= 0 && port <= 65535)
                        {
                            Debug.Log($"Attempting to connect to IP: {ipAddress}, Port: {port}");

                            networkManager.networkAddress = ipAddress;

                            var transport = networkManager.GetComponent<Mirror.TelepathyTransport>();
                            if (transport != null)
                            {
                                transport.port = (ushort)port;
                            }

                            serverRoomCode = roomCode;
                            isConnecting = true;

                            NetworkClient.Connect(ipAddress);
                            Debug.Log("Checking room existence...");
                        }
                        else
                        {
                            Debug.LogError("Invalid port in room code.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Invalid room code format.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to decode room code.");
                }
            }
            else
            {
                Debug.LogWarning("Please enter a valid room code.");
            }
        }
        else if (isConnecting)
        {
            Debug.LogWarning("Already attempting to connect...");
        }
        else
        {
            Debug.LogError("NetworkManager is not found.");
        }
    }

    private void OnClientConnected()
    {
        Debug.Log("Client successfully connected to the server.");
        isConnecting = false;

        var customRoomManager = (CustomRoomManager)networkManager;
        customRoomManager.SetRoomCode(serverRoomCode);

        SceneManager.LoadScene("LobbyScene");
    }

    private void OnClientDisconnected()
    {
        Debug.LogError("Failed to connect to the server or disconnected.");
        isConnecting = false;
    }

    private string DecodeRoomCode(string roomCode)
    {
        try
        {
            byte[] data = System.Convert.FromBase64String(roomCode);
            return System.Text.Encoding.UTF8.GetString(data);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error decoding room code: {ex.Message}");
            return null;
        }
    }
}
