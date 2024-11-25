using UnityEngine;

public class MgBullet : MonoBehaviour
{
    public float speed = 200f;
    public RectTransform minigamePanel; // MinigamePanel의 RectTransform
    public MinigameTrigger minigameTrigger;

    private float upperBoundary;

    void Start()
    {
        // MinigamePanel의 상단 경계를 기준으로 파괴 위치 설정
        RectTransform panelRectTransform = minigamePanel.GetComponent<RectTransform>();
        upperBoundary = panelRectTransform.rect.height / 2 - 50f; // 패널의 상단 경계값
        minigameTrigger = GameObject.Find("TriggerCube1").GetComponent<MinigameTrigger>();
    }

    void Update()
    {
        // 로컬 좌표 기준으로 위쪽으로 이동
        transform.Translate(Vector2.up * speed * Time.deltaTime);

        // 화면 밖으로 나가면 파괴
        if (transform.localPosition.y > upperBoundary)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject); // 적 전투기 파괴
            Destroy(gameObject); // 총알 파괴
            minigameTrigger.IncrementScore();
        }
    }
}
