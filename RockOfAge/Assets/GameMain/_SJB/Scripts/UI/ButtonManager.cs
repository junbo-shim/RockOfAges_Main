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
    #endregion

    #region RoomPanel 버튼들
    public Button roomStartButton;
    public Button player1Button;
    public Button player2Button;
    public Button player3Button;
    public Button player4Button;
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
    }
    #endregion

    #region Room Panel 의 모든 버튼을 찾아서 저장하는 메서드
    private void FindRoomButtons() 
    {
        roomStartButton = roomPanel.Find("Button_RoomStart").GetComponent<Button>();

        player1Button = roomPanel.Find("PlayerButtons").Find("Player1_Team1").GetComponent<Button>();
        player2Button = roomPanel.Find("PlayerButtons").Find("Player2_Team1").GetComponent<Button>();
        player3Button = roomPanel.Find("PlayerButtons").Find("Player3_Team2").GetComponent<Button>();
        player4Button = roomPanel.Find("PlayerButtons").Find("Player4_Team2").GetComponent<Button>();
    }
    #endregion

    #region 방에 들어갔을 때 마스터 클라이언트일 때만 활성화되는 Start 버튼
    private void CheckMasterClient()
    {
        // 지금 방에 참여해있다면
        if (PhotonNetwork.InRoom == true)
        {
            if (PhotonNetwork.IsMasterClient == true)
            {
                // master 이면 게임시작 버튼을 활성화한다
                roomStartButton.gameObject.SetActive(true);

                // 만약 readyCount 가 4 이하이면
                if (NetworkManager.Instance.readyCount < 4)
                {
                    // 상호작용은 불가능하게 해둔다
                    roomStartButton.interactable = false;
                }
                else if (NetworkManager.Instance.readyCount == 4)
                {
                    // 상호작용이 가능하게 만든다
                    roomStartButton.interactable = true;
                }
            }
            else
            {
                // master 가 아니면 꺼둔다
                roomStartButton.gameObject.SetActive(false);
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
        roomStartButton.onClick.AddListener(PressGameStartButton);
    }
    #endregion

    #region 모든 팝업창을 작게 만드는 메서드
    public void MakePanelsDefault()
    {
        loginPopup.localScale = Vector3.zero;
        signupPopup.localScale = Vector3.zero;

        lobbyPanel.localScale = Vector3.zero;

        createRoomPopup.localScale = Vector3.zero;
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
        // 로그인 창이 띄워져 있다면
        if (loginPopup.localScale == Vector3.one)
        {
            // 창의 크기를 0 으로 만든다
            loginPopup.localScale = Vector3.zero;

            TMP_InputField emailInput = loginPopup.Find("InputField_Email").GetComponent<TMP_InputField>();
            TMP_InputField passwordInput = loginPopup.Find("InputField_Password").GetComponent<TMP_InputField>();

            // 기존의 로그인 상태 표시와 InputField 의 내용을 모두 초기화한다
            loginPopup.GetComponentInChildren<TMP_Text>().color = new Color(180, 180, 180);
            loginPopup.GetComponentInChildren<TMP_Text>().text = "로그인";
            emailInput.text = default;
            passwordInput.text = default;
        }
        // 회원가입 창이 띄워져 있다면
        else if (signupPopup.localScale == Vector3.one)
        {
            // 창의 크기를 0 으로 만든다
            signupPopup.localScale = Vector3.zero;

            TMP_InputField emailInput = signupPopup.Find("InputField_Email").GetComponent<TMP_InputField>();
            TMP_InputField passwordInput = signupPopup.Find("InputField_Password").GetComponent<TMP_InputField>();
            TMP_InputField nicknameInput = signupPopup.Find("InputField_Nickname").GetComponent<TMP_InputField>();

            // 기존의 회원가입 상태 표시와 InputField 의 내용을 모두 초기화한다
            signupPopup.GetComponentInChildren<TMP_Text>().color = new Color(180, 180, 180);
            signupPopup.GetComponentInChildren<TMP_Text>().text = "회원가입";
            emailInput.text = default;
            passwordInput.text = default;
            nicknameInput.text = default;
        }
        // 방 생성 창이 띄워져 있다면
        else if (createRoomPopup.localScale == Vector3.one) 
        {
            // 창의 크기를 0 으로 만든다
            createRoomPopup.localScale = Vector3.zero;

            TMP_InputField roomNameInput = createRoomPopup.Find("InputField_RoomName").GetComponent<TMP_InputField>();

            // InputField 의 내용을 모두 초기화한다
            roomNameInput.text = default;
        }
        // 방에 참여해있는 상태라면
        else if (roomPanel.localScale == Vector3.one) 
        {
            // 창의 크기를 0 으로 만든다
            roomPanel.localScale = Vector3.zero;
            // 방을 나간다
            PhotonNetwork.LeaveRoom();
        }
    }
    #endregion

    #region 닫기 버튼 확인 로직
    public void CheckCloseButton()
    {
        // 로그인 창이 띄워져 있다면
        if (loginPopup.localScale == Vector3.one)
        {
            // closeButton 은 loginPopup 하위의 Button_Close
            closeButton = loginPopup.Find("Button_Close").GetComponent<Button>();
            closeButton.onClick.AddListener(PressClose);
        }
        // 회원가입 창이 띄워져 있다면
        else if (signupPopup.localScale == Vector3.one)
        {
            // closeButton 은 signupPopup 하위의 Button_Close
            closeButton = signupPopup.Find("Button_Close").GetComponent<Button>();
            closeButton.onClick.AddListener(PressClose);
        }
        // 방 생성 창이 띄워져 있다면
        else if (createRoomPopup.localScale == Vector3.one) 
        {
            // closeButton 은 createRoomPopup 하위의 Button_Close
            closeButton = createRoomPopup.Find("Button_Close").GetComponent<Button>();
            closeButton.onClick.AddListener(PressClose);
        }
        // 방에 참여해있는 상태라면
        else if (roomPanel.localScale == Vector3.one) 
        {
            // closeButton 은 roomPanel 하위의 Button_Close
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

        // 만약 InputField 내용이 비워져있다면 방 이름 text 를 생성자 이름으로 기입한다
        if (roomNameInput.text == "")
        {
            roomNameInput.text = "ROA 2:2 초보만";
        }

        // photon 내의 방 생성
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions, null, null);

        PressClose();
        PauseLobbyButtons();
        Invoke("ResetLobbyButtons", 2f);
        Invoke("OpenRoomPanel", 2f);
    }
    #endregion

    #region 랜덤 참여 버튼
    public void PressJoinRandomButton()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
        PauseLobbyButtons();
        Invoke("ResetLobbyButtons", 2f);
        Invoke("OpenRoomPanel", 2f);
    }
    #endregion

    #region 시작 버튼
    public void PressGameStartButton() 
    {
        if (PhotonNetwork.IsMasterClient == true) 
        {
            //
            dataContainerView = NetworkManager.Instance.myDataContainer.GetComponent<PhotonView>();
            // 마스터의 Scene 을 동기화한다
            PhotonNetwork.AutomaticallySyncScene = true;
            // 모든 photonView 에 RPC 를 쏴서 StartGame 메서드를 실행시킨다
            dataContainerView.RPC("StartGame", RpcTarget.All);
        }
    }
    #endregion

    //Button Functions ====================================================================================
    #endregion
}