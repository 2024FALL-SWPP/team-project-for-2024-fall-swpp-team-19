using System.Collections.Generic;
using UnityEngine;

public interface IMinigame
{
    void StartMinigame();
    void EndMinigame();
    bool IsActive { get; }

    IReadOnlyList<PlayerData> Players { get; }
    int MaxPlayers { get; }

    void AddPlayer(PlayerData player);
    void RemovePlayer(PlayerData player);
}
