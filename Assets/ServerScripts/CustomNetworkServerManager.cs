using Mirror;
using UnityEngine;

public class CustomNetworkServerManager : NetworkManager
{
    [SerializeField] private ClientManager clientManager; // ClientManager를 인스펙터에서 설정

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server started.");

        // Warn if ClientManager is not assigned
        if (clientManager == null)
        {
            Debug.LogWarning("ClientManager is not assigned in CustomNetworkManager.");
        }
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        Debug.Log("Host started.");

        // Add the host player data when the host starts
        if (clientManager != null)
        {
            string hostPlayerId = "Host";
            string hostPlayerName = "Host Player";
            PlayerData hostPlayerData = new PlayerData(hostPlayerId, hostPlayerName);

            clientManager.RegisterPlayer(hostPlayerData);
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        if (clientManager != null)
        {
            // Create and register PlayerData for the new player
            string playerId = conn.connectionId.ToString();
            string playerName = $"Player {conn.connectionId}";
            PlayerData playerData = new PlayerData(playerId, playerName);

            clientManager.RegisterPlayer(playerData);
            Debug.Log($"PlayerData created and registered: {playerName} (ID: {playerId})");
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (clientManager != null)
        {
            // Find and unregister PlayerData for the disconnected player
            string playerId = conn.connectionId.ToString();
            PlayerData playerToRemove = clientManager.GetPlayers()
                .Find(player => player.PlayerId == playerId);

            if (playerToRemove != null)
            {
                clientManager.UnregisterPlayer(playerToRemove);
                Debug.Log($"PlayerData unregistered: {playerToRemove.PlayerName} (ID: {playerToRemove.PlayerId})");
            }
        }

        base.OnServerDisconnect(conn);
    }
}
