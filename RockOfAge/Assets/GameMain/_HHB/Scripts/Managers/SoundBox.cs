using UnityEngine;

public class SoundBox : MonoBehaviour
{
    private AudioSource audioSource;
    public void OnEnable()
    {
        if (audioSource.clip == null)
        {
            Invoke("ReturnSoundBox", 0);
            return;
        }
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        Invoke("ReturnSoundBox", audioSource.clip.length);
    }

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ReturnSoundBox()
    {
        SoundManager.soundManager.soundQueue.Enqueue(this.gameObject);
        SoundManager.soundManager.SetInitTransform(this.gameObject);
        gameObject.SetActive(false);
    }

}

