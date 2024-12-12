using Mirror;
using UnityEngine;

public class RegisterableDevice : NetworkBehaviour
{
    public GameObject miniGamePrefab; // Mini-game prefab reference
    private MiniGameBase activeMiniGame;

    [Server]
    public bool RegisterPlayer(CustomGamePlayer player)
    {
        if (player.isInMiniGame)
        {
            Debug.Log($"[RegisterableDevice] Player {player.netId} is already in a mini-game.");
            return false;
        }

        if (activeMiniGame == null)
        {
            // Spawn the mini-game instance if it doesn't exist
            if (miniGamePrefab == null)
            {
                Debug.LogError("[RegisterableDevice] Mini-game prefab is not assigned!");
                return false;
            }

            activeMiniGame = Instantiate(miniGamePrefab).GetComponent<MiniGameBase>();
            if (activeMiniGame != null)
            {
                NetworkServer.Spawn(activeMiniGame.gameObject);
                Debug.Log("[RegisterableDevice] Mini-game instance created and spawned.");
            }
            else
            {
                Debug.LogError("[RegisterableDevice] Failed to get MiniGameBase from prefab.");
                return false;
            }
        }

        // Register the player in the mini-game
        return activeMiniGame.RegisterPlayer(player);
    }

    [Server]
    public void UnregisterPlayer(CustomGamePlayer player)
    {
        if (activeMiniGame == null) return;

        activeMiniGame.UnregisterPlayer(player);
    }
}
