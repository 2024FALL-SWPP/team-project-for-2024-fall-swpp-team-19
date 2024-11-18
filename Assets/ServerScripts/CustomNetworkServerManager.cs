using Mirror;
using UnityEngine;

public class CustomNetworkServerManager : NetworkManager
{
    private GameManager gameManager;

    public override void OnStartServer()
    {
        base.OnStartServer();
        gameManager = FindObjectOfType<GameManager>();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(playerPrefab, GetStartPosition().position, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);
        PlayerData playerData = player.GetComponent<PlayerData>();
        if (playerData != null && gameManager != null)
        {
            gameManager.RegisterPlayer(playerData);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        GameObject player = conn.identity != null ? conn.identity.gameObject : null;
        if (player != null)
        {
            PlayerData playerData = player.GetComponent<PlayerData>();
            if (playerData != null && gameManager != null)
            {
                gameManager.UnregisterPlayer(playerData);
            }
        }
        base.OnServerDisconnect(conn);
    }
}
