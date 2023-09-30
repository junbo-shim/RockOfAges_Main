using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;

public partial class NetworkManager : GlobalSingleton<NetworkManager>
{
    //Title Login ===========================================================================================

    // Title-Lobby-Room 까지의 Scene
    public string PhotonScene { get; private set; }
    // Game Scene
    public string GameScene { get; private set; }

    // Title 화면 (UI)
    public Transform TitlePanel { get; private set; }
    // 로그인 창 (UI)
    public Transform LoginPopup { get; private set; }
    // 회원가입 창 (UI)
    public Transform SignupPopup { get; private set; }

    // PlayFab DB 에서 불러온 계정 닉네임 (빠른 로그인 시에는 랜덤한 숫자)
    public string playerNickName;
    // PlayFab DB 에서 불러온 계정 점수 (PlayerData - Statistics)
    public int playerScore;


    protected override void Awake()
    {
        FindUIObjects();
        PhotonScene = "TitleScene";
        GameScene = "0921";

        cachedRoomList = new List<RoomInfo>();
        displayRoomList = new List<GameObject>();
        playerRoomList = new Player[4];

        DataContainerPrefab = Resources.Load<GameObject>("DataContainer");
        playerSeats = new bool[4] { false, false, false, false };
    }

    #region 서버연결-Photon
    private void ConnectToServer() 
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        // master server 연결 시 바로 로비로 연결
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
        // Title (Title 화면, 로그인 창, 회원가입 창)
        TitlePanel = GameObject.Find("Panel_Title").transform;
        LoginPopup = TitlePanel.Find("Popup_Login").transform;
        SignupPopup = TitlePanel.Find("Popup_Signup").transform;
        // Lobby (Lobby 화면, 방생성 창, 방참여 창, 경고문 창, 방 목록, 방정보)
        LobbyPanel = GameObject.Find("Panel_Lobby").transform;
        CreateRoomPopup = LobbyPanel.Find("Panel_Room").Find("Popup_CreateRoom").transform;
        JoinLockedRoomPopup = LobbyPanel.Find("Panel_Room").Find("Popup_JoinLockedRoom").transform;
        WaitPopup = LobbyPanel.Find("Panel_Room").Find("Popup_Wait").transform;

        RoomListContent = 
            LobbyPanel.Find("Panel_Room").Find("Panel_RoomList").Find("Viewport").Find("Content").transform;
        roomInfo = LobbyPanel.Find("Panel_Room").Find("Panel_RoomInfo").transform;

        //playerName = LobbyPanel.Find("LobbyInfo_PlayerName").GetComponent<TMP_Text>();
        //playerLobbyNumbers = LobbyPanel.Find("LobbyInfo_PlayerNumber").GetComponent<TMP_Text>();

