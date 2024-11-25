using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    public GameObject gameButtonGroup;
    public GameObject hostInputField;
    public GameObject controlButtonGroup;
    // Start is called before the first frame update
    void Start()
    {
        gameButtonGroup.SetActive(false);
        hostInputField.gameObject.SetActive(false);
        
    }

    public void PlayGameButton()
    {
        gameButtonGroup.SetActive(true);
        controlButtonGroup.SetActive(false);
    }

    public void ExitButton()
    {
        Application.Quit();
    }


    public void CreateLobbyButton()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void JoinButton()
    {
        hostInputField.gameObject.SetActive(true);
        gameButtonGroup.SetActive(false);
    }

    public void ConfigurateButton()
    {
        SceneManager.LoadScene("ConfigScene");
    }

    public void UndoButton()
    {
        gameButtonGroup.SetActive(false);
        controlButtonGroup.SetActive(true);
        hostInputField.gameObject.SetActive(false);
    }

    public void PreferenceButton(){
        //configurate button
        SceneManager.LoadScene("ConfigScene");
    }
}
