using UnityEngine;

public enum BGMSound
{ 
    BATTLEFIELD5 = 0, FIGHT2 = 1, FIGHT = 2, HAVOC = 3, STRATEGY6 = 4, VALIANT = 5, EPICBATTLE_DELITY = 6 
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager soundManager;

    private AudioSource audioSource;
    public AudioClip[] bgm;
    //private int userBgmIndex;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();  
        soundManager = this;
    }

    public void BGMCycle()
    {
        if (CycleManager.cycleManager.userState == (int)UserState.UNITSELECT)
        {
            PlayMusic((int)BGMSound.FIGHT);
        }
        else if (CycleManager.cycleManager.userState == (int)UserState.DEFENCE)
        {
            PlayMusic((int)BGMSound.STRATEGY6);
        }
        else if (CycleManager.cycleManager.userState == (int)UserState.ATTACK)
        {
            PlayMusic((int)BGMSound.HAVOC);
        }
        else if (CycleManager.cycleManager.userState == (int)UserState.ENDING)
        {
            PlayMusic((int)BGMSound.VALIANT);
        }
    }

    public void PlayMusic(int playList)
    {
        audioSource.clip = bgm[playList];
        audioSource.Play();
    }

    public void StopMusic()
    { 
        audioSource.Stop();
    }

    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void UnPauseMusic()
    {
        audioSource.UnPause();
    }
}
