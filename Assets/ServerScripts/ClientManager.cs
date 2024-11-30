using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : NetworkBehaviour
{
    private readonly List<PlayerData> players = new List<PlayerData>();

    [SerializeField] private GameObject playerPrefab; // Assign in Inspector

    public void RegisterPlayer(PlayerData player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
            Debug.Log($"Player added: {player.PlayerName} (ID: {player.PlayerId})");
            PrintCurrentState();
        }
    }

    public void UnregisterPlayer(PlayerData player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
            Debug.Log($"Player removed: {player.PlayerName} (ID: {player.PlayerId})");
            PrintCurrentState();
        }
    }

    public List<PlayerData> GetPlayers()
    {
        return new List<PlayerData>(players);
    }

    private void PrintCurrentState()
    {
        if (players.Count == 0)
        {
            Debug.Log("No players currently registered.");
        }
        else
        {
            Debug.Log("Current Players:");
            foreach (var player in players)
            {
                Debug.Log($"- {player.PlayerName} (ID: {player.PlayerId})");
            }
        }
    }

    private void Update()
    {
        if (isServer && Input.GetKeyDown(KeyCode.Return)) // Ensure only the server spawns players
        {
            SpawnPlayerPrefab();
        }
    }

    [Server]
    private void SpawnPlayerPrefab()
    {
        foreach (PlayerData player in players)
        {
            // Check if a player prefab is already spawned for the player
            // Optionally, you can add a system to track spawned prefabs
            Vector3 spawnPosition = new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5));
            GameObject playerObject = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            NetworkServer.Spawn(playerObject);

            Debug.Log($"Spawned player prefab for {player.PlayerName} at {spawnPosition}");
        }
    }
}
