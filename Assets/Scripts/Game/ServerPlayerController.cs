using Mirror;
using UnityEngine;
using System.Collections;

public class ServerPlayerController : NetworkBehaviour
{
    // Movement-related variables
    public float speed = 30.0f;
    public float jumpForce = 15.0f;
    private Rigidbody rb;
    private Animator animator;
    public LayerMask groundLayer = ~0;
    public float groundCheckDistance = 1.0f;
    private float lastGroundedTime;
    private float groundGracePeriod = 0.2f;

    // Interaction-related variables
    public float interactionRange = 15.0f;
    private bool isInteracting = false;
    private CustomGamePlayer customPlayer;

    // Combat-related variables
    [SerializeField] private float attackRange = 200.0f;
    [SerializeField] public LayerMask playerLayer;
    [SyncVar] private bool isPenalized = false;

    // Health-related variables
    [SyncVar] public bool isDead = false;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera spectatorCamera;

    private void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError($"[ServerPlayerController] Rigidbody not found in {gameObject.name} or its children.");
        }
        animator = GetComponent<Animator>();
        customPlayer = GetComponent<CustomGamePlayer>();

        if(NetworkClient.localPlayer.GetComponent<CustomGamePlayer>()!=customPlayer){
            enabled = false;
        }

        if (!isLocalPlayer)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (!isLocalPlayer || isDead || isPenalized) return;

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
            CmdAttemptKill();
        }
    }

    // Movement logic
    private void HandleMovement()
    {
        if (isInteracting) return;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * moveHorizontal + transform.forward * moveVertical;
        Vector3 velocity = moveDirection.normalized * speed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);

        bool isMoving = moveDirection.magnitude > 0.1f;

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
            GetComponent<MusicController>().PlayJumpSound();
        }
    }

    //Attack Logic
    private void HandleAttack()
    {
        animator.SetTrigger("Interaction");
    }

    // Interaction logic
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

        [Command]
    void CmdAttemptKill()
    {
        // Verify the caller's identity using connectionToClient
        GameObject callerPlayer = connectionToClient.identity.gameObject;
        CustomGamePlayer owner = callerPlayer.GetComponent<CustomGamePlayer>();

        if (owner == null || isPenalized)
        {
            Debug.LogWarning($"[ServerPlayerController] Player {connectionToClient.connectionId} cannot attack. Penalized or invalid owner.");
            return;
        }

        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, attackRange, playerLayer);
        Debug.Log($"[ServerPlayerController] Player {netId} found {hitPlayers.Length} players in attack range.");

        foreach (var hitPlayer in hitPlayers)
        {
            CustomGamePlayer targetPlayer = hitPlayer.GetComponent<CustomGamePlayer>();

            if (targetPlayer == null)
            {
                Debug.LogError("[ServerPlayerController] Target player is missing CustomGamePlayer component.");
                continue;
            }

            // Prevent attacking self
            if (owner == targetPlayer)
            {
                Debug.LogWarning("[ServerPlayerController] Player cannot attack themselves.");
                continue;
            }

            // Verify target player
            PlayerData playerData = PlayerDataManager.Instance.GetPlayerData(owner.GetColor());

            if (targetPlayer.GetColor() == playerData.target)
            {
                Debug.Log($"[ServerPlayerController] Player {netId} successfully killed target {targetPlayer.netId}.");
                hitPlayer.GetComponent<ServerPlayerController>().Kill();
                PlayerData targetPlayerData = PlayerDataManager.Instance.GetPlayerData(targetPlayer.GetColor());
                playerData.UpdateField("target", targetPlayerData.target);
            }
            else
            {
                Debug.LogWarning($"[ServerPlayerController] Player {netId} attacked wrong target. Applying penalty.");
                RpcApplyPenalty();
            }
        }
        GetComponent<MusicController>().PlayAttackSound();
    }

    [ClientRpc]
    void RpcApplyPenalty()
    {
        StartCoroutine(ApplyPenalty());
    }

    private IEnumerator ApplyPenalty()
    {
        isPenalized = true;
        yield return new WaitForSeconds(30f); // 30s penalty
        isPenalized = false;
    }

    public void Kill()
    {
        if (isDead) return;
        isDead = true;
        
        GameObject callerPlayer = connectionToClient.identity.gameObject;
        CustomGamePlayer owner = callerPlayer.GetComponent<CustomGamePlayer>();
        PlayerData playerData = PlayerDataManager.Instance.GetPlayerData(owner.GetColor());
        playerData.UpdateField("isAlive", false);

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(2.0f);

        gameObject.SetActive(false);

        // if (isLocalPlayer)
        // {
        //     if (playerCamera != null)
        //     {
        //         playerCamera.enabled = false;
        //     }

        //     if (spectatorCamera != null)
        //     {
        //         spectatorCamera.enabled = true;
        //     }
        // }

        // if (isServer)
        // {
        //     FindObjectOfType<TargetManager>().CheckRemainingPlayers();
        // }
    }

    // Ground detection
    private void CheckGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            lastGroundedTime = Time.time;
        }
    }

    private bool IsGroundedWithGracePeriod()
    {
        return (Time.time - lastGroundedTime) <= groundGracePeriod;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            lastGroundedTime = Time.time;
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
