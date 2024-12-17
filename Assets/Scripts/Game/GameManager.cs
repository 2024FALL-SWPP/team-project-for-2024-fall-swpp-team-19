using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject settingPanelPrefab; // 프리팹 참조
    private GameObject settingPanelInstance; // 생성된 패널 인스턴스

    void Start(){
        Debug.Log("GameManager Start");
    }
    void Update(){
        if(settingPanelInstance != null){
            if(settingPanelInstance.activeSelf == false){
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingPanel();
        }
    }
    private void ToggleSettingPanel()
    {
        if (settingPanelInstance == null)
        {
            Cursor.lockState = CursorLockMode.None;
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
            if (isActive)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }else{
                Cursor.lockState = CursorLockMode.None;
            }
            settingPanelInstance.SetActive(!isActive);
        }
    }

}
