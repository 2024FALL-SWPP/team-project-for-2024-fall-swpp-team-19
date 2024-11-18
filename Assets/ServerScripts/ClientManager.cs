using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private readonly List<PlayerData> players = new List<PlayerData>();
    public void RegisterPlayer(PlayerData player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }

    public void UnregisterPlayer(PlayerData player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
        }
    }

    public List<PlayerData> GetPlayers()
    {
        return new List<PlayerData>(players);
    }
}