using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

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

    private GameObject roomPrefab;
    private Button createRoomButton;
    private Button JoinRandomButton;

    private Button createConfirmButton;
    private Button commitPWButton;
    #endregion



    protected override void Awake()
    {
        GetAllPanels();
        FinTitleButtons();
        FindLobbyButtons();
        ListentAllButton();
        MakePanelsDefault();
    }

    protected override void Update()
    {
        CheckCloseButton();
    }

    #region 모든 Panel 을 NetworkManager로부터 받아오는 메서드
    private void GetAllPanels() 
    {
        titlePanel = NetworkManager.Instance.TitlePanel;
        lobbyPanel = NetworkManager.Instance.LobbyPanel;
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
        //commitPWButton.onClick
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
    }
    #endregion

    #region 방 생성 버튼
    public void PressCreateRoomButton() 
    {
        createRoomPopup.localScale = Vector3.one;
    }
    #endregion

    #region 방 생성-진짜 생성 버튼
    public void PressConfirmCreateButton() 
    {
        TMP_InputField roomNameInput = createRoomPopup.Find("InputField_RoomName").GetComponent<TMP_InputField>();
        TMP_InputField roomPWInput = createRoomPopup.Find("InputField_RoomPW").GetComponent<TMP_InputField>();

        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions, null, null);
        GameObject room = Instantiate(roomPrefab, NetworkManager.Instance.RoomListContent);

        NetworkManager.Instance.roomLists.Add(room);

        PressClose();
    }
    #endregion

    #region 랜덤 참여 버튼
    public void PressJoinRandomButton() 
    {
        // 룸 리스트 필요
        // 룸이 없다면 Wait 팝업에서 경고문을 띄우고 1.5초 뒤 꺼짐
        // 룸이 있다면 Wait 팝업에서 참가중을 띄우고 방으로
    }
    #endregion

    //Button Functions ====================================================================================
    #endregion
}