using UnityEngine;
using Mirror;

public class CustomRoomManager : NetworkRoomManager
{
    private string roomCode;

    public override void OnStartHost()
    {
        base.OnStartHost();
        roomCode = GenerateRoomCode();
        Debug.Log($"Room Code (External IP): {roomCode}");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server has started.");
    }

    public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnRoomServerAddPlayer(conn);
        Debug.Log($"Player added to room: Connection ID {conn.connectionId}");
    }

    public override void OnRoomServerPlayersReady()
    {
        base.OnRoomServerPlayersReady();
        Debug.Log("All players are ready. Transitioning to the game...");
    }

    public override void OnRoomServerSceneChanged(string sceneName)
    {
        base.OnRoomServerSceneChanged(sceneName);
        Debug.Log($"Scene changed to: {sceneName}");
    }

    public string GetRoomCode()
    {
        return roomCode;
    }

    public void SetRoomCode(string code)
    {
        roomCode = code;
        Debug.Log($"Room Code manually set to: {roomCode}");
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
        return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(rawData));
    }

    private int GetServerPort()
    {
        var transport = GetComponent<Mirror.TelepathyTransport>();
        return transport != null ? transport.port : 7777;
    }
}
