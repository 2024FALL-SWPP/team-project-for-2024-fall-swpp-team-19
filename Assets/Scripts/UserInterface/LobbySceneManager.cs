using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LobbySceneManager : MonoBehaviour
{
    private bool isReady = false;

    public Toggle[] toggles;
    public GameObject[] characters;
    public Animator animator;

    public Button readyButton;
    public TextMeshProUGUI readyButtonText;
    public Color readyColor = Color.green;
    public Color notReadyColor = Color.red;

    public TextMeshProUGUI clipboardButtonText;
    public TextMeshProUGUI playerCountText;

    private GameObject selectedCharacter;

    private int previousPlayerCount = -1;
    private string previousRoomCode = "";

    void Start()
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            int index = i;
            toggles[i].onValueChanged.AddListener((bool isOn) => ToggleCharacter(toggles[index], characters[index]));
            characters[i].SetActive(false);
        }

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
    }

    void ToggleCharacter(Toggle toggle, GameObject character)
    {
        if (toggle.isOn)
        {
            if (selectedCharacter != null)
                selectedCharacter.SetActive(false);

            foreach (var otherToggle in toggles)
            {
                if (otherToggle != toggle)
                    otherToggle.isOn = false;
            }

            selectedCharacter = character;
            selectedCharacter.SetActive(true);
            readyButton.interactable = true;
        }
        else if (selectedCharacter == character)
        {
            selectedCharacter = null;
            isReady = false;
            UpdateReadyButtonUI();
            character.SetActive(false);
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
}
