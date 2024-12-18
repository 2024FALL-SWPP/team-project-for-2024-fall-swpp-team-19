using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : NetworkBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    public class SyncPlayerDataDictionary : SyncDictionary<ColorEnum, PlayerData> { }
    public SyncPlayerDataDictionary playerDataMap = new SyncPlayerDataDictionary();


     public override void OnStartServer()
    {
        base.OnStartServer();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple PlayerDataManager instances detected. Destroying the duplicate.");
            Destroy(gameObject);
        }
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    [Server]
    public void InitializePlayerData(ColorEnum color)
    {
        if (!playerDataMap.ContainsKey(color))
        {
            playerDataMap.Add(color, new PlayerData());
            Debug.Log($"Initialized PlayerData for: {color}");
        }
    }

    [Server]
    public PlayerData GetPlayerData(ColorEnum color)
    {
        Debug.Log($"Received color {color}");
        if (playerDataMap.ContainsKey(color))
        {
            Debug.Log($"There's corresponding playerData {color}");
            return playerDataMap[color];
        }
        return null;
    }


    [Command]
    public void CmdUpdatePlayerData(ColorEnum color, string field, object value)
    {
        if (playerDataMap.ContainsKey(color))
        {
            var data = playerDataMap[color];
            data.UpdateField(field, value);
            playerDataMap[color] = data;
            Debug.Log($"Updated {field} for {color}: {value}");
        }
    }

    [Server]
    public void ClearAllData()
    {
        playerDataMap.Clear();
        Debug.Log("Cleared all PlayerData.");
    }

    [Server]
    public void AssignTargetsInCircle()
    {
        if (playerDataMap.Count < 2)
        {
            Debug.LogWarning("Not enough players to form a circle.");
            return;
        }

        // Extract all player colors into a list
        List<ColorEnum> playerColors = new List<ColorEnum>(playerDataMap.Keys);

        // Assign targets in a circular fashion
        for (int i = 0; i < playerColors.Count; i++)
        {
            ColorEnum currentPlayer = playerColors[i];
            ColorEnum nextPlayer = playerColors[(i + 1) % playerColors.Count]; // Wrap around for the last player

            var data = playerDataMap[currentPlayer];
            data.target = nextPlayer; // Update the target field
            playerDataMap[currentPlayer] = data;
        }

        // Print the circle for debugging
        Debug.Log("=== Target Circle Assignment ===");
        foreach (var player in playerDataMap)
        {
            Debug.Log($"Player {player.Key} -> Targets: {player.Value.target}");
        }
    }
    
    [Server]
    public int CountAlivePlayers()
    {
        int aliveCount = 0;

        foreach (var playerData in playerDataMap.Values)
        {
            if (playerData.isAlive) // Assuming PlayerData has an 'isAlive' property
            {
                aliveCount++;
            }
        }

        Debug.Log($"Number of alive players: {aliveCount}");
        return aliveCount;
    }

}
