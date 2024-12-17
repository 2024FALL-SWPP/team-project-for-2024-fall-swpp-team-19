using Mirror;
using UnityEngine;
using System.Collections;

public class ServerPlayerController : NetworkBehaviour
{
    public float speed = 30.0f;
    public float jumpForce = 15.0f;
    private Rigidbody rb;
    private bool isGrounded;
    private bool isInteracting = false;
    private Animator animator;

    public float interactionRange = 15.0f;
    private CustomGamePlayer customPlayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        customPlayer = GetComponent<CustomGamePlayer>();

        if (!isLocalPlayer)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        // Prevent movement if in a mini-game
        if (customPlayer != null && customPlayer.isInMiniGame)
        {
            Debug.Log($"[ServerPlayerController] Player {netId} is in a mini-game. Movement disabled.");
            return;
        }

        CheckGrounded();
        HandleMovement();
        HandleJump();

        if (Input.GetKeyDown(KeyCode.R))
        {
            HandleInteraction();
        }
    }
    private void HandleMovement()
    {
        if (isInteracting) return;
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * moveHorizontal + transform.forward * moveVertical;

        Vector3 velocity = moveDirection.normalized * speed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);

    bool isMoving = moveDirection.magnitude > 0.1f;

        // Update animation only when the state changes
        if (animator.GetBool("IsRunning") != isMoving)
        {
            animator.SetBool("IsRunning", isMoving);
        }
    }
    
    private void CheckGrounded()
    {
        // Cast a ray slightly below the character to check if near the ground
        float rayLength = 1.2f; // Adjust based on the character's size
        isGrounded = Physics.Raycast(transform.position, Vector3.down, rayLength);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space)&& isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void HandleInteraction()
    {
        if (isInteracting) return;

        isInteracting = true;
        animator.SetTrigger("Interaction");

        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange);
        Debug.Log($"[ServerPlayerController] Player {netId} attempting interaction. {colliders.Length} objects in range.");

        foreach (var collider in colliders)
        {
            Debug.Log($"[ServerPlayerController] Player {netId} detected object: {collider.gameObject.name}");

            RegisterableDevice device = collider.GetComponent<RegisterableDevice>();
            if (device != null)
            {
                Debug.Log($"[ServerPlayerController] Player {netId} found a RegisterableDevice: {device.gameObject.name}. Sending interaction request.");
                CmdTryInteractWithDevice(device.gameObject);

                StartCoroutine(ResetInteractionAfterDelay(2.0f));
                return;
            }
        }

        Debug.Log($"[ServerPlayerController] Player {netId} found no valid RegisterableDevice nearby.");
        StartCoroutine(ResetInteractionAfterDelay(2.0f));
    }

    private IEnumerator ResetInteractionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isInteracting = false;
    }

    [Command]
    private void CmdTryInteractWithDevice(GameObject deviceObject)
    {
        RegisterableDevice device = deviceObject.GetComponent<RegisterableDevice>();
        if (device != null)
        {
            bool success = device.RegisterPlayer(customPlayer);
            if (success)
            {
                Debug.Log($"[ServerPlayerController] Player {netId} successfully connected to the mini-game.");
            }
            else
            {
                Debug.Log($"[ServerPlayerController] Player {netId} failed to connect to the mini-game. Check logs in MiniGameBase for details.");
            }
        }
        else
        {
            Debug.Log($"[ServerPlayerController] No valid RegisterableDevice found for Player {netId}.");
        }
    }
}
