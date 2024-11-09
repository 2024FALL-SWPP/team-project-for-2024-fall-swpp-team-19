using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Quest: MonoBehaviour
{
    public float detectionRadius = 50f; // 감지 반경
    public TextMeshProUGUI warningText; // UI 텍스트
    private bool hasShownWarning = false; // 경고가 이미 표시되었는지 체크

    void Start()
    {
        warningText.gameObject.SetActive(false); // 초기에는 UI 비활성화
    }

    void Update()
    {
        if (hasShownWarning) return; // 이미 경고가 표시되었다면 종료

        // "Character" 태그가 붙은 모든 오브젝트를 찾음
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Character");
        foreach (GameObject character in characters)
        {
            // 특정 위치와 캐릭터 간의 거리 계산
            float distance = Vector3.Distance(character.transform.position, transform.position);
            if (distance <= detectionRadius)
            {
                ShowWarning(); // 경고 표시
                break; // 한 번만 경고를 표시하므로 루프 종료
            }
        }
    }

    void ShowWarning()
    {
        warningText.gameObject.SetActive(true); // 텍스트 UI 활성화
        hasShownWarning = true; // 경고 표시 플래그 설정
        StartCoroutine(HideWarningAfterDelay(4f)); // 4초 후 경고 숨기기
    }

    private System.Collections.IEnumerator HideWarningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        warningText.gameObject.SetActive(false); // 텍스트 UI 비활성화
    }
}
