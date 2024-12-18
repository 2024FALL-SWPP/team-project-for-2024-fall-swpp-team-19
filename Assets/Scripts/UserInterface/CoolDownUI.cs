using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoolDownUI : MonoBehaviour
{
    [SerializeField] private Image cooldownImage; // UI Image for cooldown
    private bool isCoolingDown = false;

    void Start()
    {
        cooldownImage.fillAmount = 0f;
        // StartCooldown(30f);
    }

   
    public void StartCooldown(float duration)
    {
        if (!isCoolingDown)
        {
            StartCoroutine(CooldownCoroutine(duration));
        }
    }

    private IEnumerator CooldownCoroutine(float duration)
    {
        isCoolingDown = true;
        float elapsed = 0f;

        cooldownImage.fillAmount = 1f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cooldownImage.fillAmount = 1f - (elapsed / duration); // Update UI
            yield return null;
        }

        cooldownImage.fillAmount = 0f;
        isCoolingDown = false;
    }
}