using UnityEngine;
using Mirror;

public class RegisterableDevice : NetworkBehaviour
{
    public GameObject miniGamePrefab;
    private MiniGameBase activeMiniGame;

    public bool RegisterPlayer(CustomGamePlayer player)
    {
        if (player.isInMiniGame)
        {
            Debug.Log($"Player {player.netId} is already in a mini-game.");
            return false;
        }

        if (activeMiniGame == null)
        {
            activeMiniGame = Instantiate(miniGamePrefab).GetComponent<MiniGameBase>();
            NetworkServer.Spawn(activeMiniGame.gameObject);
        }

        bool registered = activeMiniGame.RegisterPlayer(player.connectionToClient, player);

        if (registered)
        {
            player.isInMiniGame = true;
            var miniGameIdentity = activeMiniGame.GetComponent<NetworkIdentity>();
            TargetShowMiniGameCanvas(player.connectionToClient, miniGameIdentity);
            Debug.Log($"Player {player.netId} successfully registered to the mini-game.");
            return true;
        }
        else
        {
            Debug.Log($"Mini-game registration failed for Player {player.netId}.");
            return false;
        }
    }

    [TargetRpc]
    private void TargetShowMiniGameCanvas(NetworkConnection conn, NetworkIdentity miniGameIdentity)
    {
        var miniGame = miniGameIdentity.GetComponent<MiniGameBase>();
        if (miniGame != null)
        {
            var canvas = miniGame.GetCanvas();
            if (canvas != null)
            {
                canvas.enabled = true;
            }
        }
    }
}
// }