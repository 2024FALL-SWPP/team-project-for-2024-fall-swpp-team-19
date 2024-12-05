using System.Collections;
using UnityEngine;

[System.Serializable]
public class PlayerInputData
{
    public Vector2 MousePosition;
    public bool IsMouseClicked;
    public bool IsMouseReleased;
    public bool IsMovingUp;
    public bool IsMovingDown;
    public bool IsMovingLeft;
    public bool IsMovingRight;
    public bool IsInteracting;
    public bool IsInteractionReleased;
}
