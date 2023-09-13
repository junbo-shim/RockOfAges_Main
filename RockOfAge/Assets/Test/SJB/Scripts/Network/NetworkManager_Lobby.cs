using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public partial class NetworkManager : GlobalSingleton<NetworkManager>
{
    //Lobby =================================================================================================

    public Transform LobbyPanel { get; private set; }
    public Transform RoomListContent { get; private set; }
    private Transform roomInfo;
    public Transform CreateRoomPopup { get; private set; }
    public Transform JoinLockedRoomPopup { get; private set; }
    public Transform WaitPopup { get; private set; }
    private TMP_Text playerName;
    private TMP_Text playerLobbyNumbers;

    public List<GameObject> roomLists;


    protected override void Update()
    {
        
    }

    #region ·Îºñ-Photon
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        TitlePanel.localScale = Vector3.zero;
        LobbyPanel.localScale = Vector3.one;
        Debug.Log("Join Success");

        playerName.text = PlayerPrefs.GetString("name");
        playerLobbyNumbers.text = PhotonNetwork.CountOfPlayersOnMaster.ToString() + "/20";
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
    }

    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        base.OnLobbyStatisticsUpdate(lobbyStatistics);
    }
    #endregion

    private void ShowRooms() 
    {
        
    }

    private void ShowPlayerNumbers() 
    {
        
    }

    //Lobby =================================================================================================
}
