using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class ButtonManager : GlobalSingleton<ButtonManager>
{
    #region Title 및 Lobby 하위 Panel 및 Popup 들
    private Transform titlePanel;
    private Transform loginPopup;
    private Transform signupPopup;

    private Transform lobbyPanel;
    private Transform createRoomPopup;
    private Transform joinLockedRoomPopup;
    private Transform waitPopup;

    private Transform roomPanel;
    #endregion

    #region Title Panel 버튼들
    private Button titleOptionButton;
    private Button quickStartButton;
    private Button loginButton;
    private Button signupButton;
    private Button quitButton;

    private Button startButton;
    private Button resetPWButton;
    private Button closeButton;

    private Button registerButton;
    #endregion

    #region Lobby Panel 버튼들
    private Button lobbyOptionButton;

    public GameObject roomPrefab;
    private Button createRoomButton;
    private Button JoinRandomButton;

    private Button createConfirmButton;
    private Button commitPWButton;
    #endregion

    #region RoomPanel 버튼들
    public Button startReadyButton;
    public Button player1Button;
    public Button player2Button;
    public Button player3Button;
    public Button player4Button;

    public bool isReady;
    #endregion

    public PhotonView dataContainerView;

    protected override void Awake()
    {
        GetAllPanels();
        FinTitleButtons();
        FindLobbyButtons();
        FindRoomButtons();
        ListentAllButton();
        MakePanelsDefault();
    }

    protected override void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name == NetworkManager.Instance.PhotonScene) 
        {
            CheckCloseButton();
            CheckMasterClient();
        }
    }

    #region 모든 Panel 을 NetworkManager로부터 받아오는 메서드
    private void GetAllPanels() 
    {
        titlePanel = NetworkManager.Instance.TitlePanel;
        lobbyPanel = NetworkManager.Instance.LobbyPanel;
        roomPanel = NetworkManager.Instance.RoomPanel;

        loginPopup = NetworkManager.Instance.LoginPopup;
        signupPopup = NetworkManager.Instance.SignupPopup;

        createRoomPopup = NetworkManager.Instance.CreateRoomPopup;
        joinLockedRoomPopup = NetworkManager.Instance.JoinLockedRoomPopup;
        waitPopup = NetworkManager.Instance.WaitPopup;
    }
    #endregion

    #region Title Panel 의 모든 버튼을 찾아서 저장하는 메서드
    private void FinTitleButtons()
    {
        titleOptionButton = titlePanel.Find("Button_Option").GetComponent<Button>();
        quickStartButton = titlePanel.Find("Button_QuickStart").GetComponent<Button>();
        loginButton = titlePanel.Find("Button_Login").GetComponent<Button>();
        signupButton = titlePanel.Find("Button_Signup").GetComponent<Button>();
        quitButton = titlePanel.Find("Button_Quit").GetComponent<Button>();

        startButton = loginPopup.Find("Button_Start").GetComponent<Button>();
        resetPWButton = loginPopup.Find("Button_ResetPW").GetComponent<Button>();

        registerButton = signupPopup.Find("Button_Register").GetComponent<Button>();
    }
    #endregion

    #region Lobby Panel 의 모든 버튼을 찾아서 저장하는 메서드
    private void FindLobbyButtons()
    {
        lobbyOptionButton = lobbyPanel.Find("Button_Option").GetComponent<Button>();
        roomPrefab = lobbyPanel.Find("Button_Room").gameObject;

        createRoomButton = lobbyPanel.Find("Panel_Room").Find("Button_CreateRoom").GetComponent<Button>();
        JoinRandomButton = lobbyPanel.Find("Panel_Room").Find("Button_JoinRandomRoom").GetComponent<Button>();

        createConfirmButton = createRoomPopup.Find("Button_Create").GetComponent<Button>();
        commitPWButton = joinLockedRoomPopup.Find("Button_Join").GetComponent<Button>();
    }
    #endregion

    #region Room Panel 의 모든 버튼을 찾아서 저장하는 메서드
    private void FindRoomButtons() 
    {
        startReadyButton = roomPanel.Find("Button_RoomStartReady").GetComponent<Button>();

        player1Button = roomPanel.Find("PlayerButtons").Find("Player1").GetComponent<Button>();
        player2Button = roomPanel.Find("PlayerButtons").Find("Player2").GetComponent<Button>();
        player3Button = roomPanel.Find("PlayerButtons").Find("Player3").GetComponent<Button>();
        player4Button = roomPanel.Find("PlayerButtons").Find("Player4").GetComponent<Button>();
    }
    #endregion

    #region 방에 들어갔을 때 마스터 클라이언트임을 체크하면 바뀌는 Start, Ready 버튼
    private void CheckMasterClient()
    {
        if (PhotonNetwork.InRoom == true)
        {
            FindDataContainer();
            if (PhotonNetwork.IsMasterClient == true)
            {
                startReadyButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "START";

                // NullReference Exception
                if (dataContainerView.GetComponent<PlayerDataContainer>().otherPlayerReady == 3)
                {
                    startReadyButton.interactable = true;
                }
                else if (dataContainerView.GetComponent<PlayerDataContainer>().otherPlayerReady < 3)
                {
                    startReadyButton.interactable = false;
                }
                else
                {
                    Debug.Log("레디버튼 관련 에러 발생");
                }
            }
            else
            {
                startReadyButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "READY";
                startReadyButton.interactable = true;
            }
        }
    }
    #endregion

    #region 버튼에 AddListener 세팅하는 메서드
    private void ListentAllButton()
    {
        titleOptionButton.onClick.AddListener(PressOption);
        quickStartButton.onClick.AddListener(PressQuickStart);
        loginButton.onClick.AddListener(PressLogin);
        signupButton.onClick.AddListener(PressSignup);
        quitButton.onClick.AddListener(PressQuit);

        startButton.onClick.AddListener(PressStart);
        resetPWButton.onClick.AddListener(PressResetPW);

        registerButton.onClick.AddListener(PressRegister);

        createRoomButton.onClick.AddListener(PressCreateRoomButton);
        JoinRandomButton.onClick.AddListener(PressJoinRandomButton);

        createConfirmButton.onClick.AddListener(PressConfirmCreateButton);
        startReadyButton.onClick.AddListener(PressStartReadyButton);

        player1Button.onClick.AddListener(PressPlayer1);
        player2Button.onClick.AddListener(PressPlayer2);
        player3Button.onClick.AddListener(PressPlayer3);
        player4Button.onClick.AddListener(PressPlayer4);
    }
    #endregion

    #region 모든 팝업창을 작게 만드는 메서드
    public void MakePanelsDefault()
    {
        loginPopup.localScale = Vector3.zero;
        signupPopup.localScale = Vector3.zero;

        lobbyPanel.localScale = Vector3.zero;

        createRoomPopup.localScale = Vector3.zero;
        joinLockedRoomPopup.localScale = Vector3.zero;
        waitPopup.localScale = Vector3.zero;

        roomPanel.localScale = Vector3.zero;
    }
    #endregion

    #region 로그인 시 잠시 버튼을 Disable 하는 메서드
    public void PauseTitleButtons() 
    {
        titleOptionButton.interactable = false;
        quickStartButton.interactable = false;
        loginButton.interactable = false;
        signupButton.interactable = false;
        quitButton.interactable = false;
    }
    #endregion

    #region 로그인 버튼들을 다시 able 상태로 되돌리는 메서드
    public void ResetTitleButtons() 
    {
        titleOptionButton.interactable = true;
        quickStartButton.interactable = true;
        loginButton.interactable = true;
        signupButton.interactable = true;
        quitButton.interactable = true;
    }
    #endregion

    #region 로비 버튼들을 잠시 Disable 하는 메서드
    public void PauseLobbyButtons()
    {
        lobbyOptionButton.interactable = false;
        createRoomButton.interactable = false;
        JoinRandomButton.interactable = false;
    }
    #endregion

    #region 로비 버튼들을 다시 able 상태로 되돌리는 메서드
    public void ResetLobbyButtons() 
    {
        lobbyOptionButton.interactable = true;
        createRoomButton.interactable = true;
        JoinRandomButton.interactable = true;
    }
    #endregion

    #region Room Panel 켜는 메서드
    public void OpenRoomPanel() 
    {
        roomPanel.localScale = Vector3.one;
    }
    #endregion

    #region PlayerDataContainer 가 생성된 후 찾아올 메서드
    public void FindDataContainer() 
    {
        dataContainerView = NetworkManager.Instance.myDataContainer.GetComponent<PhotonView>();
    }
    #endregion

    #region 버튼 기능 메서드
    //Button Functions ====================================================================================

    #region 옵션버튼
    public void PressOption()
    {
        /*Empty For Now*/
    }
    #endregion

    #region 빠른 시작 버튼
    public void PressQuickStart()
    {
        NetworkManager.Instance.StartQuick();
        PauseTitleButtons();
        Invoke("ResetTitleButtons", 5f);
    }
    #endregion

    #region 로그인 버튼
    public void PressLogin()
    {
        loginPopup.localScale = Vector3.one;

        loginPopup.GetComponentInChildren<TMP_Text>().color = new Color(180, 180, 180);
        loginPopup.GetComponentInChildren<TMP_Text>().text = "로그인";
    }
    #endregion

    #region 회원가입 버튼
    public void PressSignup()
    {
        signupPopup.localScale = Vector3.one;
    }
    #endregion

    #region 끝내기 버튼
    public void PressQuit()
    {
        Application.Quit();
    }
    #endregion

    #region 로그인-게임 시작 버튼
    public void PressStart()
    {
        NetworkManager.Instance.Login();
        PauseTitleButtons();
        Invoke("ResetTitleButtons", 5f);        
        Invoke("PressClose", 2f);
    }
    #endregion

    #region 로그인-리셋 버튼
    public void PressResetPW()
    {
        /*Empty For Now*/
    }
    #endregion

    #region 회원가입-진짜가입 버튼
    public void PressRegister()
    {
        NetworkManager.Instance.Register();
        Invoke("PressClose", 2f);
    }
    #endregion

    #region 닫기 버튼
    public void PressClose()
    {
        if (loginPopup.localScale == Vector3.one)
        {
            loginPopup.localScale = Vector3.zero;

            TMP_InputField emailInput = loginPopup.Find("InputField_Email").GetComponent<TMP_InputField>();
            TMP_InputField passwordInput = loginPopup.Find("InputField_Password").GetComponent<TMP_InputField>();

            loginPopup.GetComponentInChildren<TMP_Text>().color = new Color(180, 180, 180);
            loginPopup.GetComponentInChildren<TMP_Text>().text = "로그인";
            emailInput.text = default;
            passwordInput.text = default;
        }
        else if (signupPopup.localScale == Vector3.one)
        {
            signupPopup.localScale = Vector3.zero;

            TMP_InputField emailInput = signupPopup.Find("InputField_Email").GetComponent<TMP_InputField>();
            TMP_InputField passwordInput = signupPopup.Find("InputField_Password").GetComponent<TMP_InputField>();
            TMP_InputField nicknameInput = signupPopup.Find("InputField_Nickname").GetComponent<TMP_InputField>();

            signupPopup.GetComponentInChildren<TMP_Text>().color = new Color(180, 180, 180);
            signupPopup.GetComponentInChildren<TMP_Text>().text = "회원가입";
            emailInput.text = default;
            passwordInput.text = default;
            nicknameInput.text = default;
        }
        else if (createRoomPopup.localScale == Vector3.one) 
        {
            createRoomPopup.localScale = Vector3.zero;

            TMP_InputField roomNameInput = createRoomPopup.Find("InputField_RoomName").GetComponent<TMP_InputField>();
            TMP_InputField roomPWInput = createRoomPopup.Find("InputField_RoomPW").GetComponent<TMP_InputField>();

            roomNameInput.text = default;
            roomPWInput.text = default;
        }
        else if (joinLockedRoomPopup.localScale == Vector3.one) 
        {
            joinLockedRoomPopup.localScale = Vector3.zero;

            TMP_InputField roomPWInput = createRoomPopup.Find("InputField_RoomPW").GetComponent<TMP_InputField>();
            roomPWInput.text = default;
        }
        else if (roomPanel.localScale == Vector3.one) 
        {
            roomPanel.localScale = Vector3.zero;
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.JoinLobby();
        }
    }
    #endregion

    #region 닫기 버튼 확인 로직
    public void CheckCloseButton()
    {
        if (loginPopup.localScale == Vector3.one)
        {
            closeButton = loginPopup.Find("Button_Close").GetComponent<Button>();
            closeButton.onClick.AddListener(PressClose);
        }
        else if (signupPopup.localScale == Vector3.one)
        {
            closeButton = signupPopup.Find("Button_Close").GetComponent<Button>();
            closeButton.onClick.AddListener(PressClose);
        }
        else if (createRoomPopup.localScale == Vector3.one) 
        {
            closeButton = createRoomPopup.Find("Button_Close").GetComponent<Button>();
            closeButton.onClick.AddListener(PressClose);
        }
        else if (joinLockedRoomPopup.localScale == Vector3.one) 
        {
            closeButton = joinLockedRoomPopup.Find("Button_Close").GetComponent<Button>();
            closeButton.onClick.AddListener(PressClose);
        }
        else if (roomPanel.localScale == Vector3.one) 
        {
            closeButton = roomPanel.Find("Button_Close").GetComponent<Button>();
            closeButton.onClick.AddListener(PressClose);
        }
    }
    #endregion

    #region 방 생성 버튼
    public void PressCreateRoomButton() 
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            createRoomPopup.localScale = Vector3.one;
        }
    }
    #endregion

    #region 방 생성-진짜 생성 버튼
    public void PressConfirmCreateButton() 
    {
        TMP_InputField roomNameInput = createRoomPopup.Find("InputField_RoomName").GetComponent<TMP_InputField>();
        //TMP_InputField roomPWInput = createRoomPopup.Find("InputField_RoomPW").GetComponent<TMP_InputField>();

        // 포톤 내의 방 생성
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions, null, null);

        PressClose();
        PauseLobbyButtons();
        Invoke("ResetLobbyButtons", 3f);
        Invoke("OpenRoomPanel", 3f);
    }
    #endregion

    #region 랜덤 참여 버튼
    public void PressJoinRandomButton()
    {
        PhotonNetwork.JoinRandomRoom();
        PauseLobbyButtons();
        Invoke("ResetLobbyButtons", 3f);
        Invoke("OpenRoomPanel", 3f);
    }
    #endregion

    #region 팀 참여 버튼
    public void PressPlayer1()
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            FindDataContainer();
            dataContainerView.RPC("SendPlayerPosition", RpcTarget.MasterClient, 
                NetworkManager.Instance.playerNickName, dataContainerView.ViewID, 0);
        }
        else if (PhotonNetwork.IsMasterClient == true) 
        {
            if (NetworkManager.Instance.playerSeats[0] == false) 
            {
                string masterNickName = NetworkManager.Instance.playerNickName;
                string masterViewID = dataContainerView.ViewID.ToString();

                //int beforeSeat = default;
                Debug.Log(NetworkManager.Instance.playerNickName);
                if (NetworkManager.Instance.roomSetting.ContainsKey(masterViewID)) 
                {
                    string seatNumber = 
                        NetworkManager.Instance.roomSetting[masterNickName].ToString();

                    int index = int.Parse(seatNumber.Split("Player")[1]);
                    //
                    //beforeSeat = index - 1;

                    NetworkManager.Instance.playerSeats[index - 1] = false;
                    NetworkManager.Instance.roomSetting[masterViewID] = "Player1_Team1";
                    NetworkManager.Instance.roomSetting["Player1"] = masterNickName;
                    NetworkManager.Instance.roomSetting[masterNickName] = masterViewID;
                    Debug.LogFormat("master = Player1, Player{0} 에서 이동함", index);
                }
                else
                {
                    NetworkManager.Instance.roomSetting[masterViewID] = "Player1_Team1";
                    NetworkManager.Instance.roomSetting["Player1"] = masterNickName;
                    NetworkManager.Instance.roomSetting[masterNickName] = masterViewID;
                    Debug.Log("master = Player1");
                }
                PhotonNetwork.CurrentRoom.SetCustomProperties(NetworkManager.Instance.roomSetting);
                NetworkManager.Instance.playerSeats[0] = true;
                //
                //dataContainerView.RPC("ChangeID", RpcTarget.All, dataContainerView.ViewID, 0, beforeSeat);
            }
            else if (NetworkManager.Instance.playerSeats[0] == true)
            {
                Debug.Log("Seat Already Taken");
            }
        }
    }

    public void PressPlayer2()
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            FindDataContainer();
            dataContainerView.RPC("SendPlayerPosition", RpcTarget.MasterClient, 
                NetworkManager.Instance.playerNickName, dataContainerView.ViewID, 1);
        }
        else if (PhotonNetwork.IsMasterClient == true) 
        {
            if (NetworkManager.Instance.playerSeats[1] == false)
            {
                string masterNickName = NetworkManager.Instance.playerNickName;
                string masterViewID = dataContainerView.ViewID.ToString();

                Debug.Log(NetworkManager.Instance.playerNickName);
                if (NetworkManager.Instance.roomSetting.ContainsKey(masterViewID))
                {
                    string seatNumber =
                        NetworkManager.Instance.roomSetting[masterNickName].ToString();

                    int index = int.Parse(seatNumber.Split("Player")[1]);
                    NetworkManager.Instance.playerSeats[index - 1] = false;
                    NetworkManager.Instance.roomSetting[masterViewID] = "Player2_Team1";
                    NetworkManager.Instance.roomSetting["Player2"] = masterNickName;
                    NetworkManager.Instance.roomSetting[masterNickName] = masterViewID;
                    Debug.LogFormat("master = Player2, Player{0} 에서 이동함", index);
                }
                else
                {
                    NetworkManager.Instance.roomSetting[masterViewID] = "Player2_Team1";
                    NetworkManager.Instance.roomSetting["Player2"] = masterNickName;
                    NetworkManager.Instance.roomSetting[masterNickName] = masterViewID;
                    Debug.Log("master = Player2");
                }
                PhotonNetwork.CurrentRoom.SetCustomProperties(NetworkManager.Instance.roomSetting);
                NetworkManager.Instance.playerSeats[1] = true;
                //dataContainerView.RPC("ChangeID", RpcTarget.All, dataContainerView.ViewID, 1);
            }
            else if(NetworkManager.Instance.playerSeats[1] == true)
            {
                Debug.Log("Seat Already Taken");
            }
        }
    }

    public void PressPlayer3()
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            FindDataContainer();
            dataContainerView.RPC("SendPlayerPosition", RpcTarget.MasterClient, 
                NetworkManager.Instance.playerNickName, dataContainerView.ViewID, 2);
        }
        else if (PhotonNetwork.IsMasterClient == true) 
        {
            if (NetworkManager.Instance.playerSeats[2] == false)
            {
                string masterNickName = NetworkManager.Instance.playerNickName;
                string masterViewID = dataContainerView.ViewID.ToString();

                Debug.Log(NetworkManager.Instance.playerNickName);
                if (NetworkManager.Instance.roomSetting.ContainsKey(masterViewID))
                {
                    string seatNumber =
                        NetworkManager.Instance.roomSetting[masterNickName].ToString();

                    int index = int.Parse(seatNumber.Split("Player")[1]);
                    NetworkManager.Instance.playerSeats[index - 1] = false;
                    NetworkManager.Instance.roomSetting[masterViewID] = "Player3_Team2";
                    NetworkManager.Instance.roomSetting["Player3"] = masterNickName;
                    NetworkManager.Instance.roomSetting[masterNickName] = masterViewID;
                    Debug.LogFormat("master = Player3, Player{0} 에서 이동함", index);
                }
                else
                {
                    NetworkManager.Instance.roomSetting[masterViewID] = "Player3_Team2";
                    NetworkManager.Instance.roomSetting["Player3"] = masterNickName;
                    NetworkManager.Instance.roomSetting[masterNickName] = masterViewID;
                    Debug.Log("master = Player3");
                }
                PhotonNetwork.CurrentRoom.SetCustomProperties(NetworkManager.Instance.roomSetting);
                NetworkManager.Instance.playerSeats[2] = true;
                //dataContainerView.RPC("ChangeID", RpcTarget.All, dataContainerView.ViewID, 2);
            }
            else if (NetworkManager.Instance.playerSeats[2] == true)
            {
                Debug.Log("Seat Already Taken");
            }
        }
    }

    public void PressPlayer4()
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            FindDataContainer();
            dataContainerView.RPC("SendPlayerPosition", RpcTarget.MasterClient, 
                NetworkManager.Instance.playerNickName, dataContainerView.ViewID, 3);
        }
        else if (PhotonNetwork.IsMasterClient == true) 
        {
            if (NetworkManager.Instance.playerSeats[3] == false)
            {
                string masterNickName = NetworkManager.Instance.playerNickName;
                string masterViewID = dataContainerView.ViewID.ToString();

                Debug.Log(NetworkManager.Instance.playerNickName);
                if (NetworkManager.Instance.roomSetting.ContainsKey(masterViewID))
                {
                    string seatNumber =
                        NetworkManager.Instance.roomSetting[masterNickName].ToString();

                    int index = int.Parse(seatNumber.Split("Player")[1]);
                    NetworkManager.Instance.playerSeats[index - 1] = false;
                    NetworkManager.Instance.roomSetting[masterViewID] = "Player4_Team2";
                    NetworkManager.Instance.roomSetting["Player4"] = masterNickName;
                    NetworkManager.Instance.roomSetting[masterNickName] = masterViewID;
                    Debug.LogFormat("master = Player4, Player{0} 에서 이동함", index);
                }
                else
                {
                    NetworkManager.Instance.roomSetting[masterViewID] = "Player4_Team2";
                    NetworkManager.Instance.roomSetting["Player4"] = masterNickName;
                    NetworkManager.Instance.roomSetting[masterNickName] = masterViewID;
                    Debug.Log("master = Player4");
                }
                PhotonNetwork.CurrentRoom.SetCustomProperties(NetworkManager.Instance.roomSetting);
                NetworkManager.Instance.playerSeats[3] = true;
                //dataContainerView.RPC("ChangeID", RpcTarget.All, dataContainerView.ViewID, 3);
            }
            else if (NetworkManager.Instance.playerSeats[3] == true)
            {
                Debug.Log("Seat Already Taken");
            }
        }
    }
    #endregion

    #region 시작 버튼
    public void PressStartReadyButton() 
    {
        if (PhotonNetwork.IsMasterClient == true) 
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            dataContainerView.RPC("StartGame", RpcTarget.All);
        }
        else 
        {
            FindDataContainer();
            if (isReady == false) 
            {
                isReady = true;
                dataContainerView.RPC("ChangeMasterReadyValue", RpcTarget.MasterClient, isReady, dataContainerView.ViewID.ToString(), dataContainerView.IsMine);
                startReadyButton.GetComponent<Image>().color = Color.grey;
            }
            else if (isReady == true)
            {
                isReady = false;
                dataContainerView.RPC("ChangeMasterReadyValue", RpcTarget.MasterClient, isReady, dataContainerView.ViewID.ToString(), dataContainerView.IsMine);
                startReadyButton.GetComponent<Image>().color = Color.white;
            }
        }
    }
    #endregion

    //Button Functions ====================================================================================
    #endregion
}