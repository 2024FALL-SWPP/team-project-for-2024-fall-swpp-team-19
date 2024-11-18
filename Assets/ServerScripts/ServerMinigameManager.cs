using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class MinigameManager : NetworkBehaviour
{
    private readonly List<MinigameBase> minigames = new List<MinigameBase>();

    [SerializeField]
    private List<GameObject> minigamePrefabs;

    [Server]
    public void CreateMinigame(Vector3 position, Quaternion rotation, int minigameIndex)
    {
        if (minigameIndex < 0 || minigameIndex >= minigamePrefabs.Count)
        {
            Debug.LogError("Invalid minigame index.");
            return;
        }

        GameObject minigameObject = Instantiate(minigamePrefabs[minigameIndex], position, rotation);
        MinigameBase minigame = minigameObject.GetComponent<MinigameBase>();

        if (minigame != null)
        {
            NetworkServer.Spawn(minigameObject);
            minigame.StartMinigame();
            RegisterMinigame(minigame);
        }
        else
        {
            Debug.LogError("Minigame prefab does not have a MinigameBase component.");
            Destroy(minigameObject);
        }
    }

    [Server]
    public void RemoveMinigame(MinigameBase minigame)
    {
        if (minigames.Contains(minigame))
        {
            minigame.EndMinigame();
            UnregisterMinigame(minigame);
            NetworkServer.Destroy(minigame.gameObject);
        }
    }

    private void RegisterMinigame(MinigameBase minigame)
    {
        if (!minigames.Contains(minigame))
        {
            minigames.Add(minigame);
            Debug.Log("Minigame registered.");
        }
    }

    private void UnregisterMinigame(MinigameBase minigame)
    {
        if (minigames.Remove(minigame))
        {
            Debug.Log("Minigame unregistered.");
        }
    }


    public List<MinigameBase> GetMinigames()
    {
        return new List<MinigameBase>(minigames);
    }


    public MinigameBase GetMinigameAtPosition(Vector3 position, float radius)
    {
        foreach (MinigameBase minigame in minigames)
        {
            if (Vector3.Distance(minigame.Position, position) <= radius)
            {
                return minigame;
            }
        }
        return null;
    }
}