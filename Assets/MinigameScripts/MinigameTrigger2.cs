using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MinigameTrigger2 : MonoBehaviour
{
    public GameObject minigameUI;
    public TextMeshProUGUI textUI;
    public TextMeshProUGUI scoreText;
    public GameObject player;
    public GameObject[] buttons;

    bool isPlayerNearby = false;
    bool isMinigameActive = false;
    int currentScore;
    int targetScore;
    Coroutine startCoroutine;
    PlayerMovement playerMovement;
    GameObject activeButton;
    // Start is called before the first frame update
    void Start()
    {
        currentScore = 0;
        targetScore = 3;
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.R) && !isMinigameActive)
        {
            StartMinigame();
        }
        if (isMinigameActive && Input.GetKeyDown(KeyCode.Escape))
        {
            EndMinigame();
        }
    }

    void StartMinigame()
    {
        isMinigameActive = true;
        minigameUI.SetActive(true);
        playerMovement.EnterMinigame();
        startCoroutine = StartCoroutine(MinigameStart());
    }

    void EndMinigame()
    {
        isMinigameActive = false;
        minigameUI.SetActive(false);
        playerMovement.ExitMinigame();
        if (startCoroutine != null)
        {
            StopCoroutine(startCoroutine);
        }
        if (activeButton != null)
            activeButton.SetActive(false);
        currentScore = 0;
        scoreText.gameObject.SetActive(false);
    }

    IEnumerator MinigameStart()
    {
        UpdateScoreText();
        textUI.gameObject.SetActive(true);
        textUI.text = "Minigame Start!";
        yield return new WaitForSeconds(1f);
        for (int i = 3; i > 0; i--)
        {
            textUI.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        textUI.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
        while (currentScore < targetScore)
        {
            // 현재 활성화된 버튼을 비활성화
            if (activeButton != null)
                activeButton.SetActive(false);

            // 랜덤으로 버튼을 선택하여 활성화
            int randomIndex = Random.Range(0, buttons.Length);
            activeButton = buttons[randomIndex];
            activeButton.SetActive(true);
            
            // 버튼 클릭 시 OnButtonClicked 호출 연결
            activeButton.GetComponent<Button>().onClick.AddListener(OnButtonClicked);

            // 2초 대기
            yield return new WaitForSeconds(1.5f);
        }
        textUI.gameObject.SetActive(true);
        textUI.text = "Minigame Success!";
        yield return new WaitForSeconds(2f);
        textUI.gameObject.SetActive(false);
        EndMinigame();
    }

    private void OnButtonClicked()
    {
        if (activeButton != null)
        {
            // 버튼 비활성화 및 점수 증가
            activeButton.SetActive(false);
            currentScore++;
            UpdateScoreText();

            // 클릭 이벤트 제거 (재사용을 위해)
            activeButton.GetComponent<Button>().onClick.RemoveListener(OnButtonClicked);
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + currentScore + "/" + targetScore;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}
