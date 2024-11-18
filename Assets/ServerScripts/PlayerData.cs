using Mirror;

public class PlayerData : NetworkBehaviour
{
    [SyncVar]
    public string playerName;
    [SyncVar]
    public PlayerData targetPlayer;

    // Add more SyncVars as needed
}
