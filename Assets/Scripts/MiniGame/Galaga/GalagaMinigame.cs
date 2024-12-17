using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Unity.VisualScripting;

public class GalagaMinigame : MiniGameBase
{
    public Texture2D enemyTexture;
    public Texture2D bulletTexture;
    public RawImage plane;
    public Text countdownText;
    public Text scoreText;

    private List<RawImage> enemies = new List<RawImage>();


    private float planeSpeed = 300f;
    private float minX = -65f;
    private float maxX = 65f;

    private bool canMovePlane = false;
    private bool canShootBullet = false;
    private float shootBulletDelay = 1f;

    [Server]
    public override void StartGame()
    {
        base.StartGame();
        Debug.Log("[GalagaMinigame] Starting game.");
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
        canMovePlane = true;
        canShootBullet = true;
        StartEnemySpawns();
    }

    [Server]
    public override void EndGame()
    {
        Debug.Log("[GalagaMinigame] Ending game.");
        base.EndGame();
        CancelInvoke(nameof(SpawnEnemy));
        ClearEnemies();
    }

    [Server]
    public override void UpdateGameLogic()
    {
        base.UpdateGameLogic();
        
        // Log how many players we're processing input for this frame
        Debug.Log($"[GalagaMinigame] UpdateGameLogic: Processing input for {currentPlayers.Count} players.");

        foreach (var player in currentPlayers)
        {
            var input = player.InputData;
            
            // Log the input data received from each player
            Debug.Log($"[GalagaMinigame] Player {player.netId} Input: " +
                      $"Left={input.IsMovingLeft}, Right={input.IsMovingRight}, Interact={input.IsInteracting}");

            HandleMovement(player, input);
            HandleShooting(player, input);
        }

        UpdateEnemies();
    }

    [Server]
    public override void ResetGame()
    {
        Debug.Log("[GalagaMinigame] Resetting game.");
        base.ResetGame();
        base.score = 0;
        enemies.Clear();
    }

    [Server]
    private void StartEnemySpawns()
    {
        Debug.Log("[GalagaMinigame] Starting enemy spawns.");
        InvokeRepeating(nameof(SpawnEnemy), 0f, 2f);
    }

    [Server]
    private void SpawnEnemy()
    {
        Vector2 position = new Vector2(Random.Range(-60, 60), 120);
        Vector2 size = new Vector2(25, 25);
        Vector2 bcOffset = new Vector2(0, 0);
        Vector2 bcSize = new Vector2(25, 25);
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
                Debug.Log("[GalagaMinigame] Removing out-of-bounds enemy.");
                Destroy(enemy.gameObject);
                enemies.RemoveAt(i);
            }
        }
    }

    [Server]
    private void HandleMovement(CustomGamePlayer player, PlayerInputData input)
    {
        if (canMovePlane)
        {
            Debug.Log($"[GalagaMinigame] Handling movement for Player {player.netId}. " +
                    $"Left={input.IsMovingLeft}, Right={input.IsMovingRight}");

            Vector3 position = plane.rectTransform.localPosition;
            if (input.IsMovingLeft) position.x -= planeSpeed * Time.deltaTime;
            if (input.IsMovingRight) position.x += planeSpeed * Time.deltaTime;

            position.x = Mathf.Clamp(position.x, minX, maxX);
            plane.rectTransform.localPosition = position;

            Debug.Log($"[GalagaMinigame] Player {player.netId} moved plane to position {position}.");
        }
    }

    [Server]
    private void HandleShooting(CustomGamePlayer player, PlayerInputData input)
    {
        if (input.IsJumping && canShootBullet)
        {
            canShootBullet = false;
            Debug.Log($"[GalagaMinigame] Handling interaction for Player {player.netId}. Interacting={input.IsInteracting}");

            Texture2D texture = bulletTexture;
            Vector3 position = plane.rectTransform.localPosition;
            position.y += 15f;
            Vector2 size = new Vector2(7, 14);
            Vector2 bcOffset = new Vector2(0, 0);
            Vector2 bcSize = new Vector2(7, 14);
            bool bcIsTrigger = false;
            bool addRB = true;

            RawImage bullet = CreateRawImage(texture, position, size, bcOffset, bcSize, bcIsTrigger, addRB);
            bullet.AddComponent<GalagaBullet>();
            bullet.AddComponent<NetworkIdentity>();
            if (bullet != null)
            {
                Debug.Log($"[GalagaMinigame] Bullet fired by Player {player.netId} at position {position}.");
            }
            StartCoroutine(DelayAction());
        }
    }

    [Server]
    IEnumerator DelayAction()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(shootBulletDelay);

        // Code to execute after the delay
        Debug.Log("[GalagaMinigame] 1 second delay passed!");
        canShootBullet = true;
    }

    [Server]
    private RawImage CreateRawImage(Texture2D texture, Vector2 position, Vector2 size,
                                    Vector2 bcOffset, Vector2 bcSize, bool bcIsTrigger, bool addRB)
    {
        Canvas canvas = GetCanvas();
        if (canvas == null)
        {
            Debug.LogError("[GalagaMinigame] Cannot create RawImage: Canvas is null.");
            return null;
        }

        if (texture == null)
        {
            Debug.LogError("[GalagaMinigame] Cannot create RawImage: Texture is null.");
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
