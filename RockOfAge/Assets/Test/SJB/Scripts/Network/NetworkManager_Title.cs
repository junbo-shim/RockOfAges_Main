using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.GroupsModels;

public partial class NetworkManager : GlobalSingleton<NetworkManager>
{
    //Title Login ===========================================================================================

    public Transform TitlePanel { get; private set; }
    public Transform LoginPopup { get; private set; }
    public Transform SignupPopup { get; private set; }

    protected override void Awake()
    {
        FindUIObjects();
        cachedRoomList = new List<RoomInfo>();
        displayRoomList = new List<GameObject>();
        playerRoomList = new Player[4];
        team1Entry = new int[2];
        team2Entry = new int[2];
    }

    #region 서버연결-Photon
    private void ConnectToServer() 
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
        //currentPlayersInLobby = PhotonNetwork.CountOfPlayersOnMaster;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.LogFormat("You are Disconnected : {0}", cause);
    }
    #endregion

    #region UI 오브젝트 찾아오는 메서드
    private void FindUIObjects() 
    {
        // Title
        TitlePanel = GameObject.Find("Panel_Title").transform;
        LoginPopup = TitlePanel.Find("Popup_Login").transform;
        SignupPopup = TitlePanel.Find("Popup_Signup").transform;
        // Lobby
        LobbyPanel = GameObject.Find("Panel_Lobby").transform;
        CreateRoomPopup = LobbyPanel.Find("Panel_Room").Find("Popup_CreateRoom").transform;
        JoinLockedRoomPopup = LobbyPanel.Find("Panel_Room").Find("Popup_JoinLockedRoom").transform;
        WaitPopup = LobbyPanel.Find("Panel_Room").Find("Popup_Wait").transform;

        RoomListContent = 
            LobbyPanel.Find("Panel_Room").Find("Panel_RoomList").Find("Viewport").Find("Content").transform;
        roomInfo = LobbyPanel.Find("Panel_Room").Find("Panel_RoomInfo").transform;

        //playerName = LobbyPanel.Find("LobbyInfo_PlayerName").GetComponent<TMP_Text>();
        //playerLobbyNumbers = LobbyPanel.Find("LobbyInfo_PlayerNumber").GetComponent<TMP_Text>();

        // Room
        RoomPanel = GameObject.Find("Panel_Room").transform;
    }
    #endregion

    #region 빠른 시작-PlayFab
    public void StartQuick() 
    {
        Debug.Log("PlayFab authenticating using Custom ID...");

        var request = new LoginWithCustomIDRequest { CustomId = PlayFabSettings.DeviceUniqueIdentifier,
            CreateAccount = true };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        PhotonNetwork.NickName = PlayFabSettings.DeviceUniqueIdentifier;
        Invoke("ConnectToServer", 2f);
    }
    #endregion

    #region 로그인-PlayFab
    public void Login()
    {
        TMP_InputField emailInput = LoginPopup.Find("InputField_Email").GetComponent<TMP_InputField>();
        TMP_InputField passwordInput = LoginPopup.Find("InputField_Password").GetComponent<TMP_InputField>();

        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
        };
        PlayerPrefs.SetString("name", emailInput.text);

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        PhotonNetwork.NickName = emailInput.text;
        Invoke("ConnectToServer", 3f);
    }

    private void OnLoginSuccess(LoginResult result) 
    {
        LoginPopup.GetComponentInChildren<TMP_Text>().color = Color.green;
        LoginPopup.GetComponentInChildren<TMP_Text>().text = "로그인 성공";
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogFormat("로그인 실패\n오류 코드 : {0}", error);
        LoginPopup.GetComponentInChildren<TMP_Text>().color = Color.red;
        LoginPopup.GetComponentInChildren<TMP_Text>().text = "로그인 실패, 오류 코드 : " + error;
    }
    #endregion

    #region Email 회원가입-PlayFab
    public void Register() 
    {
        TMP_InputField emailInput = SignupPopup.Find("InputField_Email").GetComponent<TMP_InputField>();
        TMP_InputField passwordInput = SignupPopup.Find("InputField_Password").GetComponent<TMP_InputField>();
        TMP_InputField nicknameInput = SignupPopup.Find("InputField_Nickname").GetComponent<TMP_InputField>();

        var request = new RegisterPlayFabUserRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
            Username = nicknameInput.text,
            DisplayName = nicknameInput.text
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.LogFormat("계정 등록 성공");
        SignupPopup.GetComponentInChildren<TMP_Text>().color = Color.green;
        SignupPopup.GetComponentInChildren<TMP_Text>().text = "계정 등록 성공";
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogFormat("계정 등록 실패\n오류 코드 : {0}", error);
        SignupPopup.GetComponentInChildren<TMP_Text>().color = Color.red;
        SignupPopup.GetComponentInChildren<TMP_Text>().text = "계정 등록 실패, 오류 코드 : " +  error;
    }

    #endregion

    //Title Login ===========================================================================================
}
