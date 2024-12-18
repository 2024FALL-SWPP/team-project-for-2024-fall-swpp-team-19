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

    private GameObject activeButton;
    private GameObject activeMole;

    
    public override void StartGame()
    {
        base.StartGame();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("[MoleMinigame] Starting game.");
        base.score = 0;
        base.targetScore = 3;
        StartCoroutine(CountdownAndStart());
    }

    
    private IEnumerator CountdownAndStart()
    {
        scoreText.text = "";
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        countdownText.text = "";
        scoreText.text = "Score: " + base.score.ToString() + "/" + base.targetScore.ToString();

        while (base.score < base.targetScore)
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

    
    private void OnButtonClicked()
    {
        if (activeButton != null)
        {
            activeButton.SetActive(false);
            activeMole.SetActive(false);
            base.score++;
            scoreText.text = "Score: " + base.score.ToString() + "/" + base.targetScore.ToString();

            activeButton.GetComponent<Button>().onClick.RemoveListener(OnButtonClicked);
        }
    }

    
    public override void ClearGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        base.ClearGame();
    }
}
