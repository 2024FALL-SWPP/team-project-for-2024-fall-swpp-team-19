using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class CustomRoomManager : NetworkRoomManager
{
    
    
    
    public static CustomRoomManager Instance => (CustomRoomManager)NetworkManager.singleton;

    [Header("Title Scene")]
    [Tooltip("The name of the Title Scene to return to.")]
    public string titleSceneName = "TitleScene"; 


  /// <summary>
    /// Clean up host and client data and return to the Title Scene.
    /// </summary>
    public void ReturnToTitle()
    {
        Debug.Log("Cleaning up network state and returning to Title Scene...");

        // Stop host if active
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            Debug.Log("Stopping Host...");
            StopHost();
        }
        // Stop client if connected
        else if (NetworkClient.isConnected)
        {
            Debug.Log("Stopping Client...");
            StopClient();
        }
        // Stop server if only the server is active
        else if (NetworkServer.active)
        {
            Debug.Log("Stopping Server...");
            StopServer();
        }

        // Clear pending network states
        NetworkClient.Shutdown();
        NetworkServer.Shutdown();

        // Return to Title Scene
        Debug.Log($"Loading Title Scene: {titleSceneName}");
        SceneManager.LoadScene(titleSceneName);
    }
    
    
    
    
    
    
    
    
        private string roomCode;
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
            // Prepare our spawn points
            availableSpawns = new List<Vector3>(spawnPositions);
            // Iterate over each connection and replace the RoomPlayer with a GamePlayer
            foreach (var kvp in NetworkServer.connections)
            {
                NetworkConnectionToClient conn = kvp.Value;
                if (conn != null && conn.identity != null && conn.identity.GetComponent<NetworkRoomPlayer>() != null)
                {
                    // Choose a random spawn point
                    int randomIndex = Random.Range(0, availableSpawns.Count);
                    Vector3 spawnPos = availableSpawns[randomIndex];
                    availableSpawns.RemoveAt(randomIndex);
                    // Instantiate the GamePlayer at the chosen spawn position
                    GameObject playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
                    // Replace the existing RoomPlayer for this connection with the newly spawned GamePlayer
                    NetworkServer.ReplacePlayerForConnection(conn, playerInstance, true);
                    Debug.Log($"Spawned GamePlayer for Connection ID {conn.connectionId} at {spawnPos}");
                }
            }
            
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
        else
        {
            // For non-gameplay scenes (like the room scene), call the base method
            base.OnRoomServerSceneChanged(sceneName);
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

    private int GetServerPort()
    {
        var transport = GetComponent<Mirror.TelepathyTransport>();
        return transport != null ? transport.port : 7777;
    }

    public CustomRoomPlayer GetLocalRoomPlayer()
    {
        if (NetworkClient.localPlayer != null)
        {
            Debug.Log("Connection Verified");
            CustomRoomManager customRoomManager = (CustomRoomManager)NetworkManager.singleton;
            foreach (NetworkRoomPlayer roomPlayer in customRoomManager.roomSlots)
            {
                if (roomPlayer.connectionToClient == NetworkClient.localPlayer.connectionToClient) { 
                    Debug.Log("Connection Verified");
                  if (roomPlayer is CustomRoomPlayer customRoomPlayer)
                    {
                        Debug.Log("Room Player Verified");
                        return customRoomPlayer;
                    }
                } 
            }
        }
        Debug.Log("Connection Not Verified");
        return null;
    }
}
