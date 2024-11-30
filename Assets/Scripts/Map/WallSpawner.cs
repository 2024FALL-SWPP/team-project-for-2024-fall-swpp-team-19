using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    public GameObject wallPrefab; // 생성할 벽 프리팹
    public Vector3 spawnPosition1 = new Vector3(475, 0, 345);
    public Vector3 spawnPosition2 = new Vector3(500, 0, 655);
    public Vector3 spawnPosition3 = new Vector3(345, 0, 500);
    public Vector3 spawnPosition4 = new Vector3(655, 0, 500);
    //public Vector3 spawnPosition5 = new Vector3(25, 0, 475);
    //public Vector3 spawnPosition6 = new Vector3(475, 0, 975);
    //public Vector3 spawnPosition7 = new Vector3(975, 0, 525);
    //public Vector3 spawnPosition8 = new Vector3(525, 0, 25);
    Quaternion rotation = Quaternion.Euler(0, 90, 0);
    public float spawnInterval = 5f; // 5초 간격

    void Start()
    {
        StartCoroutine(SpawnAndDestroyWall());
    }

    private IEnumerator SpawnAndDestroyWall()
    {
        while (true) // 무한 반복
        {
            // 벽 생성
            GameObject wall1 = Instantiate(wallPrefab, spawnPosition1, rotation);
            GameObject wall2 = Instantiate(wallPrefab, spawnPosition2, rotation);
            GameObject wall3 = Instantiate(wallPrefab, spawnPosition3, Quaternion.identity);
            GameObject wall4 = Instantiate(wallPrefab, spawnPosition4, Quaternion.identity);
            //GameObject wall5 = Instantiate(wallPrefab, spawnPosition5, Quaternion.identity);
            //GameObject wall6 = Instantiate(wallPrefab, spawnPosition6, rotation);
            //GameObject wall7 = Instantiate(wallPrefab, spawnPosition7, Quaternion.identity);
            //GameObject wall8 = Instantiate(wallPrefab, spawnPosition8, rotation);
            
            // 5초 후에 벽 제거
            yield return new WaitForSeconds(spawnInterval);
            Destroy(wall1);
            Destroy(wall2);
            Destroy(wall3);
            Destroy(wall4);
            //Destroy(wall5);
            //Destroy(wall6);
            //Destroy(wall7);
            //Destroy(wall8);
            
            // 다음 벽 생성을 위해 대기
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
