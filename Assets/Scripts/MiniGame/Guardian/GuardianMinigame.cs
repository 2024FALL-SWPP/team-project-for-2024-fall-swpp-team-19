using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public override void StartGame()
    {
        Debug.Log("[GuardianMinigame] Starting game.");
        base.StartGame();
        SpawnGuardians();
        StartEnemySpawns();
    }

    public override void EndGame()
    {
        Debug.Log("[GuardianMinigame] Ending game.");
        base.EndGame();
        CancelInvoke(nameof(SpawnEnemy1));
        CancelInvoke(nameof(SpawnEnemy2));
        ClearEnemies();
        ClearGuardians();
    }

    public override void UpdateGameLogic()
    {
        base.UpdateGameLogic();
        foreach (var player in currentPlayers)
        {
            var input = player.InputData;

            if (input != null)
            {
                Debug.Log($"[GuardianMinigame] Processing input for player {player.netId}.");
                HandleMovement(player, input);
                HandleInteraction(player, input);
            }
        }

        UpdateEnemies();
    }

    public override void ResetGame()
    {
        Debug.Log("[GuardianMinigame] Resetting game.");
        base.ResetGame();
        score1 = 0;
        score2 = 0;
        enemies.Clear();
        guardians.Clear();
    }

    private void SpawnGuardians()
    {
        Debug.Log($"[GuardianMinigame] Spawning guardians for {currentPlayers.Count} players.");

        for (int i = 0; i < currentPlayers.Count && i < 2; i++)
        {
            Texture2D texture = i == 0 ? guardianTexture1 : guardianTexture2;
            Vector2 position = i == 0 ? new Vector2(-100, 0) : new Vector2(100, 0);
            Vector2 size = new Vector2(100, 100); // Example size for guardians

            RawImage guardian = CreateRawImage(texture, position, size);
            guardians.Add(guardian);
        }
    }

    private void StartEnemySpawns()
    {
        Debug.Log("[GuardianMinigame] Starting enemy spawns.");
        InvokeRepeating(nameof(SpawnEnemy1), 0f, 2f);
        InvokeRepeating(nameof(SpawnEnemy2), 1f, 3f);
    }

    private void SpawnEnemy1()
    {
        Vector2 position = new Vector2(Random.Range(-60, 60), 80);
        Vector2 size = new Vector2(50, 50); // Example size for enemies
        RawImage enemy = CreateRawImage(enemyTexture1, position, size);
        enemies.Add(enemy);
    }

    private void SpawnEnemy2()
    {
        Vector2 position = new Vector2(Random.Range(-60, 60), -80);
        Vector2 size = new Vector2(50, 50); // Example size for enemies
        RawImage enemy = CreateRawImage(enemyTexture2, position, size);
        enemies.Add(enemy);
    }

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
                Debug.Log($"[GuardianMinigame] Removing out-of-bounds enemy.");
                Destroy(enemy.gameObject);
                enemies.RemoveAt(i);
            }
        }
    }

    private void HandleMovement(CustomGamePlayer player, PlayerInputData input)
    {
        Debug.Log($"[GuardianMinigame] Handling movement for player {player.netId}.");

        RawImage guardian = guardians[currentPlayers.IndexOf(player)];
        if (guardian == null) return;

        Vector3 position = guardian.rectTransform.localPosition;
        if (input.IsMovingLeft) position.x -= guardianSpeed * Time.deltaTime;
        if (input.IsMovingRight) position.x += guardianSpeed * Time.deltaTime;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        guardian.rectTransform.localPosition = position;

        Debug.Log($"[GuardianMinigame] Player {player.netId} moved to position {position}.");
    }

    private void HandleInteraction(CustomGamePlayer player, PlayerInputData input)
    {
        if (input.IsInteracting)
        {
            Debug.Log($"[GuardianMinigame] Handling interaction for player {player.netId}.");

            Texture2D cannonballTexture = currentPlayers.IndexOf(player) == 0 ? cannonballTexture1 : cannonballTexture2;
            Vector3 position = guardians[currentPlayers.IndexOf(player)].rectTransform.localPosition;
            position.y += 15f;
            Vector2 size = new Vector2(20, 20); // Example size for cannonballs

            RawImage cannonball = CreateRawImage(cannonballTexture, position, size);
            Destroy(cannonball.gameObject, 3f); // Destroy cannonball after 3 seconds
        }
    }

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

        // Explicitly set the size based on the provided dimensions
        rawImage.rectTransform.sizeDelta = size;

        // Position the element within the canvas
        rawImage.rectTransform.localPosition = position;

        Debug.Log($"[GuardianMinigame] Created UI element at position {position} with size {size} and texture {texture.name}.");
        return rawImage;
    }

    public void IncrementScore(CustomGamePlayer player)
    {
        if (player == currentPlayers[0])
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

    private void ClearEnemies()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null) Destroy(enemy.gameObject);
        }
        enemies.Clear();
    }

    private void ClearGuardians()
    {
        foreach (var guardian in guardians)
        {
            if (guardian != null) Destroy(guardian.gameObject);
        }
        guardians.Clear();
    }
}
