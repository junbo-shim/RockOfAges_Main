using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public partial class NetworkManager : GlobalSingleton<NetworkManager>
{
    //Title Login ===========================================================================================

    #region 필드
    public string PhotonScene { get; private set; }
    public string GameScene { get; private set; }

    public Transform TitlePanel { get; private set; }
    public Transform LoginPopup { get; private set; }
    public Transform SignupPopup { get; private set; }

    public string playerNickName;
    public int playerScore;
    #endregion


    protected override void Awake()
    {
        FindUIObjects();
        PhotonScene = "TitleScene";
        GameScene = "0921";

        cachedRoomDictionary = new Dictionary<string, RoomInfo>();
        dataContainers = new List<PlayerDataContainer>();

        DataContainerPrefab = Resources.Load<GameObject>("DataContainer");
        readyCount = default;
    }

    protected override void Update()
    {
        if (SceneManager.GetActiveScene().name == PhotonScene) 
        {
            // 로비 인원 출력 - 마땅한 callback 을 못찾았음
            playerLobbyNumbers.text = PhotonNetwork.CountOfPlayersOnMaster.ToString() + "/20";
        }
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
        //base.OnDisconnected(cause);
        //Debug.LogFormat("You are Disconnected : {0}", cause);
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
        WaitPopup = LobbyPanel.Find("Panel_Room").Find("Popup_Wait").transform;

        OriginalRoomObject = LobbyPanel.Find("Button_Room").gameObject;
        RoomListContent = 
            LobbyPanel.Find("Panel_Room").Find("Panel_RoomList").Find("Viewport").Find("Content").transform;
        roomInfo = LobbyPanel.Find("Panel_Room").Find("Panel_RoomInfo").transform;

        playerLobbyName = LobbyPanel.Find("LobbyInfo_PlayerName").GetComponent<TMP_Text>();
        playerLobbyNumbers = LobbyPanel.Find("LobbyInfo_PlayerNumber").GetComponent<TMP_Text>();

        // Room
        RoomPanel = GameObject.Find("Panel_Room").transform;
        roomName = RoomPanel.Find("Text (TMP)").GetComponent<TMP_Text>();
    }
    #endregion

    #region 빠른 시작-PlayFab
    public void StartQuick() 
    {
        //Debug.Log("PlayFab authenticating using Custom ID...");

        var request = new LoginWithCustomIDRequest { CustomId = PlayFabSettings.DeviceUniqueIdentifier,
            CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnQuickLoginSuccess, OnLoginFailure);
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
    }

    private void OnQuickLoginSuccess(LoginResult result)
    {
        // 중복 검출 로직 필요
        int random = (int)Random.Range(0, 10000000);
        playerNickName = random.ToString();
        Invoke("ConnectToServer", 2f);
    }

    private void OnLoginSuccess(LoginResult result) 
    {
        LoginPopup.GetComponentInChildren<TMP_Text>().color = Color.green;
        LoginPopup.GetComponentInChildren<TMP_Text>().text = "로그인 성공";

        var request = new GetPlayerProfileRequest();
        PlayFabClientAPI.GetPlayerProfile(request, OnGetProfileSuccess, OnGetProfileFailure);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        //Debug.LogFormat("로그인 실패\n오류 코드 : {0}", error);
        LoginPopup.GetComponentInChildren<TMP_Text>().color = Color.red;
        LoginPopup.GetComponentInChildren<TMP_Text>().text = "로그인 실패, 오류 코드 : " + error;
    }

    private void OnGetProfileSuccess(GetPlayerProfileResult result) 
    {
        //Debug.Log("플레이어 정보 읽기 성공");
        playerNickName = result.PlayerProfile.DisplayName;
        ReadPlayerStat();
        Invoke("ConnectToServer", 3f);
    }

    private void OnGetProfileFailure(PlayFabError error) 
    {
        //Debug.LogFormat("플레이어 정보 읽기 실패\n오류 코드 : {0}", error);

        var request = new GetPlayerProfileRequest();
        PlayFabClientAPI.GetPlayerProfile(request, OnGetProfileSuccess, OnGetProfileFailure);
    }

    // 플레이어의 Statistic 목록에서 특정 요소 (Score) 를 읽어오는 메서드
    public void ReadPlayerStat()
    {
        // PlayFab API : GetPlayerStatisticsRequest
        // PlayFab DB 에 있는 Score 정보 요청
        // 성공 시 : OnReadStaticSuccess 호출
        // 실패 시 : OnReadStaticFailure 호출
        var request = new GetPlayerStatisticsRequest();
        PlayFabClientAPI.GetPlayerStatistics(request, OnReadStaticSuccess, OnReadStaticFailure);
    }

    // 플레이어 Score 를 PlayFab DB 에서 로드 성공 시 호출되는 메서드
    private void OnReadStaticSuccess(GetPlayerStatisticsResult result)
    {
        // List 형태의 result 에서 foreach 문으로 하나씩 변수에 할당
        foreach (var data in result.Statistics)
        {
            playerScore = data.Value;
        }

        Debug.Log("Score 정보 로드 성공");
    }

    // 플레이어 Score 를 PlayFab DB 에서 로드 실패 시 호출되는 메서드
    private void OnReadStaticFailure(PlayFabError error)
    {
        Debug.Log("Score 정보 로드에 실패했습니다.");

        // 데이터 로드 재시도
        ReadPlayerStat();
    }

    // 플레이어의 Statistic 목록에서 특정 요소 (Score) 를 덮어쓰는 메서드
    public void WritePlayerStat()
    {
        // PlayFab API : UpdatePlayerStatisticsRequest
        // List 구조로 되어 있는 Statistics 변수에
        // StatisticUpdate 요소를 하나씩 작성하여 업로드
        // 성공 시 : OnWriteStaticSuccess 메서드 호출
        // 실패 시 : OnWriteStaticFailure 메서드 호출
        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "Score", Value = playerScore }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnWriteStaticSuccess, OnWriteStaticFailure);
    }

    // 플레이어 Score 를 PlayFab DB 에 업로드 성공 시 호출되는 메서드
    private void OnWriteStaticSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Score 정보 업데이트 성공");
    }

    // 플레이어 Score 를 PlayFab DB 에 업로드 실패 시 호출되는 메서드
    private void OnWriteStaticFailure(PlayFabError error)
    {
        Debug.Log("Score 정보 업데이트에 실패했습니다.");

        // 업데이트 재시도
        WritePlayerStat();
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
        //Debug.LogFormat("계정 등록 성공");
        SignupPopup.GetComponentInChildren<TMP_Text>().color = Color.green;
        SignupPopup.GetComponentInChildren<TMP_Text>().text = "계정 등록 성공";
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        //Debug.LogFormat("계정 등록 실패\n오류 코드 : {0}", error);
        SignupPopup.GetComponentInChildren<TMP_Text>().color = Color.red;
        SignupPopup.GetComponentInChildren<TMP_Text>().text = "계정 등록 실패, 오류 코드 : " +  error;
    }

    #endregion

    //Title Login ===========================================================================================
}
