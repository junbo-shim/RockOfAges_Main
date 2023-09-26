using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

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
        roomSetting = PhotonNetwork.CurrentRoom.CustomProperties;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        GameObject dataContainer =
            PhotonNetwork.Instantiate(NetworkManager.Instance.DataContainerPrefab.name, Vector3.zero, Quaternion.identity);

        if (dataContainer.GetComponent<PhotonView>().IsMine == true) 
        {
            myDataContainer = dataContainer.GetComponent<PlayerDataContainer>();
        }
        if (PhotonNetwork.IsMasterClient == true) 
        {
            masterDataContainer = myDataContainer;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        if (roomSetting != null) 
        {
            roomSetting.Clear();
        }

        if (myDataContainer != null) 
        {
            PhotonNetwork.Destroy(myDataContainer.GetComponent<PhotonView>());
        }
    }
}