        // Room (Room 화면)
        RoomPanel = GameObject.Find("Panel_Room").transform;
    }
    #endregion

    #region 빠른 시작-PlayFab
    public void StartQuick() 
    {
        Debug.Log("PlayFab authenticating using Custom ID...");

        // PlayFab API : LoginWithCustomIDRequest
        // 기기고유정보로 ID 생성
        // 만약 처음 접속하는 것이라면 자동으로 회원가입
        var request = new LoginWithCustomIDRequest { CustomId = PlayFabSettings.DeviceUniqueIdentifier,
            CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnQuickLoginSuccess, OnLoginFailure);
    }

    // StartQuick 성공 시 호출되는 메서드
    private void OnQuickLoginSuccess(LoginResult result)
    {
        // 중복 검출 로직 필요

        // 0 ~ 9999999 사이의 숫자를 게임 내 닉네임으로 설정
        // 2초 뒤 master 서버 연결
        int random = (int)Random.Range(0, 10000000);
        playerNickName = random.ToString();
        Invoke("ConnectToServer", 2f);
    }
    #endregion

    #region 로그인-PlayFab
    public void Login()
    {
        // 로그인 창의 Email 및 PW 을 입력 받을 InputField
        TMP_InputField emailInput = LoginPopup.Find("InputField_Email").GetComponent<TMP_InputField>();
        TMP_InputField passwordInput = LoginPopup.Find("InputField_Password").GetComponent<TMP_InputField>();

        // PlayFab API : LoginWithEmailAddressRequest
        // Email 과 PW 를 보내서 로그인을 시도
        // 성공 시 : OnLoginSuccess 메서드 작동
        // 실패 시 : OnLoginFailure 메서드 작동
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
        };
        //PlayerPrefs.SetString("name", emailInput.text);

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    // 로그인 성공 시 호출되는 메서드
    private void OnLoginSuccess(LoginResult result) 
    {
        // 로그인 창의 TMP Text 를 변경
        LoginPopup.GetComponentInChildren<TMP_Text>().color = Color.green;
        LoginPopup.GetComponentInChildren<TMP_Text>().text = "로그인 성공";

        // PlayFab API : GetPlayerProfileRequest
        // 해당 플레이어의 정보를 가져오는 요청
        // 성공 시 : OnGetProfileSuccess 메서드 작동
        // 실패 시 : OnGetProfileFailure 메서드 작동
        var request = new GetPlayerProfileRequest();
        PlayFabClientAPI.GetPlayerProfile(request, OnGetProfileSuccess, OnGetProfileFailure);
    }

    // 로그인 실패 시 호출되는 메서드
    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogFormat("로그인 실패\n오류 코드 : {0}", error);

        // 로그인 창의 TMP Text 를 변경
        LoginPopup.GetComponentInChildren<TMP_Text>().color = Color.red;
        LoginPopup.GetComponentInChildren<TMP_Text>().text = "로그인 실패, 오류 코드 : " + error;
    }
    #endregion

    #region 플레이어 데이터 읽기 및 쓰기-PlayFab
    // PlayFab DB 에서 로그인 한 플레이어의 데이터를 읽어오는데 성공 시 호출되는 메서드
    private void OnGetProfileSuccess(GetPlayerProfileResult result)
    {
        // NetworkManager 의 playerNickName 변수에 접속한 유저의 닉네임을 담는다
        playerNickName = result.PlayerProfile.DisplayName;
        // 접속 유저의 데이터를 읽어오는 메서드 호출
        ReadPlayerStat();
        Debug.Log("플레이어 정보 읽기 성공");
        // 3초 뒤 master 서버 연결
        Invoke("ConnectToServer", 3f);
    }

    // PlayFab DB 에서 로그인 한 플레이어의 데이터를 읽어오는데 실패 시 호출되는 메서드
    private void OnGetProfileFailure(PlayFabError error)
    {
        Debug.LogFormat("플레이어 정보 읽기 실패\n오류 코드 : {0}", error);

        // PlayFab API : GetPlayerProfileRequest
        // 플레이어 정보 불러오기 재시도
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
        // 회원가입 창의 Email, PW, 설정 닉네임을 입력 받을 InputField
        TMP_InputField emailInput = SignupPopup.Find("InputField_Email").GetComponent<TMP_InputField>();
        TMP_InputField passwordInput = SignupPopup.Find("InputField_Password").GetComponent<TMP_InputField>();
        TMP_InputField nicknameInput = SignupPopup.Find("InputField_Nickname").GetComponent<TMP_InputField>();

        // PlayFab API : RegisterPlayFabUserRequest
        // PlayFab DB 에 입력 받은 Email, PW, 닉네임과 표기이름을 송신한다
        // 성공 시 : OnRegisterSuccess 메서드 호출
        // 실패 시 : OnRegisterFailure 메서드 호출
        var request = new RegisterPlayFabUserRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
            Username = nicknameInput.text,
            DisplayName = nicknameInput.text
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    // 회원가입 성공 시 호출되는 메서드
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.LogFormat("계정 등록 성공");
        // 회원가입 창 TMP_Text 변경
        SignupPopup.GetComponentInChildren<TMP_Text>().color = Color.green;
        SignupPopup.GetComponentInChildren<TMP_Text>().text = "계정 등록 성공";
    }

    // 회원가입 실패 시 호출되는 메서드
    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogFormat("계정 등록 실패\n오류 코드 : {0}", error);
        // 회원가입 창 TMP_Text 변경
        SignupPopup.GetComponentInChildren<TMP_Text>().color = Color.red;
        SignupPopup.GetComponentInChildren<TMP_Text>().text = "계정 등록 실패, 오류 코드 : " +  error;
    }

    #endregion

    //Title Login ===========================================================================================
}
