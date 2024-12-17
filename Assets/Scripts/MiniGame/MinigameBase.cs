using System.Collections.Generic;
using Mirror;
using UnityEngine;

public abstract class MiniGameBase : NetworkBehaviour
{
    [SyncVar] public bool gameStarted = false;

    // Server-side list of players currently in the mini-game
    public List<CustomGamePlayer> currentPlayers = new List<CustomGamePlayer>();

    // SyncList to track player netIds for all clients
    public SyncList<uint> playerIdsInGame = new SyncList<uint>();

    public int score;
    public int targetScore;

    private Canvas miniGameCanvas;

    [SerializeField] private int requiredPlayersToStart = 1;
    public override void OnStartClient()
    {
        base.OnStartClient();
        InitializeCanvas();
        ShowCanvasIfLocalPlayerIsInGame();

        // Subscribe using the correct delegate signature
        playerIdsInGame.OnChange += OnPlayerIdsChanged;
    }

    private void OnPlayerIdsChanged(SyncList<uint>.Operation op, int index, uint item)
    {
        ShowCanvasIfLocalPlayerIsInGame();
    }
    private void InitializeCanvas()
    {
        miniGameCanvas = GetComponentInChildren<Canvas>();
        if (miniGameCanvas != null)
        {
            miniGameCanvas.enabled = false; // Disable by default
            Debug.Log($"[MiniGameBase] Canvas initialized on client for {gameObject.name}.");
        }
        else
        {
            Debug.LogError($"[MiniGameBase] No Canvas found in children of {gameObject.name}.");
        }
    }

    private void ShowCanvasIfLocalPlayerIsInGame()
    {
        if (miniGameCanvas == null) return;

        var localPlayer = NetworkClient.localPlayer;
        if (localPlayer == null) return;

        if (playerIdsInGame.Contains(localPlayer.netId))
        {
            miniGameCanvas.enabled = true;
            Debug.Log($"[MiniGameBase] Canvas enabled for local player {localPlayer.netId}.");
        }
        else
        {
            miniGameCanvas.enabled = false;
            NetworkServer.Destroy(gameObject);
            Debug.Log($"[MiniGameBase] Canvas hidden for local player {localPlayer.netId}.");
        }
    }

    [Server]
    public virtual bool RegisterPlayer(CustomGamePlayer player)
    {
        if (currentPlayers.Contains(player))
        {
            Debug.Log($"[MiniGameBase] Player {player.netId} is already registered.");
            return false;
        }

        currentPlayers.Add(player);
        playerIdsInGame.Add(player.netId);
        player.isInMiniGame = true;
        OnPlayerRegistered(player);

        Debug.Log($"[MiniGameBase] Player {player.netId} successfully registered.");
        if (!gameStarted && currentPlayers.Count >= requiredPlayersToStart)
        {
            StartGame();
        }

        return true;
    }

    [Server]
    public virtual void UnregisterPlayer(CustomGamePlayer player)
    {
        if (!currentPlayers.Contains(player)) return;

        currentPlayers.Remove(player);
        playerIdsInGame.Remove(player.netId);
        player.isInMiniGame = false;
        OnPlayerUnregistered(player);

        Debug.Log($"[MiniGameBase] Player {player.netId} successfully unregistered.");

        if (gameStarted && currentPlayers.Count < requiredPlayersToStart)
        {
            EndGame();
        }
    }

    [Server]
    public virtual void StartGame()
    {
        gameStarted = true;
        Debug.Log("[MiniGameBase] Game started!");
        RpcUpdateGameState();
    }

    [Server]
    public virtual void EndGame()
    {
        gameStarted = false;
        Debug.Log("[MiniGameBase] Game ended!");

        // Clear server-side tracking
        currentPlayers.Clear();
        playerIdsInGame.Clear();

        RpcUpdateGameState();
    }

    [Server]
    public virtual void ResetGame()
    {
        foreach (var player in currentPlayers)
        {
            UnregisterPlayer(player);
            NetworkServer.Destroy(player.interactingDevice);
            player.interactingDevice = null;
        }
        gameStarted = false;
        currentPlayers.Clear();
        playerIdsInGame.Clear();
    }

    [Server]
    public virtual void ClearGame()
    {
        foreach (var player in currentPlayers)
        {
            UnregisterPlayer(player);
            NetworkServer.Destroy(player.interactingDevice);
            player.interactingDevice = null;
        }
    }

    [ServerCallback]
    private void Update()
    {
        // Ensure game logic updates on the server each frame
        UpdateGameLogic();
    }

    [Server]
    public virtual void UpdateGameLogic()
    {
        // Implement logic in subclasses if needed
        if (score >= targetScore)
        {
            ClearGame();
        }
    }

    [Server]
    protected virtual void HandlePlayerInput(CustomGamePlayer player)
    {
        // Override in subclasses to handle input
    }

    [ClientRpc]
    private void RpcUpdateGameState()
    {
        // Re-check if the local player is in the game to update UI
        ShowCanvasIfLocalPlayerIsInGame();
    }

    protected virtual void OnPlayerRegistered(CustomGamePlayer player)
    {
        Debug.Log($"[MiniGameBase] Player {player.netId} registered in mini-game {gameObject.name}.");
    }

    protected virtual void OnPlayerUnregistered(CustomGamePlayer player)
    {
        Debug.Log($"[MiniGameBase] Player {player.netId} unregistered from mini-game {gameObject.name}.");
    }

    public Canvas GetCanvas()
    {
        return miniGameCanvas;
    }
}
