using UnityEngine;
using Mirror;

public class MgGameSessionManager : NetworkBehaviour
{
    [SyncVar] private bool isGameInProgress = false; // 게임 진행 여부 (서버에서 동기화)

    public static GameSessionManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 게임 시작 요청 (클라이언트가 호출)
    [Command]
    public void CmdRequestStartGame(NetworkConnectionToClient sender = null)
    {
        if (!isGameInProgress)
        {
            isGameInProgress = true;
            TargetConfirmGameStart(sender); // 게임 시작 허가
        }
        else
        {
            TargetDenyGameStart(sender); // 다른 플레이어가 게임 중일 때 거부
        }
    }

    // 게임 종료 요청 (클라이언트가 호출)
    [Command]
    public void CmdEndGame()
    {
        isGameInProgress = false;
    }

    // 클라이언트에게 게임 시작 가능을 알림
    [TargetRpc]
    private void TargetConfirmGameStart(NetworkConnection target)
    {
        // MinigameTrigger.Instance.OnGameStartConfirmed();
    }

    // 클라이언트에게 게임 시작 불가를 알림
    [TargetRpc]
    private void TargetDenyGameStart(NetworkConnection target)
    {
        Debug.Log("다른 플레이어가 이미 게임을 진행 중입니다.");
    }
}
