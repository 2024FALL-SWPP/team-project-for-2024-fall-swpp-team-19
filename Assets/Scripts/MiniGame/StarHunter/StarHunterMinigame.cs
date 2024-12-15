using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Unity.VisualScripting;

public class StarHunterMinigame : MiniGameBase
{
    public RawImage duck;
    public Text countdownText;
    public bool duckIsGrounded = true;

    private Rigidbody2D duckRb;
    private float duckSpeed = 300f;
    private float duckJumpForce = 1400f;
    private bool canDuckMove = false;
    private bool canDuckJump = false;
    private int score = 0;

    [Server]
    public override void StartGame()
    {
        base.StartGame();
        Debug.Log("[StarHunterMinigame] Starting game.");
        duckRb = duck.GetComponent<Rigidbody2D>();
        StartCoroutine(CountdownAndStart());
    }

    [Server]
    private IEnumerator CountdownAndStart()
    {
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        countdownText.text = "";
        canDuckMove = true;
        canDuckJump = true;
    }

    [Server]
    public override void EndGame()
    {
        Debug.Log("[StarHunterMinigame] Ending game.");
        base.EndGame();
    }

    [Server]
    public override void UpdateGameLogic()
    {
        base.UpdateGameLogic();

        if (canDuckMove)
        {
            // Log how many players we're processing input for this frame
            Debug.Log($"[StarHunterMinigame] UpdateGameLogic: Processing input for {currentPlayers.Count} players.");

            foreach (var player in currentPlayers)
            {
                var input = player.InputData;
                
                // Log the input data received from each player
                Debug.Log($"[StarHunterMinigame] Player {player.netId} Input: " +
                        $"Left={input.IsMovingLeft}, Right={input.IsMovingRight}, Interact={input.IsInteracting}");

                HandleMovement(player, input);
                HandleJumping(player, input);
            }
        }
    }

    [Server]
    private void HandleMovement(CustomGamePlayer player, PlayerInputData input)
    {
        Debug.Log($"[StarHunterMinigame] Handling movement for Player {player.netId}. " +
                  $"Left={input.IsMovingLeft}, Right={input.IsMovingRight}");

        if (input.IsMovingLeft) duckRb.velocity = new Vector2(-duckSpeed, duckRb.velocity.y);
        if (input.IsMovingRight) duckRb.velocity = new Vector2(duckSpeed, duckRb.velocity.y);

        Debug.Log($"[StarHunterMinigame] Player {player.netId} moved duck with velocity {duckRb.velocity.x}.");
    }

    [Server]
    private void HandleJumping(CustomGamePlayer player, PlayerInputData input)
    {
        if (input.IsJumping && duckIsGrounded && canDuckJump)
        {
            Debug.Log($"[StarHunterMinigame] Handling interaction for Player {player.netId}. Interacting={input.IsInteracting}");

            duckRb.velocity = new Vector2(duckRb.velocity.x, duckJumpForce);
            duckIsGrounded = false;
        }
    }

    [Server]
    public void IncrementScore()
    {
        score++;
    }
}
