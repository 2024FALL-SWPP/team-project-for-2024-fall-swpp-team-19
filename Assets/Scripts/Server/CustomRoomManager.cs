using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class CustomRoomManager : NetworkRoomManager
{
    private string roomCode;

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

    public override void OnRoomServerSceneChanged(string sceneName)
    {
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
        }
        else
        {
            // For non-gameplay scenes (like the room scene), call the base method
            base.OnRoomServerSceneChanged(sceneName);
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
