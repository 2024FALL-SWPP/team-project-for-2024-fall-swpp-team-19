using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;

public class LobbySceneManager : MonoBehaviour
{
    private bool isReady = false;
    ColorEnum[] colors = (ColorEnum[])Enum.GetValues(typeof(ColorEnum));

    public Toggle[] toggles;
    public GameObject[] characters;
    public Animator animator;
    public Button exitButton;
    public Button readyButton;
    public TextMeshProUGUI readyButtonText;
    public Color readyColor = Color.green;
    public Color notReadyColor = Color.red;

    public TextMeshProUGUI clipboardButtonText;
    public TextMeshProUGUI playerCountText;

    public GameObject settingPanelPrefab; // 프리팹 참조
    private GameObject settingPanelInstance; // 생성된 패널 인스턴스
    private GameObject selectedCharacter;

    private int previousPlayerCount = -1;
    private string previousRoomCode = "";
    

    void Start()
    {
       
        for (int i = 0; i < toggles.Length; i++)
        {
            int index = i;
            toggles[i].onValueChanged.AddListener((bool isOn) => ToggleCharacter(index));
            characters[i].SetActive(false);
        }

        exitButton.onClick.AddListener(Go2TitleButton);
        readyButton.onClick.AddListener(OnReady);
        readyButton.interactable = false;

        UpdateReadyButtonUI();
        UpdatePlayerCount();
        UpdateClipboardButtonUI(); // Ensure the room code is set at the start
        animator.SetBool("running_b", true);
        animator.SetBool("victory_b", false);
    }

    void Update()
    {
        CustomRoomManager roomManager = NetworkManager.singleton as CustomRoomManager;
        if (roomManager != null)
        {
            int currentPlayerCount = roomManager.roomSlots.Count;
            if (currentPlayerCount != previousPlayerCount)
            {
                UpdatePlayerCount();
                previousPlayerCount = currentPlayerCount;
            }

            string currentRoomCode = roomManager.GetRoomCode();
            if (currentRoomCode != previousRoomCode)
            {
                UpdateClipboardButtonUI();
                previousRoomCode = currentRoomCode;
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingPanel();
            
        }
    }

    void ToggleCharacter(int index)
    {   
        ColorEnum currentColor = colors[index];
        bool isColorRedundant = false;
        CustomRoomManager customRoomManager = (CustomRoomManager)NetworkManager.singleton;
        foreach (NetworkRoomPlayer roomPlayer in customRoomManager.roomSlots)
         {
             if (roomPlayer is CustomRoomPlayer customRoomPlayer) { 
                    Debug.Log("CustomRoomPlayer color : " + customRoomPlayer.GetColor());
                    Debug.Log("Current color : " + currentColor);
                    if (customRoomPlayer.GetColor() == currentColor)
                    {
                        isColorRedundant = true;
                        // break;
                    }
              } 
         }
        Debug.Log("roomSlot length : " + customRoomManager.roomSlots.Count);
        if(isColorRedundant)
        {
            Debug.Log("Color is already taken");
            // color check
            toggles[index].isOn = false;
            
            return;            
        }

        CustomRoomPlayer localRoomPlayer = NetworkClient.localPlayer.GetComponent<CustomRoomPlayer>();
        Debug.Log($"Local player color, index: {localRoomPlayer.GetColor()}, {index}");
        localRoomPlayer.SetColor(colors[index]);
        if (toggles[index].isOn)
        {
            if (selectedCharacter != null)
                selectedCharacter.SetActive(false);

            foreach (var otherToggle in toggles)
            {
                if (otherToggle != toggles[index])
                    otherToggle.isOn = false;
            }

            selectedCharacter = characters[index];
            selectedCharacter.SetActive(true);
            readyButton.interactable = true;
        }
        else if (selectedCharacter == characters[index])
        {
            selectedCharacter = null;
            isReady = false;
            UpdateReadyButtonUI();
            characters[index].SetActive(false);
            readyButton.interactable = false;
        }
    }

    void OnReady()
    {
        if (selectedCharacter != null)
        {
            isReady = !isReady;

            NetworkIdentity localIdentity = NetworkClient.localPlayer;
            if (localIdentity != null)
            {
                NetworkRoomPlayer roomPlayer = localIdentity.GetComponent<NetworkRoomPlayer>();
                if (roomPlayer != null)
                {
                    roomPlayer.CmdChangeReadyState(isReady);
                }
                else
                {
                    Debug.LogError("Local player does not have a NetworkRoomPlayer component.");
                }
            }
            else
            {
                Debug.LogError("Local player is not found.");
            }

            UpdateReadyButtonUI();
        }
        else
        {
            Debug.Log("Please select a character!");
        }
    }

    void UpdateReadyButtonUI()
    {
        readyButtonText.text = isReady ? "Ready" : "Not Ready";
        readyButton.image.color = isReady ? readyColor : notReadyColor;
    }

    void UpdateClipboardButtonUI()
    {
        CustomRoomManager roomManager = NetworkManager.singleton as CustomRoomManager;

        if (roomManager != null && clipboardButtonText != null)
        {
            string roomCode = roomManager.GetRoomCode();
            clipboardButtonText.text = !string.IsNullOrEmpty(roomCode) 
                ? $"Room Code: {roomCode}" 
                : "Room Code: Not Available";
        }
        else
        {
            clipboardButtonText.text = "Room Code: Error";
        }
    }

    void UpdatePlayerCount()
    {
        CustomRoomManager roomManager = NetworkManager.singleton as CustomRoomManager;

        if (roomManager != null)
        {
            int playerCount = roomManager.roomSlots.Count;
            int maxPlayers = roomManager.maxConnections;

            playerCountText.text = $"Players: {playerCount}/{maxPlayers}";
        }
        else
        {
            playerCountText.text = "Players: Error";
        }
    }

    void Go2TitleButton(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
    }

     private void ToggleSettingPanel()
    {
        if (settingPanelInstance == null)
        {
            // Find the Canvas in the scene
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                // 세팅 패널이 없으면 프리팹에서 생성
                settingPanelInstance = Instantiate(settingPanelPrefab);
                settingPanelInstance.transform.SetParent(canvas.transform, false); // 부모를 Canvas로 설정
                settingPanelInstance.SetActive(true);
            }
            else
            {
                Debug.LogError("Canvas not found in the scene.");
            }
        }
        else
        {
            // 세팅 패널이 이미 생성된 경우 활성화/비활성화 토글
            bool isActive = settingPanelInstance.activeSelf;
            settingPanelInstance.SetActive(!isActive);
        }
    }

}
