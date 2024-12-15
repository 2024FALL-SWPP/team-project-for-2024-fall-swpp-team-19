using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MoleMinigame : MiniGameBase
{
    public Text countdownText;
    public Text scoreText;
    public GameObject[] buttons;
    public GameObject[] moles;

    private int score = 0;
    private int targetScore = 100;
    private GameObject activeButton;
    private GameObject activeMole;

    [Server]
    public override void StartGame()
    {
        base.StartGame();
        Debug.Log("[MoleMinigame] Starting game.");
        StartCoroutine(CountdownAndStart());
    }

    [Server]
    private IEnumerator CountdownAndStart()
    {
        score = 0;
        scoreText.text = "";
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        countdownText.text = "";
        scoreText.text = "Score: " + score.ToString() + "/" + targetScore.ToString();

        while (score < targetScore)
        {
            if (activeButton != null)
                activeButton.SetActive(false);

            if (activeMole != null)
                activeMole.SetActive(false);

            int randomIndex = Random.Range(0, buttons.Length);
            activeButton = buttons[randomIndex];
            activeMole = moles[randomIndex];
            activeButton.SetActive(true);
            activeMole.SetActive(true);
            
            activeButton.GetComponent<Button>().onClick.AddListener(OnButtonClicked);

            yield return new WaitForSeconds(1.5f);
        }
    }

    [Server]
    private void OnButtonClicked()
    {
        if (activeButton != null)
        {
            activeButton.SetActive(false);
            activeMole.SetActive(false);
            score++;
            scoreText.text = "Score: " + score.ToString() + "/" + targetScore.ToString();

            activeButton.GetComponent<Button>().onClick.RemoveListener(OnButtonClicked);
        }
    }
}
