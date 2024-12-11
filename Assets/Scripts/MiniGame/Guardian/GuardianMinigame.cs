using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GuardianMinigame : MiniGameBase
{
    public Texture2D guardianTexture1;
    public Texture2D guardianTexture2;
    public Texture2D enemyTexture1;
    public Texture2D enemyTexture2;
    public Texture2D cannonballTexture1;
    public Texture2D cannonballTexture2;

    private List<RawImage> guardians = new List<RawImage>();
    private List<RawImage> enemies = new List<RawImage>();

    private int score1 = 0;
    private int score2 = 0;

    private float guardianSpeed = 300f;
    private float minX = -65f;
    private float maxX = 65f;

    [Server]
    public override void StartGame()
    {
        base.StartGame();
        Debug.Log("[GuardianMinigame] Starting game.");
        SpawnGuardians();
        StartEnemySpawns();
    }

    [Server]
    public override void EndGame()
    {
        Debug.Log("[GuardianMinigame] Ending game.");
        base.EndGame();
        CancelInvoke(nameof(SpawnEnemy1));
        CancelInvoke(nameof(SpawnEnemy2));
        ClearEnemies();
        ClearGuardians();
    }

    [Server]
    public override void UpdateGameLogic()
    {
        base.UpdateGameLogic();
        
        // Log how many players we're processing input for this frame
        Debug.Log($"[GuardianMinigame] UpdateGameLogic: Processing input for {currentPlayers.Count} players.");

        foreach (var player in currentPlayers)
        {
            var input = player.InputData;
            
            // Log the input data received from each player
            Debug.Log($"[GuardianMinigame] Player {player.netId} Input: " +
                      $"Left={input.IsMovingLeft}, Right={input.IsMovingRight}, Interact={input.IsInteracting}");

            HandleMovement(player, input);
            HandleInteraction(player, input);
        }

        UpdateEnemies();
    }

    [Server]
    public override void ResetGame()
    {
        Debug.Log("[GuardianMinigame] Resetting game.");
        base.ResetGame();
        score1 = 0;
        score2 = 0;
        enemies.Clear();
        guardians.Clear();
    }

    [Server]
    private void SpawnGuardians()
    {
        Debug.Log($"[GuardianMinigame] Spawning guardians for {currentPlayers.Count} players.");
        ClearGuardians();

        for (int i = 0; i < currentPlayers.Count && i < 2; i++)
        {
            Texture2D texture = i == 0 ? guardianTexture1 : guardianTexture2;
            Vector2 position = i == 0 ? new Vector2(-100, 0) : new Vector2(100, 0);
            Vector2 size = new Vector2(100, 100);

            RawImage guardian = CreateRawImage(texture, position, size);
            if (guardian != null)
            {
                guardians.Add(guardian);
            }
        }
    }

    [Server]
    private void StartEnemySpawns()
    {
        Debug.Log("[GuardianMinigame] Starting enemy spawns.");
        InvokeRepeating(nameof(SpawnEnemy1), 0f, 2f);
        InvokeRepeating(nameof(SpawnEnemy2), 1f, 3f);
    }

    [Server]
    private void SpawnEnemy1()
    {
        Vector2 position = new Vector2(Random.Range(-60, 60), 80);
        Vector2 size = new Vector2(50, 50);
        RawImage enemy = CreateRawImage(enemyTexture1, position, size);
        if (enemy != null) enemies.Add(enemy);
    }

    [Server]
    private void SpawnEnemy2()
    {
        Vector2 position = new Vector2(Random.Range(-60, 60), -80);
        Vector2 size = new Vector2(50, 50);
        RawImage enemy = CreateRawImage(enemyTexture2, position, size);
        if (enemy != null) enemies.Add(enemy);
    }

    [Server]
    private void UpdateEnemies()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            RawImage enemy = enemies[i];
            if (enemy == null) continue;

            Vector3 position = enemy.rectTransform.localPosition;
            position.y -= 50f * Time.deltaTime;
            enemy.rectTransform.localPosition = position;

            if (position.y < -100f)
            {
                Debug.Log("[GuardianMinigame] Removing out-of-bounds enemy.");
                Destroy(enemy.gameObject);
                enemies.RemoveAt(i);
            }
        }
    }

    [Server]
    private void HandleMovement(CustomGamePlayer player, PlayerInputData input)
    {
        int index = currentPlayers.IndexOf(player);
        if (index < 0 || index >= guardians.Count)
        {
            Debug.LogWarning($"[GuardianMinigame] Cannot handle movement for Player {player.netId}, no guardian found.");
            return;
        }

        Debug.Log($"[GuardianMinigame] Handling movement for Player {player.netId}. " +
                  $"Left={input.IsMovingLeft}, Right={input.IsMovingRight}");

        RawImage guardian = guardians[index];
        if (guardian == null) return;

        Vector3 position = guardian.rectTransform.localPosition;
        if (input.IsMovingLeft) position.x -= guardianSpeed * Time.deltaTime;
        if (input.IsMovingRight) position.x += guardianSpeed * Time.deltaTime;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        guardian.rectTransform.localPosition = position;

        Debug.Log($"[GuardianMinigame] Player {player.netId} moved guardian to position {position}.");
    }

    [Server]
    private void HandleInteraction(CustomGamePlayer player, PlayerInputData input)
    {
        if (input.IsInteracting)
        {
            int index = currentPlayers.IndexOf(player);
            if (index < 0 || index >= guardians.Count)
            {
                Debug.LogWarning($"[GuardianMinigame] Cannot handle interaction for Player {player.netId}, no guardian found.");
                return;
            }

            Debug.Log($"[GuardianMinigame] Handling interaction for Player {player.netId}. Interacting={input.IsInteracting}");

            Texture2D cannonballTexture = index == 0 ? cannonballTexture1 : cannonballTexture2;
            Vector3 position = guardians[index].rectTransform.localPosition;
            position.y += 15f;
            Vector2 size = new Vector2(20, 20);

            RawImage cannonball = CreateRawImage(cannonballTexture, position, size);
            if (cannonball != null)
            {
                // Cannonball will be destroyed after 3 seconds
                Destroy(cannonball.gameObject, 3f);
                Debug.Log($"[GuardianMinigame] Cannonball fired by Player {player.netId} at position {position}.");
            }
        }
    }

    [Server]
    private RawImage CreateRawImage(Texture2D texture, Vector2 position, Vector2 size)
    {
        Canvas canvas = GetCanvas();
        if (canvas == null)
        {
            Debug.LogError("[GuardianMinigame] Cannot create RawImage: Canvas is null.");
            return null;
        }

        if (texture == null)
        {
            Debug.LogError("[GuardianMinigame] Cannot create RawImage: Texture is null.");
            return null;
        }

        GameObject obj = new GameObject("UIElement");
        obj.transform.SetParent(canvas.transform, false);

        RawImage rawImage = obj.AddComponent<RawImage>();
        rawImage.texture = texture;
        rawImage.rectTransform.sizeDelta = size;
        rawImage.rectTransform.localPosition = position;

        return rawImage;
    }

    [Server]
    private void ClearEnemies()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null) Destroy(enemy.gameObject);
        }
        enemies.Clear();
    }

    [Server]
    private void ClearGuardians()
    {
        foreach (var guardian in guardians)
        {
            if (guardian != null) Destroy(guardian.gameObject);
        }
        guardians.Clear();
    }

    [Server]
    public void IncrementScore(CustomGamePlayer player)
    {
        if (currentPlayers.Count > 0 && player == currentPlayers[0])
        {
            score1++;
            Debug.Log($"[GuardianMinigame] Player {player.netId} scored! Score1: {score1}");
        }
        else if (currentPlayers.Count > 1 && player == currentPlayers[1])
        {
            score2++;
            Debug.Log($"[GuardianMinigame] Player {player.netId} scored! Score2: {score2}");
        }
    }
}
