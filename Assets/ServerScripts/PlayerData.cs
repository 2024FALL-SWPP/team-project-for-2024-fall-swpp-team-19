using Mirror;

public class PlayerData
{
    public string PlayerId { get; private set; }
    public string PlayerName { get; private set; }
    public bool IsReady { get; set; }

    public PlayerData(string playerId, string playerName)
    {
        PlayerId = playerId;
        PlayerName = playerName;
        IsReady = false;
    }
}