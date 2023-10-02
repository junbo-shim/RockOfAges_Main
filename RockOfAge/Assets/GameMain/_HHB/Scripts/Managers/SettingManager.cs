using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum UserScreen 
{ 
    FULLSCREEN = 0, WINDOWMODE = 1/*, Screen16To10 = 2, Screen16To9 = 3*/
}

//public enum PlayTime
//{
//    BATTLEFIELD5 = 83, FIGHT2 = 43, FIGHT = 77, HAVOC = 107, STRATEGY6 = 89, VALIANT = 173, EPICBATTLE_DELITY = 117
//}

public class SettingManager : MonoBehaviour
{
    public static SettingManager settingManager;

    #region 변수 

    #region BUTTONS
    [Header("BUTTONS")]
    public Button escButton;
    public Button quitGameButton;
    public Button lobbyButton;
    public Button musicLeftButton;
    public Button musicRightButton;
    #endregion

    #region RESOLUTION
    // 해상도
    [Header("RESOLUTION")]
    public TMP_Dropdown dropDown;
    private int setWidth;
    private int setHeight;
    private int userResoultionValue;
    private bool isWindowed;
    #endregion

    #region SOUND
    // BGM
    [Header("SOUND_BGM")]
    public Slider bgmSlider;
    public Button bgmButton;
    public GameObject muteBGMSlider;
    public AudioMixer bgmMixer;
    private bool bgmMuted = false;

    [Header("SOUND_VFX")]
    //VFX
    public Slider vfxSlider;
    public Button vfxButton;
    public GameObject muteVFXSlider;
    public AudioMixer vfxMixer;
    private bool vfxMuted = false;

    [Header("SOUND_MASTER")]
    //Master
    public Slider masterSlider;
    public Button masterButton;
    public GameObject muteMasterSlider;
    public AudioMixer masterMixer;
    private bool masterMuted = false;

    [Header("SOUND_COMMON")]
    //공용
    public Sprite soundDisabled;
    public Sprite soundAbled;
    // 0번 마스터, 1번 bgm, 2번 vfx
    // 이전 사운드 볼륨 캐싱
    public float[] previewSoundFloat;


    #endregion

    #region LEGACY
    //public List<GameObject> uis = new List<GameObject>();
    //public List<Transform> uiHolders = new List<Transform>();
    #endregion 

    #endregion



    private void Awake()
    {

        settingManager = this;
        SetDropDown();
        previewSoundFloat = new float[3] { 1f, 1f, 1f };
        PackStart();

    }

    private void PackStart()
    {
        escButton.onClick.AddListener(() => OnClickEscButton());
        quitGameButton.onClick.AddListener(() => OnClickQuitGameButton());
        lobbyButton.onClick.AddListener(() => OnClickLobbyButton());

        dropDown.onValueChanged.AddListener(delegate { OnDropDownValueChange(); });

        masterSlider.onValueChanged.AddListener(delegate { MasterAudioSlider(); });
        masterButton.onClick.AddListener(() => MasterAudioButton());

        bgmSlider.onValueChanged.AddListener(delegate { BGMAudioSlider(); });
        bgmButton.onClick.AddListener(() => BGMAudioButton());


        vfxSlider.onValueChanged.AddListener(delegate { VFXAudioSlider(); });
        vfxButton.onClick.AddListener(() => VFXAudioButton());

        musicLeftButton.onClick.AddListener(()=>OnLeftMusicButton());
        musicRightButton.onClick.AddListener(()=>OnRightMusicButton());
    }

