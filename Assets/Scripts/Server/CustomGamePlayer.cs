using Mirror;
using UnityEngine;

public class CustomGamePlayer : NetworkBehaviour
{
    public PlayerInputData InputData = new PlayerInputData();
    public bool isInMiniGame = false;

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
            IsInteracting = Input.GetKeyDown(KeyCode.E),
            IsInteractionReleased = Input.GetKeyUp(KeyCode.E)
        };

        CmdUpdateInput(newInput);
    }

    [Command]
    public void CmdUpdateInput(PlayerInputData input)
    {
        InputData = input;
    }

    public void EnterMiniGame()
    {
        isInMiniGame = true;
    }

    public void ExitMiniGame()
    {
        isInMiniGame = false;
    }
}
