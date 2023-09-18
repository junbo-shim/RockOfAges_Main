using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

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
    private Button joinTeam1Button;
    private Button joinTeam2Button;

    //public Button roomReadyButton;
    public Button startReadyButton;
    public Button goTeam1Button;
    public Button goTeam2Button;
    #endregion

    protected override void Awake()
    {
        GetAllPanels();
        FinTitleButtons();
        FindLobbyButtons();
        FindRoomButtons();
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

        if (PhotonNetwork.IsMasterClient == true) 
        {
            startReadyButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "START";
            startReadyButton.interactable = false;
        }
        else 
        {
            startReadyButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "READY";
            startReadyButton.interactable = true;
        }

        goTeam1Button =
            roomPanel.Find("Buttons").Find("JoinTeam1").GetComponent<Button>();
        goTeam2Button =
            roomPanel.Find("Buttons").Find("JoinTeam2").GetComponent<Button>();
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

        goTeam1Button.onClick.AddListener(PressJoinTeam1);
        goTeam1Button.onClick.AddListener(PressJoinTeam2);
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
        Invoke("ResetTitleButtons", 4f);
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
        Invoke("ResetTitleButtons", 4f);
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
        
        //Photon.Realtime.Room newRoom = PhotonNetwork.CurrentRoom;

        // 포톤 방의 데이터 세팅 (커스텀 프로퍼티-Hashtable)
        //ExitGames.Client.Photon.Hashtable roomHashTable = new ExitGames.Client.Photon.Hashtable();
        //roomHashTable["password"] = roomPWInput.text;
        //roomHashTable["Seat_1"] = "Empty";
        //roomHashTable["Seat_2"] = "Empty";
        //roomHashTable["Seat_3"] = "Empty";
        //roomHashTable["Seat_4"] = "Empty";

        //newRoom.SetCustomProperties(roomHashTable);

        PressClose();

        roomPanel.localScale = Vector3.one;

        //NetworkManager.Instance.UpdateRoomDisplay();
    }
    #endregion

    #region 랜덤 참여 버튼
    public void PressJoinRandomButton()
    {
        PhotonNetwork.JoinRandomRoom();
        roomPanel.localScale = Vector3.one;
    }
    #endregion

    #region 팀 참여 버튼
    private void PressJoinTeam1()
    {
        if (PhotonNetwork.IsMasterClient == true) 
        {
            if (NetworkManager.Instance.team1Entry[0] == default && NetworkManager.Instance.team1Entry[1] == default)
            {
                NetworkManager.Instance.team1Entry[0] = 1;
            }
            else if (NetworkManager.Instance.team1Entry[0] == default && NetworkManager.Instance.team1Entry[1] != default)
            {
                NetworkManager.Instance.team1Entry[0] = 1;
            }
            else if (NetworkManager.Instance.team1Entry[0] != default && NetworkManager.Instance.team1Entry[1] == default)
            {
                NetworkManager.Instance.team1Entry[1] = 2;
            }
            else if (NetworkManager.Instance.team1Entry[0] != default && NetworkManager.Instance.team1Entry[1] != default)
            {
                /*Do Nothing*/
            }
        }
        else 
        {
            if (NetworkManager.Instance.team1Entry[0] == default && NetworkManager.Instance.team1Entry[1] == default) 
            {
                NetworkManager.Instance.team1Entry[0] = 1;
                photonView.RPC("SendMasterRoomPosition", RpcTarget.MasterClient, NetworkManager.Instance.team1Entry[0]);
            }
            else if (NetworkManager.Instance.team1Entry[0] == default && NetworkManager.Instance.team1Entry[1] != default) 
            {
                NetworkManager.Instance.team1Entry[0] = 1;
                photonView.RPC("SendMasterRoomPosition", RpcTarget.MasterClient, NetworkManager.Instance.team1Entry[0]);
            }
            else if (NetworkManager.Instance.team1Entry[0] != default && NetworkManager.Instance.team1Entry[1] == default)
            {
                NetworkManager.Instance.team1Entry[1] = 2;
                photonView.RPC("SendMasterRoomPosition", RpcTarget.MasterClient, NetworkManager.Instance.team1Entry[1]);
            }
            else if (NetworkManager.Instance.team1Entry[0] != default && NetworkManager.Instance.team1Entry[1] != default)
            {
                /*Do Nothing*/
            }
        }
    }

    private void PressJoinTeam2()
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            if (NetworkManager.Instance.team2Entry[0] == default && NetworkManager.Instance.team2Entry[1] == default)
            {
                NetworkManager.Instance.team2Entry[0] = 3;
            }
            else if (NetworkManager.Instance.team2Entry[0] == default && NetworkManager.Instance.team2Entry[1] != default)
            {
                NetworkManager.Instance.team2Entry[0] = 3;
            }
            else if (NetworkManager.Instance.team2Entry[0] != default && NetworkManager.Instance.team2Entry[1] == default)
            {
                NetworkManager.Instance.team2Entry[1] = 4;
            }
            else if (NetworkManager.Instance.team2Entry[0] != default && NetworkManager.Instance.team2Entry[1] != default)
            {
                /*Do Nothing*/
            }
        }
        else
        {
            if (NetworkManager.Instance.team2Entry[0] == default && NetworkManager.Instance.team2Entry[1] == default)
            {
                NetworkManager.Instance.team2Entry[0] = 3;
                photonView.RPC("SendMasterRoomPosition", RpcTarget.MasterClient, NetworkManager.Instance.team2Entry[0]);
            }
            else if (NetworkManager.Instance.team2Entry[0] == default && NetworkManager.Instance.team2Entry[1] != default)
            {
                NetworkManager.Instance.team2Entry[0] = 3;
                photonView.RPC("SendMasterRoomPosition", RpcTarget.MasterClient, NetworkManager.Instance.team2Entry[0]);
            }
            else if (NetworkManager.Instance.team2Entry[0] != default && NetworkManager.Instance.team2Entry[1] == default)
            {
                NetworkManager.Instance.team2Entry[1] = 4;
                photonView.RPC("SendMasterRoomPosition", RpcTarget.MasterClient, NetworkManager.Instance.team2Entry[1]);
            }
            else if (NetworkManager.Instance.team2Entry[0] != default && NetworkManager.Instance.team2Entry[1] != default)
            {
                /*Do Nothing*/
            }
        }
    }
    #endregion

    #region 시작 버튼
    public void PressStartReadyButton() 
    {

    }
    #endregion

    //Button Functions ====================================================================================
    #endregion
}