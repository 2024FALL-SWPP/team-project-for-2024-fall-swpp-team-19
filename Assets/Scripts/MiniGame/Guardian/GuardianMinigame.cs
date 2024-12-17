using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Unity.VisualScripting;

public class GuardianMinigame : MiniGameBase
{
    public Texture2D enemyTexture;
    public Texture2D cannonballTexture;
    public RawImage guardian;
    public Text countdownText;
    public Text scoreText;

    private List<RawImage> enemies = new List<RawImage>();

    private float guardianSpeed = 300f;
    private float minX = -65f;
    private float maxX = 65f;

    private bool canMoveGuardian = false;
    private bool canShootCannonball = false;
    private float shootCannonballDelay = 1f;

    [Server]
    public override void StartGame()
    {
        base.StartGame();
        Debug.Log("[GuardianMinigame] Starting game.");
        base.score = 0;
        base.targetScore = 3;
        StartCoroutine(CountdownAndStart());
    }

    [Server]
    private IEnumerator CountdownAndStart()
    {
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        countdownText.text = "";
        scoreText.text = "Score: " + base.score.ToString() + "/" + base.targetScore.ToString();
        canMoveGuardian = true;
        canShootCannonball = true;
        StartEnemySpawns();
    }

    [Server]
    public override void EndGame()
    {
        Debug.Log("[GuardianMinigame] Ending game.");
        base.EndGame();
        CancelInvoke(nameof(SpawnEnemy));
        ClearEnemies();
        canMoveGuardian = false;
        canShootCannonball = false;
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
            HandleShooting(player, input);
        }

        UpdateEnemies();
    }

    [Server]
    public override void ResetGame()
    {
        Debug.Log("[GuardianMinigame] Resetting game.");
        base.ResetGame();
        base.score = 0;
        enemies.Clear();
    }

    [Server]
    private void StartEnemySpawns()
    {
        Debug.Log("[GuardianMinigame] Starting enemy spawns.");
        InvokeRepeating(nameof(SpawnEnemy), 0f, 2f);
    }

    [Server]
    private void SpawnEnemy()
    {
        Vector2 position = new Vector2(Random.Range(-60, 60), 80);
        Vector2 size = new Vector2(40, 40);
        Vector2 bcOffset = new Vector2(8.5f, 1.5f);
        Vector2 bcSize = new Vector2(23, 35);
        bool bcIsTrigger = true;
        bool addRB = false;

        RawImage enemy = CreateRawImage(enemyTexture, position, size, bcOffset, bcSize, bcIsTrigger, addRB);
        enemy.tag = "Enemy";
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
        if (canMoveGuardian)
        {
            Debug.Log($"[GuardianMinigame] Handling movement for Player {player.netId}. " +
                    $"Left={input.IsMovingLeft}, Right={input.IsMovingRight}");

            Vector3 position = guardian.rectTransform.localPosition;
            if (input.IsMovingLeft) position.x -= guardianSpeed * Time.deltaTime;
            if (input.IsMovingRight) position.x += guardianSpeed * Time.deltaTime;

            position.x = Mathf.Clamp(position.x, minX, maxX);
            guardian.rectTransform.localPosition = position;

            Debug.Log($"[GuardianMinigame] Player {player.netId} moved guardian to position {position}.");
        }
    }

    [Server]
    private void HandleShooting(CustomGamePlayer player, PlayerInputData input)
    {
        if (input.IsJumping && canShootCannonball)
        {
            canShootCannonball = false;
            Debug.Log($"[GuardianMinigame] Handling interaction for Player {player.netId}. Interacting={input.IsInteracting}");

            Texture2D texture = cannonballTexture;
            Vector3 position = guardian.rectTransform.localPosition;
            position.y += 15f;
            Vector2 size = new Vector2(15, 15);
            Vector2 bcOffset = new Vector2(0, 0);
            Vector2 bcSize = new Vector2(1, 1);
            bool bcIsTrigger = false;
            bool addRB = true;

            RawImage cannonball = CreateRawImage(texture, position, size, bcOffset, bcSize, bcIsTrigger, addRB);
            cannonball.AddComponent<GuardianCannonBall>();
            cannonball.AddComponent<NetworkIdentity>();
            if (cannonball != null)
            {
                Debug.Log($"[GuardianMinigame] Cannonball fired by Player {player.netId} at position {position}.");
            }
            StartCoroutine(DelayAction());
        }
    }

    [Server]
    IEnumerator DelayAction()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(shootCannonballDelay);

        // Code to execute after the delay
        Debug.Log("1 second delay passed!");
        canShootCannonball = true;
    }

    [Server]
    private RawImage CreateRawImage(Texture2D texture, Vector2 position, Vector2 size,
                                    Vector2 bcOffset, Vector2 bcSize, bool bcIsTrigger, bool addRB)
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

        BoxCollider2D boxCollider = obj.AddComponent<BoxCollider2D>();
        boxCollider.offset = bcOffset;
        boxCollider.size = bcSize;
        boxCollider.isTrigger = bcIsTrigger;

        if (addRB)
        {
            Rigidbody2D rigidbody2D = obj.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0;
        }

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
    public void IncrementScore()
    {
        base.score++;
        scoreText.text = "Score: " + base.score.ToString() + "/" + base.targetScore.ToString();
    }
}