    #region RESOLUTION
    public void SwitchFullAndWindowMode()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            ToggleResolution();
        }
    }



    private void ToggleResolution()
    {
        switch (userResoultionValue)
        {
            case (int)UserScreen.FULLSCREEN:
                userResoultionValue = (int)UserScreen.WINDOWMODE;
                break;
            case (int)UserScreen.WINDOWMODE:
                userResoultionValue = (int)UserScreen.FULLSCREEN;
                break;
        }
        dropDown.value = userResoultionValue;
        OnDropDownValueChange();
    }


    private void SetDropDown()
    {
        #region LEGACY
        //AddUIToChangerResolution();
        //AddHolderToChangerResolution();
        //dropDown.options.Add(new TMP_Dropdown.OptionData("1920X1200"));
        //dropDown.options.Add(new TMP_Dropdown.OptionData("1920X1080"));
        #endregion

        isWindowed = false;
        setWidth = 1920;
        setHeight = 1080;
        dropDown.options.Add(new TMP_Dropdown.OptionData("FullScreen"));
        dropDown.options.Add(new TMP_Dropdown.OptionData("WindowedMode"));
        dropDown.value = (int)UserScreen.FULLSCREEN;
        dropDown.RefreshShownValue();
    }

    #region LEGACY

    //private void AddUIToChangerResolution()
    //{
    //    uis.Add(Global_PSC.FindTopLevelGameObject("UserSelectUI"));
    //    uis.Add(Global_PSC.FindTopLevelGameObject("CommonUI"));
    //    uis.Add(Global_PSC.FindTopLevelGameObject("RockSelectUI"));
    //    uis.Add(Global_PSC.FindTopLevelGameObject("AttackUI"));
    //    uis.Add(Global_PSC.FindTopLevelGameObject("DefenceUI"));
    //    uis.Add(Global_PSC.FindTopLevelGameObject("GameEndUI"));
    //    uis.Add(Global_PSC.FindTopLevelGameObject("EscUI"));
    //}

    //private void AddHolderToChangerResolution()
    //{
    //    GameObject commonUI = Global_PSC.FindTopLevelGameObject("CommonUI");
    //    Transform commonHolder = commonUI.transform.GetChild(0);
    //    uiHolders.Add(commonHolder);

    //    GameObject selectUI = Global_PSC.FindTopLevelGameObject("RockSelectUI");
    //    Transform selectHolder = selectUI.transform.GetChild(0);
    //    uiHolders.Add(selectHolder);

    //    GameObject attackUI = Global_PSC.FindTopLevelGameObject("AttackUI");
    //    Transform attackHolder = attackUI.transform.GetChild(0);
    //    uiHolders.Add(attackHolder);

    //    GameObject defenceUI = Global_PSC.FindTopLevelGameObject("DefenceUI");
    //    Transform defenceHolder = defenceUI.transform.GetChild(0);
    //    uiHolders.Add(defenceHolder);

    //    GameObject gameEndUI = Global_PSC.FindTopLevelGameObject("GameEndUI");
    //    Transform gameEndHolder = gameEndUI.transform.GetChild(0);
    //    uiHolders.Add(gameEndHolder);

    //    GameObject escUI = Global_PSC.FindTopLevelGameObject("EscUI");
    //    Transform escHolder = escUI.transform.GetChild(0);
    //    uiHolders.Add(escHolder);
    //}

    //public void SetScreen16To10()
    //{
    //    setWidth = 1920; setHeight = 1200;
    //    Screen.SetResolution(setWidth, setHeight, isWindowed);

    //    foreach (GameObject ui in uis)
    //    { 
    //        CanvasScaler uiCanvas = ui.GetComponent<CanvasScaler>();
    //        uiCanvas.referenceResolution = new Vector2(setWidth, setHeight);
    //    }

    //    foreach (Transform holder in uiHolders)
    //    { 
    //        RectTransform rectTransform = holder.GetComponent<RectTransform>();
    //        rectTransform.sizeDelta = new Vector2(setWidth, setHeight); 
    //    }
    //}

    //public void SetScreen16To9()
    //{
    //    setWidth = 1920; setHeight = 1080;
    //    Screen.SetResolution(setWidth, setHeight, isWindowed);

    //    foreach (GameObject ui in uis)
    //    {
    //        CanvasScaler uiCanvas = ui.GetComponent<CanvasScaler>();
    //        uiCanvas.referenceResolution = new Vector2(setWidth, setHeight);
    //    }
    //    foreach (Transform holder in uiHolders)
    //    {
    //        RectTransform rectTransform = holder.GetComponent<RectTransform>();
    //        rectTransform.sizeDelta = new Vector2(setWidth, setHeight);
    //    }
    //}
    #endregion


    public void OnClickEscButton()
    {
        CycleManager.cycleManager.UpdateESCCycle();
        escButton.interactable = false;
        escButton.interactable = true;
    }



    public void OnDropDownValueChange()
    {
        userResoultionValue = dropDown.value;
        if (userResoultionValue == (int)UserScreen.FULLSCREEN)
        {
            SetFullScreen();
        }
        else if (userResoultionValue == (int)UserScreen.WINDOWMODE)
        {
            SetWindowedMode();  
        }
    }

    public void SetFullScreen()
    {
        isWindowed = true;
        Screen.SetResolution(setWidth, setHeight, isWindowed);
    }

    public void SetWindowedMode()
    {
        isWindowed = false;
        Screen.SetResolution(setWidth, setHeight, isWindowed);
    }
    #endregion

    #region PHOTON BUTTONS
    public void OnClickQuitGameButton()
    {
        // 게임 끄기
    }

    public void OnClickLobbyButton()
    {
        // 게임 로비
    }
    #endregion

    #region SOUND

    #region MASTER
    public void MasterAudioSlider()
    { 
        float sound = masterSlider.value;

        if (sound == -40f)
        {
            masterMixer.SetFloat("Master", -80f);
            masterButton.image.sprite = soundDisabled;
        }
        else 
        {
            masterMixer.SetFloat("Master", Mathf.Log10(sound) * 20);
            previewSoundFloat[0] = sound;
            masterButton.image.sprite = soundAbled;

        }
    }

    public void MasterAudioButton()
    {
        if (masterMuted == false)
        {
            masterButton.image.sprite = soundDisabled;
            masterMixer.SetFloat("Master", -80f);
            masterSlider.gameObject.SetActive(false);
            muteMasterSlider.SetActive(true);
            VFXAudioButton();
            BGMAudioButton();
        }
        else 
        {
            muteMasterSlider.SetActive(false);
            masterSlider.gameObject.SetActive(true);
            masterButton.image.sprite = soundAbled;
            masterMixer.SetFloat("Master", Mathf.Log10(previewSoundFloat[0])*20);
            masterSlider.value = previewSoundFloat[0];
            VFXAudioButton();
            BGMAudioButton();
        }
        masterMuted = !masterMuted;
    }
    #endregion

    #region VFX
    public void VFXAudioSlider()
    {
        float sound = vfxSlider.value;

        if (sound == -40f)
        {
            vfxMixer.SetFloat("VFX", -80f);
            vfxButton.image.sprite = soundDisabled;
        }
        else
        {
            vfxMixer.SetFloat("VFX", Mathf.Log10(sound) * 20);
            previewSoundFloat[1] = sound;
            vfxButton.image.sprite = soundAbled;
        }
    }

    public void VFXAudioButton()
    {
        if (vfxMuted == false)
        {
            vfxButton.image.sprite = soundDisabled;
            vfxMixer.SetFloat("VFX", -80f);
            vfxSlider.gameObject.SetActive(false);
            muteVFXSlider.SetActive(true);
        }
        else
        {
            muteVFXSlider.SetActive(false);
            vfxSlider.gameObject.SetActive(true);
            vfxButton.image.sprite = soundAbled;
            vfxMixer.SetFloat("VFX", Mathf.Log10(previewSoundFloat[1]) * 20);
            vfxSlider.value = previewSoundFloat[1];
        }
        vfxMuted = !vfxMuted;
    }
    #endregion

    #region BGM
    public void BGMAudioSlider()
    {
        float sound = bgmSlider.value;

        if (sound == -40f)
        {
            bgmMixer.SetFloat("BGM", -80f);
            bgmButton.image.sprite = soundDisabled;
        }
        else
        {
            bgmMixer.SetFloat("BGM", Mathf.Log10(sound) * 20);
            previewSoundFloat[2] = sound;
            bgmButton.image.sprite = soundAbled;
        }
    }

    public void BGMAudioButton()
    {
        if (bgmMuted == false)
        {
            bgmButton.image.sprite = soundDisabled;
            bgmMixer.SetFloat("BGM", -80f);
            bgmSlider.gameObject.SetActive(false);
            muteBGMSlider.SetActive(true);
        }
        else
        {
            muteBGMSlider.SetActive(false);
            bgmSlider.gameObject.SetActive(true);
            bgmButton.image.sprite = soundAbled;
            bgmMixer.SetFloat("BGM", Mathf.Log10(previewSoundFloat[2]) * 20);
            bgmSlider.value = previewSoundFloat[2];
        }
        bgmMuted = !bgmMuted;
    }
    #endregion

    #region CUSTOM


    public void OnRightMusicButton()
    {
        int index = SoundManager.soundManager.currentBGMIndex;
        SoundManager.soundManager.StopMusic();
        if (index < (int)BGMSound.LENGHT)
        {
            SoundManager.soundManager.currentBGMIndex++;
        }
        else { SoundManager.soundManager.currentBGMIndex = 0; }
        SoundManager.soundManager.PlayBGMMusic((BGMSound)SoundManager.soundManager.currentBGMIndex);
    }

    public void OnLeftMusicButton()
    {
        int index = SoundManager.soundManager.currentBGMIndex;
        if (index-- == -1)
        {
            SoundManager.soundManager.currentBGMIndex = (int)BGMSound.EPICBATTLE;
        }
        SoundManager.soundManager.StopMusic();
        SoundManager.soundManager.currentBGMIndex--;
        SoundManager.soundManager.PlayBGMMusic((BGMSound)SoundManager.soundManager.currentBGMIndex);
    }


    #endregion

    #endregion

}


