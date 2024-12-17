using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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



    [Header("Game Player Spawn Settings")]
    [Tooltip("Positions in the Game scene where players can spawn.")]
    public Vector3[] spawnPositions;
    private List<Vector3> availableSpawns;

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


    bool hasEntered;
    
    public override void OnRoomServerSceneChanged(string sceneName)
    {
        base.OnRoomServerSceneChanged(sceneName);
        if (sceneName == GameplayScene)
        {
            if (!hasEntered)
            {
                Debug.Log("Spawning players...");
                hasEntered = true;

                // Prepare our spawn points
                availableSpawns = new List<Vector3>(spawnPositions);

                // Iterate over each RoomPlayer in roomSlots
                foreach (var roomPlayer in roomSlots)
                {
                    if (roomPlayer != null && roomPlayer.connectionToClient != null)
                    {
                        // Ensure the room player is of type CustomRoomPlayer
                        if (roomPlayer is CustomRoomPlayer customRoomPlayer)
                        {
                            // Get the player's selected color (or other property)
                            ColorEnum colorEnum = customRoomPlayer.GetColor();

                            // Choose the prefab to spawn based on the colorEnum
                            GameObject prefabToSpawn = GetPrefabForColor(colorEnum);

                            if (prefabToSpawn == null)
                            {
                                Debug.LogError($"No prefab found for color: {colorEnum}");
                                continue;
                            }

                            // Choose a random spawn point
                            int randomIndex = Random.Range(0, availableSpawns.Count);
                            Vector3 spawnPos = availableSpawns[randomIndex];
                            availableSpawns.RemoveAt(randomIndex);

                            // Instantiate the GamePlayer prefab at the chosen spawn position
                            GameObject playerInstance = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

                            // Replace the existing RoomPlayer for this connection with the newly spawned GamePlayer
                            NetworkServer.ReplacePlayerForConnection(roomPlayer.connectionToClient, playerInstance, true);

                            Debug.Log($"Spawned GamePlayer for Connection ID {roomPlayer.connectionToClient.connectionId} at {spawnPos}");
                        }
                    }
                }
            }
        }
        else
        {
            hasEntered = false;
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
