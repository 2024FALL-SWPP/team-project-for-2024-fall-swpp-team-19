using Mirror;
using UnityEngine;
using System.Collections.Generic;

public abstract class MinigameBase : NetworkBehaviour, IMinigame
{

    [SyncVar]
    private int currentPlayerCount = 0;
    public int CurrentPlayerCount => currentPlayerCount;
}