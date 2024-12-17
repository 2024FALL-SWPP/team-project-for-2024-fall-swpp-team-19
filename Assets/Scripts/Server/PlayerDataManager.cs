using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : NetworkBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    public class SyncPlayerDataDictionary : SyncDictionary<ColorEnum, PlayerData> { }
    public SyncPlayerDataDictionary playerDataMap = new SyncPlayerDataDictionary();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
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

}
