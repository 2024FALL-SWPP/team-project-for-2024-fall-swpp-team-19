using System.Collections.Generic;
using UnityEngine;

public class MgEnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // 적 전투기 프리팹
    public RectTransform minigamePanel; // MinigamePanel의 RectTransform
    public float spawnInterval = 1f; // 적 생성 간격
    public float spawnXRange = 87f; // 적이 생성될 X축 범위
    public float spawnYPosition = 100f; // 적이 생성될 Y축 위치

    private List<GameObject> enemies = new List<GameObject>(); // 생성된 적군 리스트

    private void Start()
    {
        
    }

    void SpawnEnemy()
    {
        // 적의 생성 위치를 중앙을 기준으로 설정 (X축 범위를 반영)
        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnXRange, spawnXRange), // 패널의 중앙을 기준으로 X축 랜덤 생성
            spawnYPosition, // Y축 위치 설정 (패널의 위쪽)
            0 // Z축은 0으로 설정
        );

        // 적 생성 (MinigamePanel을 부모로 설정)
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemy.transform.SetParent(minigamePanel, false); // MinigamePanel의 자식으로 설정
        enemy.GetComponent<EnemyController>().minigamePanel = minigamePanel;
        enemies.Add(enemy); // 리스트에 추가
    }

    public void StartSpawnEnemy()
    {
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval); // 적 생성 반복 호출
    }

    public void StopSpawnEnemy()
    {
        CancelInvoke("SpawnEnemy");
    }


    // 모든 적군을 파괴하는 함수
    public void DestroyAllEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        enemies.Clear(); // 리스트 초기화
    }
}
