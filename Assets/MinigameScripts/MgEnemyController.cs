using UnityEngine;

public class MgEnemyController : MonoBehaviour
{
    public float speed = 100f;
    public RectTransform minigamePanel; // MinigamePanel의 RectTransform을 직접 연결

    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);

        // Y 위치를 검사하여 파괴
        if (transform.localPosition.y < -minigamePanel.rect.height / 2 + 13f)
        {
            Destroy(gameObject);
        }
    }
}
