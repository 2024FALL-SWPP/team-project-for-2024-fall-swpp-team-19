using Mirror;
using UnityEngine;

public class ServerPlayerController : NetworkBehaviour
{
    public float speed = 50.0f;
    public float jumpForce = 8.0f;
    private Rigidbody rb;
    private bool isGrounded;
    private Animator animator;

    public float interactionRange = 5.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (!isLocalPlayer)
        {
            enabled = false;
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        MovePlayer();
        Jump();

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interaction();
        }
    }

    void MovePlayer()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * moveHorizontal + transform.forward * moveVertical;

        rb.MovePosition(transform.position + moveDirection.normalized * speed * Time.deltaTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void Interaction()
    {
        animator.SetTrigger("Interaction");

        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange);

        foreach (var collider in colliders)
        {
            MiniGameBase miniGame = collider.GetComponent<MiniGameBase>();
            if (miniGame != null)
            {
                CustomGamePlayer player = GetComponent<CustomGamePlayer>();
                if (player != null && miniGame.RegisterPlayer(connectionToClient, player))
                {
                    Debug.Log("Player registered to the mini-game.");
                }
                else
                {
                    Debug.Log("Mini-game is full or registration failed.");
                }
                return;
            }
        }

        Debug.Log("No mini-game found nearby.");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
