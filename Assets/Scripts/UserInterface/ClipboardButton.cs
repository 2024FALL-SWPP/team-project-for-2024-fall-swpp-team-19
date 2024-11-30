using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ClipboardButton : MonoBehaviour
{
    private Button clipboardButton;

    private void Start()
    {
        clipboardButton = GetComponent<Button>();

        if (clipboardButton == null)
        {
            Debug.LogError("No Button component found on this GameObject.");
            return;
        }

        clipboardButton.onClick.AddListener(CopyRoomCodeToClipboard);
    }

    public void CopyRoomCodeToClipboard()
    {
        CustomRoomManager roomManager = CustomRoomManager.singleton as CustomRoomManager;

        if (roomManager != null)
        {
            string roomCode = roomManager.GetRoomCode();

            if (!string.IsNullOrEmpty(roomCode))
            {
                GUIUtility.systemCopyBuffer = roomCode;
                Debug.Log($"Room code copied to clipboard: {roomCode}");
            }
            else
            {
                Debug.LogWarning("Room code is empty or not available.");
            }
        }
        else
        {
            Debug.LogError("CustomRoomManager singleton is null.");
        }
    }
}
