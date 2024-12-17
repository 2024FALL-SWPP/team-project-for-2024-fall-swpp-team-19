using UnityEngine;
using Mirror;
using System.Collections;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar] private bool isDead = false;
    [SerializeField] private Animator animator;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera spectatorCamera;

    public void Kill()
    {
        if (isDead) return;

        isDead = true;

        // Play dying animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Wait for the animation to finish before disabling
        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        GetComponent<MusicController>().PlayDeathSound();
        yield return new WaitForSeconds(2.0f); // Adjust based on animation duration

        // Disable the character
        gameObject.SetActive(false);

        // Switch cameras
        if (isLocalPlayer)
        {
            if (playerCamera != null)
            {
                playerCamera.enabled = false;
            }

            if (spectatorCamera != null)
            {
                spectatorCamera.enabled = true;
            }
        }

        // Notify TargetManager to check remaining players
        if (isServer)
        {
            FindObjectOfType<TargetManager>().CheckRemainingPlayers();
        }
    }

    public bool IsDead()
    {
        return isDead;
    }
}
