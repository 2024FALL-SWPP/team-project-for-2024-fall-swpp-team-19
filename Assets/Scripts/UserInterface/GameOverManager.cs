using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public GameObject go2TitleButton;
    public GameObject go2LobbyButton;

    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StayLobby()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
    }

    public void Go2Title()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
    }
}
