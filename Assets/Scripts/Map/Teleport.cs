using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Vector3 targetPoint = new Vector3(250f, 0f, 250f);
    public float distanceThreshold = 100f; // 거리 기준
    public float checkDuration = 2f; // 체크 지속 시간
    private float timer = 0f; // 타이머
    private bool isClose = false; // 가까운 상태 체크

    void Update()
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Character1"); // "Character" 태그를 가진 오브젝트 검색

        foreach (GameObject character in characters)
        {
            float distance = Vector3.Distance(character.transform.position, transform.position);

            if (distance <= distanceThreshold)
            {
                timer += Time.deltaTime;
                isClose = true;

                if (timer >= checkDuration)
                {
                    character.transform.position = targetPoint; // 순간이동
                    timer = 0f; // 타이머 초기화
                }
            }
            else
            {
                timer = 0f; // 거리가 멀어지면 타이머 초기화
                isClose = false;
            }
        }
    }
}
