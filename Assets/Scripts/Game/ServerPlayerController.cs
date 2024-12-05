using Mirror;
using UnityEngine;

public class ServerPlayerController : NetworkBehaviour
{
    public float speed = 50.0f;
    public float jumpForce = 8.0f;
    private Rigidbody rb;
    private bool isGrounded;
    private Animator animator;

    public float interactionRange = 50.0f;
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

        HandleMovement();
        HandleJump();

        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleInteraction();
        }
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * moveHorizontal + transform.forward * moveVertical;

        rb.MovePosition(transform.position + moveDirection.normalized * speed * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void HandleInteraction()
    {
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
                return;
            }
        }

        Debug.Log($"[ServerPlayerController] Player {netId} found no valid RegisterableDevice nearby.");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
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
