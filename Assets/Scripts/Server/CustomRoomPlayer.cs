using Mirror;
using UnityEngine;

public class CustomRoomPlayer : NetworkRoomPlayer
{
    [SyncVar]
    private ColorEnum color = ColorEnum.Undefined;


    public ColorEnum GetColor(){
        return color;
    }

    [Command]
    public void SetColor(ColorEnum newColor){
        color = newColor;
    }


    
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
