using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoolDownUI : MonoBehaviour
{
    public static CoolDownUI Instance; // 싱글톤 인스턴스

    [SerializeField] private Image cooldownImage;
    private bool isCooldownActive = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // 인스턴스 할당
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        cooldownImage.fillAmount = 0f;
    }

    public void StartCooldown(float duration)
    {
        if (isCooldownActive) return; // 이미 쿨다운 중이면 무시
        StartCoroutine(CooldownRoutine(duration));
    }

    private IEnumerator CooldownRoutine(float duration)
    {
        isCooldownActive = true;

        float timePassed = 0f;
        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            cooldownImage.fillAmount = 1f - (timePassed / duration);
            yield return null;
        }

        cooldownImage.fillAmount = 0f;
        isCooldownActive = false;
    }
}
