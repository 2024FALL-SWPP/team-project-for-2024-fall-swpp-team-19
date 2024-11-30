using Mirror;
using UnityEngine;

public class CustomRoomPlayer : NetworkRoomPlayer
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log($"Room Player {index} connected.");
    }

    public override void OnClientEnterRoom()
    {
        base.OnClientEnterRoom();
        Debug.Log($"Room Player {index} entered room.");
    }

    public override void OnClientExitRoom()
    {
        base.OnClientExitRoom();
        Debug.Log($"Room Player {index} exited room.");
    }
}
