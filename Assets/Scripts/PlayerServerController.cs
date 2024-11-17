using Mirror;
using UnityEngine;

public class PlayerServerController : NetworkBehaviour
{
    [SyncVar]
    public string playerName;

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            CmdInteract("Interact");
        }
    }

    [Command]
    void CmdInteract(string action)
    {
        Debug.Log($"{playerName} performed interaction: {action}");
        RpcBroadcastInteraction(action);
    }

    [ClientRpc]
    void RpcBroadcastInteraction(string action)
    {
        if (!isLocalPlayer)
            Debug.Log($"Player {playerName} performed: {action}");
    }
}