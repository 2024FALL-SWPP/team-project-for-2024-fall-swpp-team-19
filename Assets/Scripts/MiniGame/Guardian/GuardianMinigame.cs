using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GuardianMinigame : MiniGameBase
{
    public GameObject guardianPrefab1;
    public GameObject guardianPrefab2;
    public GameObject enemyPrefab1;
    public GameObject enemyPrefab2;
    public GameObject cannonballPrefab1;
    public GameObject cannonballPrefab2;

    public Transform guardianSpawn1;
    public Transform guardianSpawn2;

    private List<CustomGamePlayer> guardians = new List<CustomGamePlayer>();
    private List<GameObject> enemies = new List<GameObject>();

    private int score1 = 0;
    private int score2 = 0;

    private float guardianSpeed = 300f;
    private float minX = -65f;
    private float maxX = 65f;

    public override void StartGame()
    {
        base.StartGame();
        SpawnGuardians();
        StartEnemySpawns();
    }

    public override void EndGame()
    {
        base.EndGame();
        CancelInvoke(nameof(SpawnEnemy1));
        CancelInvoke(nameof(SpawnEnemy2));
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
    }

    public override void UpdateGameLogic()
    {
        base.UpdateGameLogic();
        foreach (var player in currentPlayers)
        {
            var input = player.InputData;

            if (input != null)
            {
                HandleMovement(player, input);
                HandleInteraction(player, input);
            }
        }
    }

    public override void ResetGame()
    {
        base.ResetGame();
        score1 = 0;
        score2 = 0;
        enemies.Clear();
    }

    private void SpawnGuardians()
    {
        if (currentPlayers.Count >= 2)
        {
            var guardian1 = Instantiate(guardianPrefab1, guardianSpawn1.position, Quaternion.identity);
            var guardian2 = Instantiate(guardianPrefab2, guardianSpawn2.position, Quaternion.identity);

            NetworkServer.Spawn(guardian1);
            NetworkServer.Spawn(guardian2);

            guardians.Add(currentPlayers[0]);
            guardians.Add(currentPlayers[1]);
        }
    }

    private void StartEnemySpawns()
    {
        InvokeRepeating(nameof(SpawnEnemy1), 0f, 2f);
        InvokeRepeating(nameof(SpawnEnemy2), 0f, 2f);
    }

    private void SpawnEnemy1()
    {
        var enemy = Instantiate(enemyPrefab1, new Vector3(Random.Range(-60, 60), 80, 0), Quaternion.identity);
        NetworkServer.Spawn(enemy);
        enemies.Add(enemy);
    }

    private void SpawnEnemy2()
    {
        var enemy = Instantiate(enemyPrefab2, new Vector3(Random.Range(-60, 60), -80, 0), Quaternion.identity);
        NetworkServer.Spawn(enemy);
        enemies.Add(enemy);
    }

    private void HandleMovement(CustomGamePlayer player, PlayerInputData input)
    {
        var moveDirection = Vector3.zero;

        if (input.IsMovingLeft) moveDirection += Vector3.left;
        if (input.IsMovingRight) moveDirection += Vector3.right;

        player.transform.localPosition += moveDirection * guardianSpeed * Time.deltaTime;

        float clampedX = Mathf.Clamp(player.transform.localPosition.x, minX, maxX);
        player.transform.localPosition = new Vector3(clampedX, player.transform.localPosition.y, player.transform.localPosition.z);
    }

    private void HandleInteraction(CustomGamePlayer player, PlayerInputData input)
    {
        if (input.IsInteracting)
        {
            if (player == guardians[0])
            {
                ShootCannonBall(player, cannonballPrefab1, 15f);
            }
            else if (player == guardians[1])
            {
                ShootCannonBall(player, cannonballPrefab2, -15f);
            }
        }
    }

    public void ShootCannonBall(CustomGamePlayer player, GameObject cannonballPrefab, float offsetY)
    {
        var cannonball = Instantiate(cannonballPrefab, player.transform);
        var localPos = player.transform.localPosition;
        localPos.y += offsetY;
        cannonball.transform.localPosition = localPos;
        NetworkServer.Spawn(cannonball);
    }

    public void IncrementScore(CustomGamePlayer player)
    {
        if (player == guardians[0])
        {
            score1++;
        }
        else if (player == guardians[1])
        {
            score2++;
        }
    }
}
