using Mirror; // Ensure you import Mirror namespace for multiplayer functionality
using UnityEngine;

public class ServerPlayerController : NetworkBehaviour
{
    public float speed = 50.0f;          // Speed of movement
    public float jumpForce = 8.0f;      // Jump force
    private Rigidbody rb;
    private bool isGrounded;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Disable this script for non-local players
        if (!isLocalPlayer)
        {
            enabled = false;
        }
    }

    void Update()
    {
        // Ensure only the local player processes input
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
        // Get input for movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calculate movement relative to the character's current facing direction
        Vector3 moveDirection = transform.right * moveHorizontal + transform.forward * moveVertical;

        // Normalize movement direction to prevent faster diagonal movement and move the character
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
        animator.SetTrigger("Interaction"); // Trigger the wielding animation
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
