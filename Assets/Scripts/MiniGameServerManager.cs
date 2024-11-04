using Mirror;
using UnityEngine;

public class MiniGameServerManager : NetworkBehaviour
{
    [SyncVar]
    public bool isMiniGameActive;

    public void StartMiniGame()
    {
        if (isServer)
        {
            isMiniGameActive = true;
            RpcStartMiniGame();
        }
    }

    [ClientRpc]
    void RpcStartMiniGame()
    {
        Debug.Log("MiniGame started!");
    }
}