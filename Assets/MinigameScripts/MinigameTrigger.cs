using UnityEngine;
using TMPro;
using System.Collections;

public class MinigameTrigger : MonoBehaviour
{
    public GameObject player;                     // 플레이어 오브젝트
    public GameObject minigameUI;                 // 미니게임 UI 패널
    public TextMeshProUGUI scoreText;             // 점수 텍스트 (TextMeshPro)
    public GameObject enemySpawner;               // 적 생성기 오브젝트
    public TextMeshProUGUI titleText;             // 게임 제목 텍스트 객체
    public TextMeshProUGUI countdownText;         // 카운트다운 텍스트 객체
    public TextMeshProUGUI clearText;             // 게임 클리어 텍스트 객체

    private float titleDisplayTime = 1f;          // 제목 표시 시간
    private float countdownDisplayTime = 1f;      // 카운트다운 표시 시간
    private float clearMessageDisplayTime = 2f;   // 클리어 메시지 표시 시간
    private PlayerMovement playerMovement;
    private bool isPlayerNearby = false;
    private bool isMinigameActive = false;
    private int score = 0;
    private int targetKillCount = 3;
    private Coroutine startMinigameCoroutine;     // StartMinigame 코루틴을 추적하는 변수

    private void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        minigameUI.SetActive(false);              // 미니게임 UI 비활성화 상태로 시작
        scoreText.gameObject.SetActive(false);
        enemySpawner.SetActive(false);            // 적 생성기도 비활성화 상태로 시작
        titleText.gameObject.SetActive(false);    // 게임 제목 텍스트 비활성화
        countdownText.gameObject.SetActive(false);// 카운트다운 텍스트 비활성화
        clearText.gameObject.SetActive(false);    // 클리어 텍스트 비활성화
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.R) && !isMinigameActive)
        {
            startMinigameCoroutine = StartCoroutine(StartMinigame()); // 코루틴 시작 시 변수에 할당
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMinigameActive)
            {
                EndMinigame();
            }
            else if (startMinigameCoroutine != null)
            {
                StopCoroutine(startMinigameCoroutine); // 코루틴 중지
                EndMinigame();
                startMinigameCoroutine = null;
                Debug.Log("Minigame start canceled by Escape key.");
            }
        }
    }

    private IEnumerator StartMinigame()
    {
        playerMovement.EnterMinigame();

        // 게임 제목 표시
        minigameUI.SetActive(true);
        titleText.gameObject.SetActive(true);
        titleText.text = "Minigame Start!";
        yield return new WaitForSeconds(titleDisplayTime);
        titleText.gameObject.SetActive(false);

        // 카운트다운 시작
        countdownText.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(countdownDisplayTime);
        }
        countdownText.gameObject.SetActive(false);

        // 미니게임 UI 및 적 생성기 활성화
        enemySpawner.SetActive(true); 
        enemySpawner.GetComponent<EnemySpawner>().StartSpawnEnemy();
        
        score = 0;
        scoreText.gameObject.SetActive(true);
        UpdateScoreText();
        isMinigameActive = true;

        startMinigameCoroutine = null; // 코루틴 종료 후 변수 초기화
    }

    private void EndMinigame()
    {
        isMinigameActive = false;
        playerMovement.ExitMinigame();

        // 미니게임 UI 및 적 생성기 비활성화
        enemySpawner.GetComponent<EnemySpawner>().StopSpawnEnemy();
        enemySpawner.GetComponent<EnemySpawner>().DestroyAllEnemies();
        scoreText.gameObject.SetActive(false);
        enemySpawner.SetActive(false);
        minigameUI.SetActive(false);
        Debug.Log("Game Over! Final Score: " + score);
    }

    public void IncrementScore()
    {
        score++;
        UpdateScoreText();

        if (score >= targetKillCount)
        {
            ClearEnemies();
            StartCoroutine(HandleGameClear()); // 목표 달성 시 클리어 처리
        }
    }

    private IEnumerator HandleGameClear()
    {
        clearText.gameObject.SetActive(true);   // 클리어 텍스트 활성화
        clearText.text = "Congratulations!\nGame Cleared!";
        yield return new WaitForSeconds(clearMessageDisplayTime);

        clearText.gameObject.SetActive(false);  // 클리어 텍스트 비활성화
        EndMinigame();                          // 미니게임 종료
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString() + "/" + targetKillCount.ToString();
    }

    void ClearEnemies()
    {
        enemySpawner.GetComponent<EnemySpawner>().StopSpawnEnemy();
        enemySpawner.GetComponent<EnemySpawner>().DestroyAllEnemies();
    }
}
