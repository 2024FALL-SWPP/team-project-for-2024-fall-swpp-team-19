using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameOverManager : NetworkBehaviour
{
    public GameObject go2TitleButton;
    public GameObject go2LobbyButton;
    private ColorEnum color = ColorEnum.Undefined;
    public GameObject[] characters;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;

        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(false);
        }

        if (PlayerDataManager.Instance != null)
        {
            foreach (var entry in PlayerDataManager.Instance.playerDataMap)
            {
                if (entry.Value != null && entry.Value.isAlive)
                {
                    Debug.Log("Color: " + entry.Key);
                    color = entry.Key;
                    break;
                }
            }

            if (color != ColorEnum.Undefined)
            {
                characters[(int)color].SetActive(true);
            }

            foreach (var key in PlayerDataManager.Instance.playerDataMap.Keys)
            {
                PlayerDataManager.Instance.CmdUpdatePlayerData(key, "isAlive", true);
                PlayerDataManager.Instance.CmdUpdatePlayerData(key, "color", ColorEnum.Undefined);
            }
        }

        Debug.Log("GameOverManager Start: " + NetworkManager.singleton);
    }

    public void StayLobby()
    {
        CustomRoomManager.Instance.ServerChangeScene("LobbyScene");
    }

    public void Go2Title()
    {
        CustomRoomManager.Instance.ReturnToTitle();
    }
}
