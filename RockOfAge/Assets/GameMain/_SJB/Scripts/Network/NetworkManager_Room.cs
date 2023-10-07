using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public partial class NetworkManager : GlobalSingleton<NetworkManager>
{
    //Room =================================================================================================

    #region 필드
    // Canvas_TitleAndLobby : Panel_Room
    public Transform RoomPanel { get; private set; }

    // 방 이름 표시하는 TMP_Text
    public TMP_Text roomName;

    // 
    public List<PlayerDataContainer> dataContainers;

    // PlayerDataContainer 프리팹
    public GameObject DataContainerPrefab { get; private set; }

    // 생성된 나의 DataContainer
    public GameObject myDataContainer;

    // 모든 플레이어 Ready Check 용 int 변수
    public int readyCount;
    #endregion


    #region 룸-Photon

    // 방에 참여하면 호출되는 callback 메서드
    public override void OnJoinedRoom()
    {
        // 방에 참여 시 (master 포함) PhotonView 가 달려있는 오브젝트를 하나씩 생성한다
        GameObject dataContainer =
            PhotonNetwork.Instantiate(DataContainerPrefab.name, Vector3.zero, Quaternion.identity);

        // 만약 생성한 오브젝트가 내 것이면
        if (dataContainer.GetComponent<PhotonView>().IsMine == true)
        {
            // myDataContainer 에 나의 dataContainer GameObject 를 담아둔다
            myDataContainer = dataContainer;
        }

        // 방의 이름을 표시하는 custom 메서드
        if (PhotonNetwork.IsMasterClient == true) 
        {
            myDataContainer.GetComponent<PhotonView>().RPC("SendMasterName", 
                RpcTarget.All, myDataContainer.GetComponent<PlayerDataContainer>().PlayerName);
        }
    }


    // 방을 떠나면 호출되는 callback 메서드
    public override void OnLeftRoom()
    {
        // 방의 이름을 표시하는 custom 메서드
        if (PhotonNetwork.IsMasterClient == true)
        {
            myDataContainer.GetComponent<PhotonView>().RPC("SendMasterName",
                RpcTarget.All, myDataContainer.GetComponent<PlayerDataContainer>().PlayerName);
        }
    }

    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        // 방의 이름을 표시하는 custom 메서드
        if (PhotonNetwork.IsMasterClient == true)
        {
            myDataContainer.GetComponent<PhotonView>().RPC("SendMasterName",
                RpcTarget.All, myDataContainer.GetComponent<PlayerDataContainer>().PlayerName);
        }
    }

    // 방의 사람이 나가면 호출되는 callback 메서드
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 방의 이름을 표시하는 custom 메서드
        if (PhotonNetwork.IsMasterClient == true)
        {
            myDataContainer.GetComponent<PhotonView>().RPC("SendMasterName",
                RpcTarget.All, myDataContainer.GetComponent<PlayerDataContainer>().PlayerName);
        }
        // 버튼에 저장된 정보 초기화 메서드
        ResetAllData();
    }


    // 나말고 플레이어가 방을 떠났을 경우 버튼에서 받아온 모든 데이터 (Identifier 등)를 모두 초기화하는 custom 메서드
    public void ResetAllData() 
    {
        ButtonManager.Instance.player1Button.interactable = true;
        ButtonManager.Instance.player1Button.GetComponent<TeamButton>().playerIdentifier = -1;
        ButtonManager.Instance.player1Button.GetComponent<TeamButton>().playerName.text = null;
        ButtonManager.Instance.player1Button.GetComponent<TeamButton>().readyCheck.enabled = false;

        ButtonManager.Instance.player2Button.interactable = true;
        ButtonManager.Instance.player2Button.GetComponent<TeamButton>().playerIdentifier = -1;
        ButtonManager.Instance.player2Button.GetComponent<TeamButton>().playerName.text = null;
        ButtonManager.Instance.player2Button.GetComponent<TeamButton>().readyCheck.enabled = false;

        ButtonManager.Instance.player3Button.interactable = true;
        ButtonManager.Instance.player3Button.GetComponent<TeamButton>().playerIdentifier = -1;
        ButtonManager.Instance.player3Button.GetComponent<TeamButton>().playerName.text = null;
        ButtonManager.Instance.player3Button.GetComponent<TeamButton>().readyCheck.enabled = false;

        ButtonManager.Instance.player4Button.interactable = true;
        ButtonManager.Instance.player4Button.GetComponent<TeamButton>().playerIdentifier = -1;
        ButtonManager.Instance.player4Button.GetComponent<TeamButton>().playerName.text = null;
        ButtonManager.Instance.player4Button.GetComponent<TeamButton>().readyCheck.enabled = false;

        readyCount = default;

        PhotonNetwork.CurrentRoom.CustomProperties.Clear();
    }
    #endregion

    //Room =================================================================================================
}
