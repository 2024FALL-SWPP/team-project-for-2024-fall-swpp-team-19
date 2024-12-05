using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class MiniGameBase : NetworkBehaviour
{
    public List<CustomGamePlayer> currentPlayers = new List<CustomGamePlayer>();
    private Canvas miniGameCanvas;

    [SerializeField]
    private int requiredPlayersToStart = 1; // Number of players required to start the game, configurable in the Inspector.

    private bool gameStarted = false;

    private void Awake()
    {
        InitializeCanvas();
    }

    private void InitializeCanvas()
    {
        miniGameCanvas = GetComponentInChildren<Canvas>();
        if (miniGameCanvas != null)
        {
            miniGameCanvas.enabled = false;
        }
        else
        {
            Debug.LogError($"MiniGameBase: No Canvas found in children of {gameObject.name}");
        }
    }

    public Canvas GetCanvas()
    {
        return miniGameCanvas;
    }

    [Server]
    public virtual bool RegisterPlayer(NetworkConnectionToClient conn, CustomGamePlayer player)
    {
        if (currentPlayers.Contains(player))
        {
            Debug.Log($"[MiniGameBase] Player {player.netId} is already registered.");
            return false;
        }

        currentPlayers.Add(player);
        OnPlayerRegistered(conn, player);
        Debug.Log($"[MiniGameBase] Player {player.netId} successfully registered.");

        // Check if the required number of players has been met to start the game.
        if (!gameStarted && currentPlayers.Count >= requiredPlayersToStart)
        {
            StartGame();
        }

        return true;
    }

    [Server]
    public virtual bool UnregisterPlayer(NetworkConnectionToClient conn, CustomGamePlayer player)
    {
        if (!currentPlayers.Contains(player))
        {
            Debug.Log($"[MiniGameBase] Player {player.netId} is not registered.");
            return false;
        }

        currentPlayers.Remove(player);
        OnPlayerUnregistered(conn, player);
        Debug.Log($"[MiniGameBase] Player {player.netId} successfully unregistered.");

        // Stop the game if players drop below the required number.
        if (gameStarted && currentPlayers.Count < requiredPlayersToStart)
        {
            EndGame();
        }

        return true;
    }

    [Server]
    public virtual void StartGame()
    {
        gameStarted = true;

        Debug.Log("[MiniGameBase] Game started!");
        if (miniGameCanvas != null)
        {
            RpcShowCanvasToPlayers();
        }
    }

    [Server]
    public virtual void EndGame()
    {
        gameStarted = false;

        Debug.Log("[MiniGameBase] Game ended.");
        if (miniGameCanvas != null)
        {
            RpcHideCanvasFromPlayers();
        }
        currentPlayers.Clear();
    }

    [Server]
    public virtual void ResetGame()
    {
        gameStarted = false;
        currentPlayers.Clear();
    }

    [ServerCallback]
    public virtual void UpdateGameLogic()
    {
        foreach (var player in currentPlayers)
        {
            HandlePlayerInput(player);
        }
    }

    [Server]
    protected virtual void HandlePlayerInput(CustomGamePlayer player)
    {
    }

    [TargetRpc]
    private void TargetShowCanvas(NetworkConnection conn)
    {
        if (miniGameCanvas != null)
        {
            miniGameCanvas.enabled = true;
        }
    }

    [TargetRpc]
    private void TargetHideCanvas(NetworkConnection conn)
    {
        if (miniGameCanvas != null)
        {
            miniGameCanvas.enabled = false;
        }
    }

    [ClientRpc]
    private void RpcShowCanvasToPlayers()
    {
        if (miniGameCanvas != null)
        {
            miniGameCanvas.enabled = true;
        }
    }

    [ClientRpc]
    private void RpcHideCanvasFromPlayers()
    {
        if (miniGameCanvas != null)
        {
            miniGameCanvas.enabled = false;
        }
    }

    protected virtual void OnPlayerRegistered(NetworkConnectionToClient conn, CustomGamePlayer player)
    {
    }

    protected virtual void OnPlayerUnregistered(NetworkConnectionToClient conn, CustomGamePlayer player)
    {
    }
}
