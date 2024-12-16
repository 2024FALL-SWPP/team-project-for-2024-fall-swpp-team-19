using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class TestScript : NetworkBehaviour
{
    public ToggleManager toggleManager;
    // Start is called before the first frame update
    void Start()
    {   
        toggleManager.InitializeToggles();
        // Invoke("DelayedMethod", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DelayedMethod(){
         CustomRoomManager customRoomManager = (CustomRoomManager)NetworkManager.singleton;
        Cursor.lockState = CursorLockMode.None;
         customRoomManager.ServerChangeScene("LobbyScene");
    }
}
