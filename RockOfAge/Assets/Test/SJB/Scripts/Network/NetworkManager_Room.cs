using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public partial class NetworkManager : GlobalSingleton<NetworkManager>
{
    public Transform RoomPanel { get; private set; }
    public TMP_Text roomName;

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        if (PhotonNetwork.IsMasterClient == true) 
        {
            ButtonManager.Instance.roomReadyButton.enabled = false;
            ButtonManager.Instance.roomReadyButton.transform.SetAsFirstSibling();
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (PhotonNetwork.IsMasterClient == true)
        {
            ButtonManager.Instance.roomReadyButton.enabled = false;
            ButtonManager.Instance.roomReadyButton.transform.SetAsFirstSibling();
        }
        else
        {
            ButtonManager.Instance.roomStartButton.enabled = false;
            ButtonManager.Instance.roomStartButton.transform.SetAsFirstSibling();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnLeftRoom()
    {
        
    }

    private void LocatePlayer() 
    {
    
    }
}
