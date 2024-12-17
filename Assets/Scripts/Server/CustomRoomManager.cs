using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class CustomRoomManager : NetworkRoomManager
{
   
    public static CustomRoomManager Instance => (CustomRoomManager)NetworkManager.singleton;

    private string roomCode;
    public string GetRoomCode()
    {
        return roomCode;
    }

    public void SetRoomCode(string code)
    {
        roomCode = code;
        Debug.Log($"Room Code manually set to: {roomCode}");
    }
 
    private string GenerateRoomCode()
    {
        string externalIP = ExternalIPHelper.GetExternalIPAddress();
        int port = GetServerPort();

        if (externalIP == "Unknown")
        {
            Debug.LogError("Unable to generate room code: External IP not found.");
            return "Error: No IP";
        }

        string rawData = $"{externalIP}:{port}";
        return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(rawData));
    }

    

    public enum StartupMode
    {
        None,
        Host,
        Client
    }

    [Header("Startup Configuration")]
    [Tooltip("Determines what action to take when the LobbyScene starts.")]
    public StartupMode startupMode = StartupMode.None;
    // Called when the LobbyScene starts
    public void HandleLobbyStartup()
    {
        switch (startupMode)
        {
            case StartupMode.Host:
                if (!NetworkServer.active && !NetworkClient.active)
                {
                    Debug.Log("Starting as Host...");
                    StartHost();
                }
                else
                {
                    Debug.LogWarning("Host or Client already active.");
                }
                break;

            case StartupMode.Client:
                if (!NetworkClient.isConnected)
                {
                    Debug.Log("Starting as Client...");
                    StartClient();
                }
                else
                {
                    Debug.LogWarning("Client already connected.");
                }
                break;

            case StartupMode.None:
            default:
                Debug.Log("No action specified for LobbyScene startup.");
                break;
        }
        startupMode = StartupMode.None;
    }
    
    public override void OnStartHost()
    {
        base.OnStartHost();
        roomCode = GenerateRoomCode();
        Debug.Log($"Room Code (External IP): {roomCode}");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server has started.");
    }

    public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
    {
        // In the room scene, let the base method create the RoomPlayer.
        base.OnRoomServerAddPlayer(conn);
        Debug.Log($"Player added to room: Connection ID {conn.connectionId}");
    }

    public override void OnRoomServerPlayersReady()
    {
        // Call base to handle the normal transition to the gameplay scene
        base.OnRoomServerPlayersReady();
        Debug.Log("All players are ready. Transitioning to the game...");
    }

    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        // Retrieve the color from the RoomPlayer
        ColorEnum playerColor = roomPlayer.GetComponent<CustomRoomPlayer>().GetColor();
        gamePlayer.GetComponent<CustomGamePlayer>().SetColor(playerColor);
        return true;
    }

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        // Get the CustomRoomPlayer component to access the color
        CustomRoomPlayer customRoomPlayer = roomPlayer.GetComponent<CustomRoomPlayer>();
        ColorEnum playerColor = customRoomPlayer != null ? customRoomPlayer.GetColor() : ColorEnum.Undefined;

        // Choose the corresponding prefab based on the color
        GameObject selectedPrefab = GetPrefabForColor(playerColor);

        // Fallback to the default playerPrefab if no prefab matches the color
        if (selectedPrefab == null)
        {
            Debug.LogWarning($"No prefab found for color: {playerColor}. Using default playerPrefab.");
            selectedPrefab = playerPrefab;

        }

        // Choose a random spawn position
        if (availableSpawns == null || availableSpawns.Count == 0)
        {
            availableSpawns = new List<Vector3>(spawnPositions); // Reset spawn positions if needed
        }

        int randomIndex = Random.Range(0, availableSpawns.Count);
        Vector3 spawnPosition = availableSpawns[randomIndex];
        availableSpawns.RemoveAt(randomIndex); // Prevent reusing the same position

        // Instantiate the selected prefab at the chosen spawn position
        GameObject gamePlayer = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"Created GamePlayer with color: {playerColor} at position: {spawnPosition}");

        return gamePlayer;
    }





    [Header("Game Player Spawn Settings")]
    [Tooltip("Positions in the Game scene where players can spawn.")]
    public Vector3[] spawnPositions;
    private List<Vector3> availableSpawns;   


    [Header("Minigame Device Spawn Settings")]
    [Tooltip("Positions and Rotations in the Game scene where Minigame can spawn.")]
    public Vector3[] deviceSpawnPositions;
    public Vector3[] deviceSpawnRotations;
    public Vector3[] deviceLateSpawnPositions;
    public Vector3[] deviceLateSpawnRotations;


    public override void OnRoomServerSceneChanged(string sceneName)
    {
        base.OnRoomServerSceneChanged(sceneName);
        Debug.Log($"Scene changed to: {sceneName}");
        // If this is the gameplay scene, handle player spawning ourselves
        if (sceneName == GameplayScene)
        {
            // Prepare out spawn points of devices
            for (int i = 0; i < deviceSpawnPositions.Length; i++)
            {
                Vector3 position = deviceSpawnPositions[i];
                Vector3 rotation = deviceSpawnRotations[i];

                int randomIndex = Random.Range(0, 4);
                GameObject minigameDevice = Instantiate(spawnPrefabs[randomIndex], position, Quaternion.Euler(rotation.x, rotation.y, rotation.z));
                NetworkServer.Spawn(minigameDevice);
            }

            StartCoroutine(LateSpawn());
        }
    }

    private IEnumerator LateSpawn()
    {
        yield return new WaitForSeconds(10f);

        // Prepare out spawn points of devices
        for (int i = 0; i < deviceLateSpawnPositions.Length; i++)
        {
            Vector3 position = deviceLateSpawnPositions[i];
            Vector3 rotation = deviceLateSpawnRotations[i];

            int randomIndex = Random.Range(0, 4);
            GameObject minigameDevice = Instantiate(spawnPrefabs[randomIndex], position, Quaternion.Euler(rotation.x, rotation.y, rotation.z));
            NetworkServer.Spawn(minigameDevice);
        }
    }

    [Header("Player Prefabs")]
    public GameObject redPlayerPrefab;
    public GameObject bluePlayerPrefab;
    public GameObject greenPlayerPrefab;
    public GameObject blackPlayerPrefab;
    public GameObject yellowPlayerPrefab;
    public GameObject pinkPlayerPrefab;
    public GameObject whitePlayerPrefab;
    public GameObject purplePlayerPrefab;

    private GameObject GetPrefabForColor(ColorEnum colorEnum)
    {
        switch (colorEnum)
        {
            case ColorEnum.Red:
                return redPlayerPrefab;
            case ColorEnum.Blue:
                return bluePlayerPrefab;
            case ColorEnum.Green:
                return greenPlayerPrefab;
            case ColorEnum.Black:
                return blackPlayerPrefab;
            case ColorEnum.Yellow:
                return yellowPlayerPrefab;
            case ColorEnum.Pink:
                return pinkPlayerPrefab;
            case ColorEnum.White:
                return whitePlayerPrefab;
            case ColorEnum.Purple:
                return purplePlayerPrefab;

            default:
                return null;
        }
    }

    private int GetServerPort()
    {
        var transport = GetComponent<Mirror.TelepathyTransport>();
        return transport != null ? transport.port : 7777;
    }

    // public CustomRoomPlayer GetLocalRoomPlayer()
    // {
    //     if (NetworkClient.localPlayer != null)
    //     {
    //         Debug.Log("Connection Verified");
    //         CustomRoomManager customRoomManager = (CustomRoomManager)NetworkManager.singleton;
    //         foreach (NetworkRoomPlayer roomPlayer in customRoomManager.roomSlots)
    //         {
    //             if (roomPlayer.connectionToClient == NetworkClient.localPlayer.connectionToClient) { 
    //                 Debug.Log("Connection Verified");
    //               if (roomPlayer is CustomRoomPlayer customRoomPlayer)
    //                 {
    //                     Debug.Log("Room Player Verified");
    //                     return customRoomPlayer;
    //                 }
    //             } 
    //         }
    //     }
    //     Debug.Log("Connection Not Verified");
    //     return null;
    // }

    [Header("Title Scene")]
    [Tooltip("The name of the Title Scene to return to.")]
    public string titleSceneName = "TitleScene";


    public void ReturnToTitle()
    {
        Debug.Log("Cleaning up network state and returning to Title Scene...");
            if (NetworkServer.active || NetworkClient.isConnected)
            {
                Debug.Log("Disconnecting network connections...");
                NetworkClient.Disconnect();
                NetworkServer.DisconnectAll();
            }
            // Shutdown networking without destroying the manager
            NetworkClient.Shutdown();
            NetworkServer.Shutdown();
            // Load Title Scene
            SceneManager.LoadScene(titleSceneName);
    }

    
}
