using UnityEngine;

public class MgPlayerController : MonoBehaviour
{
    public float speed = 20f;             // 플레이어 이동 속도
    public GameObject bulletPrefab;      // 발사할 총알 프리팹
    public Transform firePoint;          // 총알 발사 위치
    public RectTransform minigamePanel; // MinigamePanel의 RectTransform

    void Start()
    {
        // MinigamePanel의 RectTransform 가져오기
        RectTransform panelRectTransform = minigamePanel.GetComponent<RectTransform>();

        // MinigamePanel의 중심 위치 계산
        Vector3 panelCenter = panelRectTransform.anchoredPosition;

        // 패널 중앙에 플레이어 전투기 위치 설정 (예: Y축 아래로 120 유닛 이동)
        transform.localPosition = new Vector3(panelCenter.x, panelCenter.y - 120f, 0);
    }

    void Update()
    {
        // 좌우 이동
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.localPosition += Vector3.right * horizontalInput * speed * Time.deltaTime;

        // 화면 밖으로 나가지 않도록 제한
        float clampedX = Mathf.Clamp(transform.localPosition.x, -87f, 87f);
        transform.localPosition = new Vector2(clampedX, transform.localPosition.y);

        // firePoint 위치도 플레이어 위치에 상대적으로 조정
        firePoint.localPosition = transform.localPosition + new Vector3(0, 20f, 0); // 플레이어 바로 위에 총알 발사 위치 설정 (예시 값)

        // 발사
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // 적 생성 (MinigamePanel을 부모로 설정)
        GameObject bullet = Instantiate(bulletPrefab, firePoint.localPosition, Quaternion.identity);;
        bullet.transform.SetParent(minigamePanel, false); // MinigamePanel의 자식으로 설정
        bullet.GetComponent<Bullet>().minigamePanel = minigamePanel;
    }
}
