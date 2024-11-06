using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
public class CustomNetworkServerManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
    }
}