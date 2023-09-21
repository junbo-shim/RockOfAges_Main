using UnityEngine;
using System.Collections.Generic;
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
    //private TMP_Text playerName;
    //private TMP_Text playerLobbyNumbers;

    // 캐싱한 룸 리스트
    public List<RoomInfo> cachedRoomList;
    // 실제 생성되는 룸 리스트
    public List<GameObject> displayRoomList;

    #region 로비-Photon
    // 로비에 들어오면 호출되는 메서드
    public override void OnJoinedLobby()
    {
        TitlePanel.localScale = Vector3.zero;
        LobbyPanel.localScale = Vector3.one;
        Debug.Log("Join Success");
        
        
        //playerName.text = PlayerPrefs.GetString("name");
        //playerLobbyNumbers.text = PhotonNetwork.CountOfPlayersOnMaster.ToString() + "/20";
    }
    // 로비를 떠날시 호출되는 메서드
    public override void OnLeftLobby()
    {
        
    }
    // 로비에 있는 Room 이 생성되거나 파괴될 시 호출되는 메서드
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        CacheServerRoomList(roomList);
        UpdateRoomDisplay();
    }
    // OnRoomListUpdate 의 List 를 클라이언트 변수에 캐싱하는 메서드
    private void CacheServerRoomList(List<RoomInfo> roomList) 
    {
    }
    // 방 UI 에 대한 변화를 반영하는 메서드
    public void UpdateRoomDisplay()
    {
        #region Legacy 2
        //if (RoomListContent.childCount == 0) 
        //{
        //    /*Do Nothing*/
        //}
        //else if (RoomListContent.childCount > 0) 
        //{

        //for (int i = 0; i < RoomListContent.childCount; i++) 
        //{
        //    Destroy(displayRoomList[i]);
        //}
        //}

        //if (cachedRoomList.Count > 0) 
        //{
        //    for (int i = 0; i < cachedRoomList.Count; i++)
        //    {
        //        GameObject roomUI = Instantiate(ButtonManager.Instance.roomPrefab, RoomListContent);
        //        displayRoomList.Add(roomUI);

        //        displayRoomList[i].transform.GetChild(0).GetComponent<TMP_Text>().text =
        //            cachedRoomList[i].Name;
        //        displayRoomList[i].transform.GetChild(1).GetComponent<TMP_Text>().text =
        //            cachedRoomList[i].PlayerCount + "/4";
        //    }
        //    Debug.Log(RoomListContent.childCount);
        //}
        #endregion

        #region Legacy
        //if (RoomListContent.childCount >= 0)
        //{
        //    for (int i = 0; i < RoomListContent.childCount; i++)
        //    {
        //        Destroy(displayRoomList[i]);
        //    }

        //    cachedRoomList.Clear();
        //    cachedRoomList = roomList;

        //    for (int i = 0; i < cachedRoomList.Count; i++)
        //    {
        //        Debug.Log(displayRoomList);
        //        GameObject newRoom = Instantiate(ButtonManager.Instance.roomPrefab, RoomListContent);
        //        displayRoomList.Add(newRoom);
        //    }
        //}
        #endregion
    }
    #endregion

    //Lobby =================================================================================================
}
