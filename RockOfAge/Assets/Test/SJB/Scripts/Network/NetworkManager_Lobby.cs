using TMPro;
using UnityEngine;

public partial class NetworkManager : GlobalSingleton<NetworkManager>
{
    //Lobby =================================================================================================

    public Transform LobbyPanel { get; private set; }
    private Transform roomListContent;
    private Transform roomInfo;
    public Transform CreateRoomPopup { get; private set; }
    public Transform JoinLockedRoomPopup { get; private set; }
    public Transform WaitPopup { get; private set; }
    private TMP_Text playerName;
    private TMP_Text playerNumbers;


    protected override void Update()
    {

    }

    //Lobby =================================================================================================
}
