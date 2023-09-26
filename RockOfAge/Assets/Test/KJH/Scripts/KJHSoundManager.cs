using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip attackSound;
    public AudioClip damageSound;
    public AudioClip idleSound;

    private AudioSource audioSource;

    // 사운드 매니저의 이름을 저장하는 변수
    public string soundManagerName;

    private void Awake()
    {
        // 각각의 사운드 매니저 인스턴스에 이름을 설정합니다.
        gameObject.name = soundManagerName;

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAttackSound()
    {
        if (attackSound != null)
        {
            audioSource.clip = attackSound;
            audioSource.Play();
        }
    }

    public void PlayDamageSound()
    {
        if (damageSound != null)
        {
            audioSource.clip = damageSound;
            audioSource.Play();
        }
    }

    public void PlayIdleSound()
    {
        if (idleSound != null)
        {
            audioSource.clip = idleSound;
            audioSource.Play();
        }
    }
}