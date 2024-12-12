using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class TargetManager : NetworkBehaviour
{
    private List<GameObject> players = new List<GameObject>();

    public void RegisterPlayer(GameObject player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }

    public void AssignTargets()
    {
        for (int i = 0; i < players.Count; i++)
        {
            int targetIndex = (i + 1) % players.Count;
            players[i].GetComponent<PlayerCombat>().SetTarget(players[targetIndex]);
        }
    }

    public void CheckRemainingPlayers()
    {
        int aliveCount = 0;

        foreach (var player in players)
        {
            if (player != null && !player.GetComponent<PlayerHealth>().IsDead())
            {
                aliveCount++;
            }
        }

        if (aliveCount == 1)
        {
            RpcMoveToGameOverScene();
        }
    }

    [ClientRpc]
    void RpcMoveToGameOverScene()
    {
        // Load GameOver scene for all players
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");
    }
}