using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UserScreen 
{ 
    FullScreen = 0, WindowedMode = 1
}

public class SettingManager : MonoBehaviour
{
    public static SettingManager settingManager;
    //
    public Button escButton;
    public Button quitGameButton;
    public Button lobbyButton;
    public Button musicLeftButton;
    public Button musicRightButton;

    public TMP_Dropdown dropDown;

    private int setWidth;
    private int setHeight;
    private int userResoultionValue;
    private void Awake()
    {
        settingManager = this;
        SetDropDown();
        setWidth = 1920;
        setHeight = 1080;

    }


    #region LEGAFCY
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.Return))
        //{
        //    switch (userResoultionValue)
        //    {
        //        case (int)UserScreen.FullScreen:
        //            dropDown.onValueChanged.AddListener(delegate { OnDropDownValueChange(); });
        //            break;
        //        case (int)UserScreen.WindowedMode:
        //            dropDown.onValueChanged.AddListener(delegate { OnDropDownValueChange(); });
        //            break;
        //    }
        //}
    }
    #endregion


    private void SetDropDown()
    {
        dropDown.options.Add(new TMP_Dropdown.OptionData("FullScreen"));
        dropDown.options.Add(new TMP_Dropdown.OptionData("WindowedMode"));
        dropDown.value = (int)UserScreen.FullScreen;
        dropDown.RefreshShownValue();
    }


    private void Start()
    {
        escButton.onClick.AddListener(() => OnClickEscButton());
        quitGameButton.onClick.AddListener(() => OnClickQuitGameButton());
        lobbyButton.onClick.AddListener(() => OnClickLobbyButton());
        dropDown.onValueChanged.AddListener(delegate { OnDropDownValueChange(); });
    }

    public void OnClickEscButton()
    {
        CycleManager.cycleManager.UpdateESCCycle();
        escButton.interactable = false;
        escButton.interactable = true;
    }

    public void OnClickQuitGameButton()
    { 
        // 게임 끄기
    }

    public void OnClickLobbyButton()
    { 
        // 게임 로비
    }

    public void OnDropDownValueChange()
    {
        userResoultionValue = dropDown.value;
        if (userResoultionValue == (int)UserScreen.FullScreen)
        {
            SetFullScreen();
        }
        else if (userResoultionValue == (int)UserScreen.WindowedMode)
        {
            SetWindowedMode();  
        }
    }

    public void SetFullScreen()
    {
        Screen.SetResolution(setWidth, setHeight, true);
    }

    public void SetWindowedMode()
    {
        Screen.SetResolution(setWidth, setHeight, false);
    }
}
