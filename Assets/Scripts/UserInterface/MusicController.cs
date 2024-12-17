using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour
{

    [Header("Audio Mixer Groups")]
    public AudioMixerGroup sfxGroup; // Assign this to the SFX group in your AudioMixer

    [Header("Audio Clips")]
    public AudioClip jumpClip;
    public AudioClip attackClip;
    public AudioClip deathClip;
    public AudioClip teleportClip;
    public AudioClip successClip;
    public AudioClip failureClip;

    private AudioSource sfxSource; // Cached AudioSource for SFX playback
    
    private float attackCooldown = 1.8f; // 공격 쿨타임 (초)
    private bool canAttack = true; // 공격 가능 여부
    void Awake()
    {
        // Initialize AudioSource
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxGroup;
        sfxSource.playOnAwake = false;
    }

    // Play a specific effect
    private void PlaySoundEffect(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.clip = clip;
            sfxSource.Play();
        }
        else
        {
            Debug.LogWarning("No AudioClip assigned for this action!");
        }
    }

    // Methods to trigger specific sounds
    public void PlayJumpSound()
    {
        PlaySoundEffect(jumpClip);
    }

    public void PlayAttackSound()
    {
        if (!canAttack) return; // 공격 불가능 상태라면 실행하지 않음
        StartCoroutine(PlayAttackSoundDelay());
    }
    public void PlayDeathSound()
    {
        PlaySoundEffect(deathClip);
    }

    public void PlayTeleportSound()
    {
        PlaySoundEffect(teleportClip);
    }

    public void PlaySuccessSound()
    {
        PlaySoundEffect(successClip);
    }

    public void PlayFailureSound()
    {
        PlaySoundEffect(failureClip);
    }

    private IEnumerator PlayAttackSoundDelay()
    {
        canAttack = false; // 공격 중
        yield return new WaitForSeconds(0.5f); // 공격 사운드 딜레이
        PlaySoundEffect(attackClip); // 사운드 재생
        yield return new WaitForSeconds(attackCooldown - 0.5f); // 남은 쿨타임 대기
        canAttack = true; // 공격 가능
    }
}
