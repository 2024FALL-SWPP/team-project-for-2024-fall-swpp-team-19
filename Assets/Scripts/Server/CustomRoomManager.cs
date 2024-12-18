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

    
    public override void OnStartHost()
    {
        base.OnStartHost();
        roomCode = GenerateRoomCode();
        Debug.Log($"Room Code (External IP): {roomCode}");
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
        CustomRoomPlayer customRoomPlayer = roomPlayer.GetComponent<CustomRoomPlayer>();
        ColorEnum playerColor = customRoomPlayer != null ? customRoomPlayer.GetColor() : ColorEnum.Undefined;

        // Initialize PlayerData in PlayerDataManager
        PlayerDataManager.Instance.InitializePlayerData(playerColor);

        GameObject selectedPrefab = GetPrefabForColor(playerColor);
        if (selectedPrefab == null)
        {
            Debug.LogWarning($"No prefab found for color: {playerColor}. Using default playerPrefab.");
            selectedPrefab = playerPrefab;
        }

        if (availableSpawns == null || availableSpawns.Count == 0)
        {
            availableSpawns = new List<Vector3>(spawnPositions);
        }

        int randomIndex = Random.Range(0, availableSpawns.Count);
        Vector3 spawnPosition = availableSpawns[randomIndex];
        availableSpawns.RemoveAt(randomIndex);

        GameObject gamePlayer = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        NetworkServer.ReplacePlayerForConnection(conn, gamePlayer, ReplacePlayerOptions.KeepAuthority);

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

    private GameObject[] spawnedDevices = new GameObject[16];
    private GameObject[] lateSpawnedDevices = new GameObject[4];

    public override void OnRoomServerSceneChanged(string sceneName)
    {
        base.OnRoomServerSceneChanged(sceneName);
        Debug.Log($"Scene changed to: {sceneName}");

        // If this is the gameplay scene, handle player spawning ourselves
        if (sceneName == GameplayScene)
        {
            InvokeRepeating(nameof(FirstSpawn), 0f, 60f);
            InvokeRepeating(nameof(LateSpawn), 30f, 60f);

            // Call AssignTargetsInCircle to set player targets
            PlayerDataManager.Instance.AssignTargetsInCircle();
            Debug.Log("Assigned targets for all players in a circle.");
        }
    }

    private void FirstSpawn()
    {
        // Prepare out spawn points of devices
        for (int i = 0; i < deviceSpawnPositions.Length; i++)
        {
            if (spawnedDevices[i] == null)
            {
                Vector3 position = deviceSpawnPositions[i];
                Vector3 rotation = deviceSpawnRotations[i];

                int randomIndex = Random.Range(0, 4);
                spawnedDevices[i] = Instantiate(spawnPrefabs[randomIndex], position, Quaternion.Euler(rotation.x, rotation.y, rotation.z));
                NetworkServer.Spawn(spawnedDevices[i]);
            }
        }
    }

    private void LateSpawn()
    {
        // Prepare out spawn points of devices
        for (int i = 0; i < deviceLateSpawnPositions.Length; i++)
        {
            if (lateSpawnedDevices[i] == null)
            {
                Vector3 position = deviceLateSpawnPositions[i];
                Vector3 rotation = deviceLateSpawnRotations[i];

                int randomIndex = Random.Range(0, 4);
                lateSpawnedDevices[i] = Instantiate(spawnPrefabs[randomIndex], position, Quaternion.Euler(rotation.x, rotation.y, rotation.z));
                NetworkServer.Spawn(lateSpawnedDevices[i]);
            }
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

    


    public void PrintTargetCircle()
    {
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogWarning("PlayerDataManager is not initialized.");
            return;
        }

        var playerDataMap = PlayerDataManager.Instance.playerDataMap;

        if (playerDataMap.Count == 0)
        {
            Debug.Log("No players available to print target circle.");
            return;
        }

        Debug.Log("=== Current Target Circle ===");
        foreach (var player in playerDataMap)
        {
            Debug.Log($"Player {player.Key} -> Targets: {player.Value.target}");
        }
    }

    private bool isGameStarted = false;

    void Update()
    {
        // Debugging: Keep printing the playerDataMap size
        if (PlayerDataManager.Instance != null)
        {
            if(PlayerDataManager.Instance.playerDataMap.Count > 1 && !isGameStarted)
            {
                PlayerDataManager.Instance.AssignTargetsInCircle();
                isGameStarted = true;
            }
            int dataSize = PlayerDataManager.Instance.playerDataMap.Count;
            Debug.Log($"PlayerDataMap Size: {dataSize}");
        }
        PrintTargetCircle();
    }
}
