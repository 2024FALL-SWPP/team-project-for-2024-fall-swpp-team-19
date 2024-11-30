using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Vector3 targetPoint = new Vector3(250f, 0f, 250f);
    public float distanceThreshold = 100f; // �Ÿ� ����
    public float checkDuration = 2f; // üũ ���� �ð�
    private float timer = 0f; // Ÿ�̸�
    private bool isClose = false; // ����� ���� üũ

    void Update()
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Character1"); // "Character" �±׸� ���� ������Ʈ �˻�

        foreach (GameObject character in characters)
        {
            float distance = Vector3.Distance(character.transform.position, transform.position);

            if (distance <= distanceThreshold)
            {
                timer += Time.deltaTime;
                isClose = true;

                if (timer >= checkDuration)
                {
                    character.transform.position = targetPoint; // �����̵�
                    timer = 0f; // Ÿ�̸� �ʱ�ȭ
                }
            }
            else
            {
                timer = 0f; // �Ÿ��� �־����� Ÿ�̸� �ʱ�ȭ
                isClose = false;
            }
        }
    }
}
