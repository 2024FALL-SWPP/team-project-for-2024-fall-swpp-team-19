using System.Collections;
using System.Collections.Generic;
using Edgegap;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneManager : MonoBehaviour
{
    private bool isReady = false;

    public Toggle[] toggles;
    public GameObject[] characters;

    public Button readyButton;
    public TextMeshProUGUI readyButtonText; // Button의 Text 컴포넌트
    public Color readyColor = Color.green;
    public Color notReadyColor = Color.red;

    private GameObject selectedCharacter;

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

        // 초기 색상 설정
        UpdateReadyButtonUI();
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
            isReady = !isReady; // 준비 상태를 반전
            UpdateReadyButtonUI();
            
            if (isReady)
                Debug.Log("Ready with character: " + selectedCharacter.name);
            else
                Debug.Log("Not ready");
        }
        else
        {
            Debug.Log("캐릭터를 선택하세요!");
        }
    }

    void UpdateReadyButtonUI()
    {
        if (isReady)
        {
            readyButtonText.text = "Ready";
            readyButton.image.color = readyColor;
        }
        else
        {
            readyButtonText.text = "Not Ready";
            readyButton.image.color = notReadyColor;
        }
    }
}
