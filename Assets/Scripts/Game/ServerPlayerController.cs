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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (!isLocalPlayer)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

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

        foreach (var collider in colliders)
        {
            RegisterableDevice device = collider.GetComponent<RegisterableDevice>();
            if (device != null)
            {
                CmdTryInteractWithDevice(device.gameObject);
                return;
            }
        }

        Debug.Log("No interactable device found nearby.");
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
            bool success = device.RegisterPlayer(GetComponent<CustomGamePlayer>());
            if (success)
            {
                Debug.Log($"Player {netId} successfully connected to the mini-game.");
            }
            else
            {
                Debug.Log($"Player {netId} failed to connect to the mini-game.");
            }
        }
        else
        {
            Debug.Log($"No valid RegisterableDevice found for interaction by Player {netId}.");
        }
    }
}
