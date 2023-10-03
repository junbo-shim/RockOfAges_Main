using System.Collections;
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

    public Queue<GameObject> soundQueue = new Queue<GameObject>();
    public GameObject soundBox;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();  
        soundManager = this;
        IniInitializeSoundManager();
    }

    public void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayNextTrack();
        }

    }

    #region SOUNDPOOLING
    private void IniInitializeSoundManager()
    { 
        for (int i = 0; i < 20; i++)
        {
            soundQueue.Enqueue(CreatSoundPooling());
        }
    }

    private GameObject CreatSoundPooling()
    {
        var soundObj = Instantiate(soundBox);
        soundObj.name = "SoundBox";
        SetInitTransform(soundObj);
        soundObj.SetActive(false);
        return soundObj;      
    }

    public void SetInitTransform(GameObject soundObj)
    {
        GameObject mother = Global_PSC.FindTopLevelGameObject("PoolingSoundObjectQueue");
        soundObj.transform.SetParent(mother.transform);
    }

    private void SetObjectTransfrom(GameObject needSoundObj, GameObject soundBox)
    {
        //float scaleX = needSoundObj.transform.localScale.x;
        //float scaleY = needSoundObj.transform.localScale.y;
        //float scaleZ = needSoundObj.transform.localScale.z;

        //soundBox.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

        soundBox.transform.position = needSoundObj.transform.position;
    }

    // 사운드 매니저는 풀링만 해주고 audioClip을 가질 수 있는 껍데기를 만들고
    // 새로운 스크립트를 만들어서 거기서 audioClip과 실행, 리턴을 만들고 (Prefab)
    // 부서지는 게임오브젝트랑 별개로 놀아야함.
    // awake에서 clip을 캐싱하고 자기 자신 init
    // 사운드가 disable 상태
    // onable로
    // clip을 environment가 들고 있고


    public void GetSoundPooling(GameObject needSoundObj, AudioClip audioClip)
    {
        if (soundQueue.Count > 0)
        {
            var soundBox = soundQueue.Dequeue();
 
            SetObjectTransfrom(needSoundObj, soundBox);
            AudioSource audioSource = soundBox.GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            soundBox.gameObject.SetActive(true);

        }
        else 
        { 
            var newSoundBox = CreatSoundPooling();
            SetObjectTransfrom(needSoundObj, newSoundBox);
            AudioSource audioSource = newSoundBox.GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            newSoundBox.gameObject.SetActive(true);
        }
    }




    #endregion


    #region BGM
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
    #endregion 

    //public void PauseMusic()
    //{
    //    audioSource.Pause();
    //}

    //public void UnPauseMusic()
    //{
    //    audioSource.UnPause();
    //}
}
