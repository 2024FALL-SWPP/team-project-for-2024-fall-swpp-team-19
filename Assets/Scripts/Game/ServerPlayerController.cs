using Mirror;
using UnityEngine;
using System.Collections;

public class ServerPlayerController : NetworkBehaviour
{
    public float speed = 30.0f;
    public float jumpForce = 15.0f;
    private Rigidbody rb;
    public bool isGrounded;
    private bool isInteracting = false;
    private Animator animator;

    public LayerMask groundLayer = ~0;
    public float groundCheckDistance = 1.0f; // Distance for raycast to check ground
    private float lastGroundedTime;
    private float groundGracePeriod = 0.2f; // Grace period after landing

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

        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleInteraction();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            HandleAttack();
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

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGroundedWithGracePeriod())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // Prevent further jumps until grounded again
        }
    }

    private void HandleInteraction()
    {
        if (isInteracting) return;

        isInteracting = true;

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

    private void HandleAttack()
    {
        animator.SetTrigger("Interaction");
    }

    private void CheckGrounded()
    {
        // Cast a ray downward to check if the player is near the ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            isGrounded = true;
            lastGroundedTime = Time.time; // Update the last grounded time
        }
        else
        {
            isGrounded = false;
        }
    }

    private bool IsGroundedWithGracePeriod()
    {
        // Allow jumping if grounded or within the grace period after landing
        return isGrounded || (Time.time - lastGroundedTime) <= groundGracePeriod;
    }

    private void OnCollisionStay(Collision collision)
    {
        // Additional collision check to reinforce grounded state
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
            lastGroundedTime = Time.time; // Update the last grounded time
        }
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
