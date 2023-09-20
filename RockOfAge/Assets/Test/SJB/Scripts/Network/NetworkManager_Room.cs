using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;

public partial class NetworkManager : GlobalSingleton<NetworkManager>
{
    public Transform RoomPanel { get; private set; }
    public TMP_Text roomName;
    public Player[] playerRoomList;

    public GameObject DataContainerPrefab { get; private set; }
    public PlayerDataContainer myDataContainer;
    public PlayerDataContainer masterDataContainer;

    public ExitGames.Client.Photon.Hashtable roomSetting;
    public bool[] playerSeats;

    protected override void Update()
    {
    }


    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        Debug.Log("방 생성 완료");
        Debug.Log("저는 마스터 클라이언트 입니다.");
        roomSetting = PhotonNetwork.CurrentRoom.CustomProperties;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        GameObject dataContainer =
            PhotonNetwork.Instantiate(NetworkManager.Instance.DataContainerPrefab.name, Vector3.zero, Quaternion.identity);
        Debug.Log(dataContainer.GetComponent<PhotonView>().ViewID);
        Debug.Log(dataContainer.GetComponent<PhotonView>().IsMine);

        if (dataContainer.GetComponent<PhotonView>().IsMine == true) 
        {
            myDataContainer = dataContainer.GetComponent<PlayerDataContainer>();
        }
        if (PhotonNetwork.IsMasterClient == true) 
        {
            masterDataContainer = myDataContainer;
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        roomSetting.Clear();
    }
}
