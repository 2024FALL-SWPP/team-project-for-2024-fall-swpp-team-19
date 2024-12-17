using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Unity.VisualScripting;

public class GameOverManager : NetworkBehaviour
{
    public GameObject go2TitleButton;
    public GameObject go2LobbyButton;
    private ColorEnum color = ColorEnum.Undefined;
    public GameObject[] characters;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        // inactivate the character
        for(int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(false);
        }
        CustomRoomManager customRoomManager = (CustomRoomManager)NetworkManager.singleton;
        foreach (NetworkRoomPlayer roomPlayer in customRoomManager.roomSlots)
         {
             if (roomPlayer is CustomRoomPlayer customRoomPlayer) { 
                    if(customRoomPlayer.GetIsAlive())
                    {   
                        Debug.Log("Color: " + customRoomPlayer.GetColor());
                        color = customRoomPlayer.GetColor();
                        break;
                    }
              } 
         }
        characters[(int)color].SetActive(true);

        foreach (NetworkRoomPlayer roomPlayer in customRoomManager.roomSlots)
         {
             if (roomPlayer is CustomRoomPlayer customRoomPlayer) { 
                    customRoomPlayer.SetIsAlive(true);
                    customRoomPlayer.SetColor(ColorEnum.Undefined);
              } 
         }
        Debug.Log("GameOverManager start "+NetworkManager.singleton);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("GameOverManager start "+NetworkManager.singleton);
        
    }


    public void StayLobby()
    {
        CustomRoomManager customRoomManager=(CustomRoomManager)NetworkManager.singleton;
        customRoomManager.ServerChangeScene("LobbyScene");
    }

    public void Go2Title()
    {
        CustomRoomManager.Instance.ReturnToTitle();
    }

    void GetColor()
    {
        // CustomRoomPlayer roomPlayer = NetworkClient.connection.identity.GetComponent<CustomRoomPlayer>();
        // color = roomPlayer.color;
    }
}
