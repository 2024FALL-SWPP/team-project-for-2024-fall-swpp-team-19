using Mirror;
using UnityEngine;

public class CustomGamePlayer : NetworkBehaviour
{
    [SyncVar]
    private ColorEnum color = ColorEnum.Undefined;
    public ColorEnum GetColor(){
        return color;
    }

    public void SetColor(ColorEnum newColor){
        color = newColor;
    }

    [SyncVar] public bool isInMiniGame = false;
    public PlayerInputData InputData = new PlayerInputData();
    public GameObject interactingDevice;

    private int completedMinigames = 0;
    private int minigamesForClue = 3;

    private void Update()
    {
        if (!isLocalPlayer || !isInMiniGame) return;

        PlayerInputData newInput = new PlayerInputData
        {
            MousePosition = Input.mousePosition,
            IsMouseClicked = Input.GetMouseButtonDown(0),
            IsMouseReleased = Input.GetMouseButtonUp(0),
            IsMovingUp = Input.GetKey(KeyCode.W),
            IsMovingDown = Input.GetKey(KeyCode.S),
            IsMovingLeft = Input.GetKey(KeyCode.A),
            IsMovingRight = Input.GetKey(KeyCode.D),
            IsJumping = Input.GetKey(KeyCode.Space),
            IsInteracting = Input.GetKeyDown(KeyCode.E),
            IsInteractionReleased = Input.GetKeyUp(KeyCode.E),
            IsEscape = Input.GetKey(KeyCode.Escape)
        };

        CmdUpdateInput(newInput);
    }

    [Command]
    private void CmdUpdateInput(PlayerInputData input)
    {
        InputData = input;
        Debug.Log($"[CustomGamePlayer] Server received new input from Player {netId}: " +
                  $"Left={input.IsMovingLeft}, Right={input.IsMovingRight}, Interact={input.IsInteracting}");
    }

    [Server]
    public void DestroyDevice()
    {
        NetworkServer.Destroy(interactingDevice);
    }

    public int GetCompletedMinigames()
    {
        return completedMinigames;
    }

    public int GetMinigamesForClue()
    {
        return minigamesForClue;
    }

    [Command]
    public void IncrementCompletedMinigames()
    {
        completedMinigames++;
        Debug.Log($"[CustomGamePlayer] Player {netId} completed {completedMinigames} of {minigamesForClue} games.");
        CheckClueAvailable();
    }

    [Server]
    private void CheckClueAvailable()
    {
        if (completedMinigames >= minigamesForClue)
        {
            Debug.Log($"color is {color}");
            PlayerData playerData = PlayerDataManager.Instance.GetPlayerData(color);
            Debug.Log($"playerData.target = {playerData.target}");
            if (isLocalPlayer)
                ToggleManager.Instance.TargetToggleReveal(playerData.target);
            completedMinigames = 0;
            if (minigamesForClue > 1)
            {
                minigamesForClue--;
            }
        }
    }
}
