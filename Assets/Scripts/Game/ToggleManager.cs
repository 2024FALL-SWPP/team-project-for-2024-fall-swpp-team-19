using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Reflection.Emit;

public class ToggleManager : MonoBehaviour
{
    public GameObject togglePrefab; // 미리 만들어둔 Toggle Prefab
    public Transform toggleContainer; // Vertical Layout Group이 있는 컨테이너

    private List<Toggle> toggles = new List<Toggle>();

    public void InitializeToggles()
    {
        CustomRoomManager customRoomManager = (CustomRoomManager)NetworkManager.singleton;

        if (customRoomManager == null || customRoomManager.roomSlots.Count == 0)
        {
            Debug.LogError("No players found in the room.");
            return;
        }

        // 기존 토글 제거
        foreach (Transform child in toggleContainer)
        {
            Destroy(child.gameObject);
        }
        toggles.Clear();

        CustomRoomPlayer localRoomPlayer = NetworkClient.localPlayer.GetComponent<CustomRoomPlayer>();

        if (localRoomPlayer == null)
        {
            Debug.LogError("LocalRoomPlayer is null.");
            return;
        }

        // 나의 색상
        ColorEnum myColor = localRoomPlayer.GetColor();

        foreach (NetworkRoomPlayer roomPlayer in customRoomManager.roomSlots)
        {
            if (roomPlayer is CustomRoomPlayer customRoomPlayer)
            {
                // 토글 생성
                Toggle toggle = Instantiate(togglePrefab, toggleContainer).GetComponent<Toggle>();
                toggles.Add(toggle);

                ActivateChildObjects(toggle);
                // 토글의 색상 설정
                SetToggleColor(toggle, customRoomPlayer.GetColor());
                // 본인의 색상일 경우에 text를 "Me"로 설정
                if (customRoomPlayer.GetColor() == myColor)
                {
                    SetToggleLabelText(toggle, "Me");
                }
                else{
                    DisableToggleLabelText(toggle);
                }
                // 토글의 상태 설정 (기본값: 꺼짐)
                toggle.isOn = false;
            }
        }

        // 레이아웃 갱신
        ForceLayoutRebuild();
    }

    //Toggle의 색상을 설정하는 함수
    private void SetToggleColor(Toggle toggle, ColorEnum colorEnum)
    {
        // ColorEnum에 따라 색상 설정
        Color color;
        switch (colorEnum)
        {
            case ColorEnum.Black:
                color = Color.black;
                break;
            case ColorEnum.Blue:
                color = Color.blue;
                break;
            case ColorEnum.Green:
                color = Color.green;
                break;
            case ColorEnum.Red:
                color = Color.red;
                break;
            case ColorEnum.White:
                color = Color.white;
                break;
            case ColorEnum.Pink:
                color = new Color(1f, 0.4f, 0.7f); // Pink (임의의 RGB 값)
                break;
            case ColorEnum.Purple:
                color = new Color(0.5f, 0f, 0.5f); // Purple
                break;
            case ColorEnum.Yellow:
                color = Color.yellow;
                break;
            default:
                color = Color.gray; // Undefined 색상
                break;
        }

        // 토글의 배경 이미지 색상 변경
        Image backgroundImage = toggle.targetGraphic as Image;
        if (backgroundImage != null)
        {
            Debug.Log("Background Image Found, Color changed to " + color);
            backgroundImage.color = color;
        }
    }

    private void ForceLayoutRebuild()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(toggleContainer.GetComponent<RectTransform>());
    }


    public void ActivateChildObjects(Toggle toggle)
    {
        // 하위 오브젝트 모두 활성화
        foreach (Transform child in toggle.transform)
        {
            child.gameObject.SetActive(true);

            // 하위에 있는 Image 컴포넌트 활성화
            Image imageComponent = child.GetComponent<Image>();
            if (imageComponent != null)
            {
                imageComponent.enabled = true;
            }
        }
    }



    // Toggle의 label의 text를 변경하는 함수
    // Target으로 지정할 시에 사용하면 됩니다.
    // ex) SetToggleLabelText(toggle, "Target");
    public void SetToggleLabelText(Toggle toggle, string text)
    {
        // Toggle 하위에 있는 Label 하위에 있는 Text 컴포넌트를 찾아서 text 변경
        foreach (Transform child in toggle.transform)
        {
            Text textComponent = child.GetComponent<Text>();
            if (textComponent != null)
            {
                textComponent.text = text;
                textComponent.enabled = true;
            }
        }
    }

    // Toggle의 label의 text를 disabled로 변경하는 함수
    // Target이 변경됐을 때 사용하면 됩니다.
    public void DisableToggleLabelText(Toggle toggle)
    {
        // Toggle 하위에 있는 Label 하위에 있는 Text 컴포넌트를 찾아서 text 변경
        foreach (Transform child in toggle.transform)
        {
            Text textComponent = child.GetComponent<Text>();
            if (textComponent != null)
            {
                textComponent.enabled = false;
            }
        }
    }

    // Toggle을 비활성화하는 함수
    // 플레이어가 사망했을 때 사용하면 됩니다.
    public void SetToggleUninteractable(Toggle toggle)
    {
        toggle.interactable = false;
    }
}