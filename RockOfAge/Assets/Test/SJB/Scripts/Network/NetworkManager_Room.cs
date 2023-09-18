using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;

public partial class NetworkManager : GlobalSingleton<NetworkManager>
{
    public Transform RoomPanel { get; private set; }
    public TMP_Text roomName;
    public Player[] playerRoomList;

    public ExitGames.Client.Photon.Hashtable roomSetting;
    public int[] team1Entry;
    public int[] team2Entry;

    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) 
        {
            foreach (var player in roomSetting) 
            {
                Debug.Log(player.Key);
                Debug.Log(player.Value);
            }
        }
    }


    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("방 생성 완료");

        if(PhotonNetwork.IsMasterClient == true) 
        {
            Debug.Log("저는 마스터 클라이언트 입니다.");
        }

        roomSetting = new ExitGames.Client.Photon.Hashtable { };
    }

    //public override void OnJoinedRoom()
    //{
    //    if (PhotonNetwork.IsMasterClient == true)
    //    {
    //        ButtonManager.Instance.roomReadyButton.enabled = false;
    //        ButtonManager.Instance.roomReadyButton.transform.SetAsFirstSibling();
    //    }
    //    else
    //    {
    //        ButtonManager.Instance.roomStartButton.enabled = false;
    //        ButtonManager.Instance.roomStartButton.transform.SetAsFirstSibling();
    //    }

    //    playerRoomList = PhotonNetwork.PlayerList;
    //    Debug.Log(playerRoomList[0]);
    //    Debug.Log(playerRoomList[1]);
    //    Debug.Log(playerRoomList[2]);

    //    ExitGames.Client.Photon.Hashtable playerProperty = 
    //        new ExitGames.Client.Photon.Hashtable { { "Team", teamNumberInProperty } };
    //    PhotonNetwork.SetPlayerCustomProperties(playerProperty);
    //}

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("guest 들어옴");
    }

    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        Debug.Log("guest 나감");
    }

    [PunRPC]
    private void SendMasterRoomPosition(int positionInfo) 
    {
        if (PhotonNetwork.IsMasterClient == true) 
        {
            if (positionInfo == 1)
            {
                roomSetting[photonView.ViewID] = "player1";
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomSetting);
            }
            else if (positionInfo == 2)
            {
                roomSetting[photonView.ViewID] = "player2";
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomSetting);
            }
            else if (positionInfo == 3)
            {
                roomSetting[photonView.ViewID] = "player3";
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomSetting);
            }
            else if (positionInfo == 4)
            {
                roomSetting[photonView.ViewID] = "player4";
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomSetting);
            }
        }
        else
        {
            /*Do Nothing*/
        }
    }

    [PunRPC]
    private void MovePlayer() 
    {
        
    }
}
