using Mirror;
using UnityEngine;

public class CustomRoomManager : NetworkRoomManager
{
    private string roomCode;
    public bool playerCountDirty = true;
    public bool roomCodeDirty = true;

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log($"Client connected: Connection ID {conn.connectionId}");
        MarkPlayerCountDirty();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log($"Client disconnected: Connection ID {conn.connectionId}");
        MarkPlayerCountDirty();
    }

    private void MarkPlayerCountDirty()
    {
        playerCountDirty = true;
    }

    private void MarkRoomCodeDirty()
    {
        roomCodeDirty = true;
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        roomCode = GenerateRoomCode();
        Debug.Log($"Room Code (External IP): {roomCode}");
        MarkRoomCodeDirty();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server has started.");
        NetworkServer.RegisterHandler<RoomCheckRequest>(OnRoomCheckRequest);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("Server has stopped.");
        NetworkServer.UnregisterHandler<RoomCheckRequest>();
    }

    public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnRoomServerAddPlayer(conn);
        Debug.Log($"Player added to room: Connection ID {conn.connectionId}");
        MarkPlayerCountDirty();
    }

    public override void OnRoomServerPlayersReady()
    {
        base.OnRoomServerPlayersReady();
        Debug.Log("All players are ready. Starting the game...");
        ServerChangeScene(GameplayScene);
    }

    public override void OnRoomServerSceneChanged(string sceneName)
    {
        base.OnRoomServerSceneChanged(sceneName);
        Debug.Log($"OnRoomServerSceneChanged called with scene: {sceneName}");

        if (sceneName == RoomScene)
        {
            Debug.Log("Server has loaded the Room Scene. Marking player count as dirty.");
            MarkPlayerCountDirty();
        }
    }

    public string GetRoomCode()
    {
        return roomCode;
    }

    public void SetRoomCode(string code)
    {
        roomCode = code;
        MarkRoomCodeDirty();
    }

    private string GenerateRoomCode()
    {
        string externalIP = ExternalIPHelper.GetExternalIPAddress();
        int port = GetServerPort();

        if (externalIP == "Unknown")
        {
            Debug.LogError("Unable to generate room code: External IP not found.");
            return "Error: No IP";
        }

        string rawData = $"{externalIP}:{port}";
        string encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(rawData));
        return encoded;
    }

    private int GetServerPort()
    {
        var transport = GetComponent<Mirror.TelepathyTransport>();
        if (transport != null)
        {
            return transport.port;
        }
        Debug.LogWarning("Transport component not found. Using default port 7777.");
        return 7777;
    }

    private void OnRoomCheckRequest(NetworkConnectionToClient conn, RoomCheckRequest msg)
    {
        Debug.Log($"Received room check request for room code: {msg.RoomCode}");
        bool isRoomValid = msg.RoomCode == roomCode;
        conn.Send(new RoomCheckResponse { IsRoomValid = isRoomValid });
    }

    public int GetPlayerCount()
    {
        return roomSlots.Count;
    }
}
