using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KJHSoundManager : MonoBehaviour
{
    public AudioClip[] characterSounds; // 각 스프라이트에 대한 사운드 배열

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // 스프라이트에 대한 사운드를 재생하는 함수
    public void PlayCharacterSound(int characterIndex)
    {
        if (characterIndex >= 0 && characterIndex < characterSounds.Length)
        {
            audioSource.clip = characterSounds[characterIndex];
            audioSource.Play();
        }
    }
}
