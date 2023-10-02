using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum BGMSound
{ 
   BATTLEFIELD5 = 0, FIGHT2 = 1, FIGHT = 2, HAVOC = 3, STRATEGY6 = 4, VALIANT = 5, EPICBATTLE = 6 , LENGHT = 7
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager soundManager;


    [Header("SOUND")]
    private AudioSource audioSource;
    public AudioClip[] bgm;
    public BGMSound[] playList;
    //private int userBgmIndex;
    [Header("CUSTOM SOUND")]
    public int currentBGMIndex = 0;
    public TextMeshProUGUI musicName;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();  
        soundManager = this;
    }

    public void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayNextTrack();
        }

    }

    public void BGMCycle()
    {
        if (CycleManager.cycleManager.userState == (int)UserState.UNITSELECT)
        {
            int randomNumber = Random.Range(0, 2);
            switch (randomNumber)
            {
                case 0:
                    PlayBGMMusic(BGMSound.FIGHT2);
                    break;
                case 1:
                    PlayBGMMusic(BGMSound.FIGHT);
                    break;
            }
        }
        else if (CycleManager.cycleManager.userState == (int)UserState.DEFENCE)
        {
            int randomNumber = Random.Range(0, 3);
            switch (randomNumber)
            {
                case 0:
                    PlayBGMMusic(BGMSound.FIGHT2);
                    break;
                case 1:
                    PlayBGMMusic(BGMSound.FIGHT);
                    break;
                case 2:
                    PlayBGMMusic(BGMSound.STRATEGY6);
                    break;
            }
        }
        else if (CycleManager.cycleManager.userState == (int)UserState.ATTACK)
        {
            int randomNumber = Random.Range(0, 3);
            switch (randomNumber)
            {
                case 0:
                    PlayBGMMusic(BGMSound.BATTLEFIELD5);
                    break;
                case 1:
                    PlayBGMMusic(BGMSound.HAVOC);
                    break;
                case 2:
                    PlayBGMMusic(BGMSound.EPICBATTLE);
                    break;
            }
        }
        else if (CycleManager.cycleManager.userState == (int)UserState.ENDING)
        {
            PlayBGMMusic(BGMSound.VALIANT);
        }
    }

    public void PlayNextTrack()
    {
        if (currentBGMIndex < bgm.Length)
        {
            PlayBGMMusic((BGMSound)currentBGMIndex);
            currentBGMIndex++;

            // 마지막 곡이면 첫 번째 곡으로 되돌린다.
            if (currentBGMIndex == bgm.Length)
            {
                currentBGMIndex = 0;
            }
        }
    }


    public void PlayBGMMusic(BGMSound playList)
    {
        musicName.text = playList.ToString();
        audioSource.clip = bgm[(int)playList];
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    //public void PauseMusic()
    //{
    //    audioSource.Pause();
    //}

    //public void UnPauseMusic()
    //{
    //    audioSource.UnPause();
    //}
}
