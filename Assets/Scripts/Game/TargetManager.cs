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
        List<GameObject> availableTargets = new List<GameObject>(players);

        foreach (var player in players)
        {
            if (availableTargets.Count == 1)
            {
                // If only one target remains, assign it to the last player
                player.GetComponent<PlayerCombat>().SetTarget(availableTargets[0]);
                break;
            }

            // Remove the current player from available targets to avoid self-targeting
            availableTargets.Remove(player);

            // Pick a random target from the remaining players
            int randomIndex = Random.Range(0, availableTargets.Count);
            GameObject target = availableTargets[randomIndex];

            // Assign the target and remove it from the list
            player.GetComponent<PlayerCombat>().SetTarget(target);
            availableTargets.Remove(target);
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
