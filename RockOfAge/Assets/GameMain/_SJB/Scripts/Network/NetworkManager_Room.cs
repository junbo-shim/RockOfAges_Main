using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public partial class NetworkManager : GlobalSingleton<NetworkManager>
{
    //Room =================================================================================================

    #region 필드
    // Canvas_TitleAndLobby : Panel_Room
    public Transform RoomPanel { get; private set; }

    // 방 이름 표시하는 TMP_Text
    public TMP_Text roomName;

    // 방에 들어와있는 플레이어들을 캐싱할 배열
    public Player[] playerRoomList;

    // Team 1 버튼 프리팹 (Blue)
    public GameObject Team1ButtonPrefab { get; private set; }

    // Team 2 버튼 프리팹 (Red)
    public GameObject Team2ButtonPrefab { get; private set; }

    // PlayerDataContainer 프리팹
    public GameObject DataContainerPrefab { get; private set; }
    public PlayerDataContainer myDataContainer;
    public PlayerDataContainer masterDataContainer;

    public ExitGames.Client.Photon.Hashtable roomSetting;
    public bool[] playerSeats;
    #endregion


    #region 룸-Photon

    // 방을 생성하면 호출되는 callback 메서드
    public override void OnCreatedRoom()
    {
        // master 가 처음 방을 생성했을 때 버튼 오브젝트를 네트워크로 생성하는 custom 메서드
        MakeTeamButtons();
    }


    // 방에 참여하면 호출되는 callback 메서드
    public override void OnJoinedRoom()
    {
        // 방의 이름을 표시하는 custom 메서드
        ShowRoomName();

        // ? 현재 방의 CustomProperties 를 roomSetting HashTable 에 캐싱한다
        roomSetting = PhotonNetwork.CurrentRoom.CustomProperties;


        // 방에 참여 시 (master 포함) PhotonView 가 달려있는 오브젝트를 하나씩 생성한다
        GameObject dataContainer =
            PhotonNetwork.Instantiate(DataContainerPrefab.name, Vector3.zero, Quaternion.identity);

        // 만약 생성한 오브젝트가 내 것이면
        if (dataContainer.GetComponent<PhotonView>().IsMine == true)
        {
            // myDataContainer 에 현재 오브젝트를 할당한다
            myDataContainer = dataContainer.GetComponent<PlayerDataContainer>();
        }
        // 
        if (PhotonNetwork.IsMasterClient == true)
        {
            masterDataContainer = myDataContainer;
        }
    }


    // 방에 사람이 나가면 호출되는 메서드
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 방의 이름을 표시하는 custom 메서드
        ShowRoomName();
    }


    // 방을 떠나면 호출되는 callback 메서드
    public override void OnLeftRoom()
    {
        // 방 세팅이 남아 있다면 초기화한다
        if (roomSetting != null)
        {
            roomSetting.Clear();
        }
        // PhotonView 오브젝트가 남아있다면 삭제한다
        if (myDataContainer != null)
        {
            PhotonNetwork.Destroy(myDataContainer.GetComponent<PhotonView>());
        }
    }


    // master 가 처음 방을 생성했을 때 버튼 오브젝트를 네트워크로 생성하는 custom 메서드
    public void MakeTeamButtons() 
    {
        // Vector3 를 지역변수로 잠깐 생성, 각각 버튼별 위치를 지정
        Vector3 Player1Team1 = new Vector3();
        Vector3 Player2Team1 = new Vector3();
        Vector3 Player3Team2 = new Vector3();
        Vector3 Player4Team2 = new Vector3();

        // Vector3 에 맞게 각각 버튼을 PhotonNetwork.Instantiate 해준다 - 모든 방 참여자가 공유
        //PhotonNetwork.Instantiate(Team1ButtonPrefab, Player1Team1, Quaternion.identity, );
        //PhotonNetwork.Instantiate(Team1ButtonPrefab, Player2Team1, Quaternion.identity, );
        //PhotonNetwork.Instantiate(Team2ButtonPrefab, Player3Team2, Quaternion.identity, );
        //PhotonNetwork.Instantiate(Team2ButtonPrefab, Player4Team2, Quaternion.identity, );
    }


    // 방의 이름을 표시하는 custom 메서드
    public void ShowRoomName() 
    {
        // master 이름을 저장
        string masterName = PhotonNetwork.MasterClient.NickName;
        // 현재 방 이름을 표시
        roomName.text = masterName + "의 방 : " + PhotonNetwork.CurrentRoom.Name;
    }

    #endregion

    //Room =================================================================================================
}
