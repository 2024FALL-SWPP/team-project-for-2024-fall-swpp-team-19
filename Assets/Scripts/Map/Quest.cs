using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Quest: MonoBehaviour
{
    public float detectionRadius = 50f; // ���� �ݰ�
    public TextMeshProUGUI warningText; // UI �ؽ�Ʈ
    private bool hasShownWarning = false; // ��� �̹� ǥ�õǾ����� üũ

    void Start()
    {
        warningText.gameObject.SetActive(false); // �ʱ⿡�� UI ��Ȱ��ȭ
    }

    void Update()
    {
        if (hasShownWarning) return; // �̹� ��� ǥ�õǾ��ٸ� ����

        // "Character" �±װ� ���� ��� ������Ʈ�� ã��
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Character");
        foreach (GameObject character in characters)
        {
            // Ư�� ��ġ�� ĳ���� ���� �Ÿ� ���
            float distance = Vector3.Distance(character.transform.position, transform.position);
            if (distance <= detectionRadius)
            {
                ShowWarning(); // ��� ǥ��
                break; // �� ���� ��� ǥ���ϹǷ� ���� ����
            }
        }
    }

    void ShowWarning()
    {
        warningText.gameObject.SetActive(true); // �ؽ�Ʈ UI Ȱ��ȭ
        hasShownWarning = true; // ��� ǥ�� �÷��� ����
        StartCoroutine(HideWarningAfterDelay(4f)); // 4�� �� ��� �����
    }

    private System.Collections.IEnumerator HideWarningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        warningText.gameObject.SetActive(false); // �ؽ�Ʈ UI ��Ȱ��ȭ
    }
}
