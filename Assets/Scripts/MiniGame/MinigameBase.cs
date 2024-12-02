using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MiniGameBase : NetworkBehaviour
{
    [SyncVar]
    public int maxPlayers = 1;

    public List<CustomGamePlayer> currentPlayers = new List<CustomGamePlayer>();

    [Server]
    public bool RegisterPlayer(NetworkConnectionToClient conn, CustomGamePlayer player)
    {
        if (currentPlayers.Count >= maxPlayers)
        {
            return false;
        }

        currentPlayers.Add(player);
        NotifyPlayerRegistered(conn);
        return true;
    }

    [TargetRpc]
    private void NotifyPlayerRegistered(NetworkConnectionToClient conn)
    {
        Debug.Log("Player successfully registered.");
    }

    public bool IsPlayerRegistered(CustomGamePlayer player)
    {
        return currentPlayers.Contains(player);
    }
}
