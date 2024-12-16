using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TestScript : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {   
 
        Invoke("DelayedMethod", 2f);
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
