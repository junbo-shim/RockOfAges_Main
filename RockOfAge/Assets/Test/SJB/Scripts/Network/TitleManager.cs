using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviourPunCallbacks
{
    // 서버에 접속한 인원을 저장할 변수
    private int connectedPlayerNumbers;
    // 서버에 접속한 플레이어의 이름을 저장할 리스트
    public static List<string> playerNames;
    // 서버 접속 상태
    private TMP_Text serverStatus;

    private TMP_InputField playerNameSpace;
    private Button optionButton;
    private Button startButton;
    private Button quitButton;
    

    private void Awake()
    {
        playerNames = new List<string>();
        serverStatus = GameObject.Find("ServerStatus").GetComponentInChildren<TMP_Text>();

        playerNameSpace = GameObject.Find("NameSpace").GetComponent<TMP_InputField>();
        optionButton = GameObject.Find("Button_Option").GetComponent<Button>();
        startButton = GameObject.Find("Button_Start").GetComponent<Button>();
        quitButton = GameObject.Find("Button_Quit").GetComponent<Button>();
    }

    void Start()
    {
#if PhotonSymbol
        ConnectMasterServer();
#endif
    }

    void Update()
    {
#if PhotonSymbol
        UpdateMasterServerStatus();
#endif
    }


    #region 포톤 : 마스터서버 연결관련 메서드
#if PhotonSymbol
    // 마스터 서버에 연결하는 커스텀 메서드
    private void ConnectMasterServer() 
    {
        PhotonNetwork.ConnectUsingSettings();
        startButton.interactable = false;
        serverStatus.text = "Connecting to Master Server";
        serverStatus.color = Color.yellow;
    }

    // 마스터 서버의 상태를 업데이트 해서 text 로 출력하는 커스텀 메서드
    private void UpdateMasterServerStatus()
    {
        if (connectedPlayerNumbers != PhotonNetwork.CountOfPlayersOnMaster) 
        {
            OnConnectedToMaster();
        }
    }

    // 마스터 서버와 연결될 경우 호출되는 메서드
    public override void OnConnectedToMaster()
    {
        connectedPlayerNumbers = PhotonNetwork.CountOfPlayersOnMaster;

        startButton.interactable = true;
        serverStatus.text = "Online\nNumber of Connected Users : " + connectedPlayerNumbers.ToString();
        serverStatus.color = Color.green;
    }

    // 마스터 서버와 연결이 끊어질 경우 호출되는 메서드
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogFormat("마스터서버와의 연결 해제\n원인 : {0}", cause);

        startButton.interactable = false;
        serverStatus.text = "Offline\nConnecting to Master Server...";
        serverStatus.color = Color.red;
        PhotonNetwork.ConnectUsingSettings();
    }
#endif
    #endregion


    #region 포톤 : 플레이어 이름 비교 및 저장
    private void UpdatePlayerName() 
    {
        
    }

    private void CheckPlayerName() 
    {
        //if(playerNameSpace.text.Length > 2) 
        //{
        //    startButton.interactable = true;

        //    foreach () 
        //    {
        //        if (playerNameSpace.text != ) 
        //        {
        //            PhotonNetwork.CountOfPlayers
        //        }
            
        //    }
        //}
    }

    private void RecordPlayerName() 
    {
        PhotonNetwork.NickName = playerNameSpace.text;


    }
    #endregion


    #region 포톤 : 게임시작-로비이동 버튼
    private void GoToLobby() 
    {
    
    }
    #endregion

    #region 게임 종료 버튼
    private void QuitGame() 
    {
    
    }
    #endregion

    #region 옵션 버튼
    private void OpenOptionWindow() 
    {
    
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    public override void OnConnected()
    {
        base.OnConnected();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
    }

    public override void OnRegionListReceived(RegionHandler regionHandler)
    {
        base.OnRegionListReceived(regionHandler);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        base.OnFriendListUpdate(friendList);
    }

    public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        base.OnCustomAuthenticationResponse(data);
    }

    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
        base.OnCustomAuthenticationFailed(debugMessage);
    }

    public override void OnWebRpcResponse(OperationResponse response)
    {
        base.OnWebRpcResponse(response);
    }

    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        base.OnLobbyStatisticsUpdate(lobbyStatistics);
    }

    public override void OnErrorInfo(ErrorInfo errorInfo)
    {
        base.OnErrorInfo(errorInfo);
    }
    #endregion
}
