using UnityEngine;
using Mirror;
using System.Collections;

public class PlayerCombat : NetworkBehaviour
{
    [SerializeField] private float attackRange = 2.0f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Animator animator;

    [SerializeField] private CoolDownUI cooldownUI; // Reference to CooldownUI script
    [SerializeField] private float attackCooldownTime = 30.0f; // Cooldown duration

    private GameObject target;
    private bool isPenalized = false;

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (!isLocalPlayer || isPenalized) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            CmdAttemptKill();
        }
    }

    [Command]
    void CmdAttemptKill()
    {
        if (target == null) return;

        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, attackRange, playerLayer);

        foreach (var hitPlayer in hitPlayers)
        {
            if (hitPlayer.gameObject == target)
            {
                // Correct target
                hitPlayer.GetComponent<PlayerHealth>().Kill();
                return;
            }
            else
            {
                // Incorrect target
                RpcApplyPenalty();
            }
        }
    }

    [ClientRpc]
    void RpcApplyPenalty()
    {
        StartCoroutine(ApplyPenalty());
        cooldownUI?.StartCooldown(attackCooldownTime);
    }

    private IEnumerator ApplyPenalty()
    {
        isPenalized = true;
        yield return new WaitForSeconds(30f); // 30 seconds penalty
        isPenalized = false;
    }
}
