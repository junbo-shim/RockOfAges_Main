using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public partial class NetworkManager : GlobalSingleton<NetworkManager>
{
    //Lobby =================================================================================================

    #region 필드
    // Canvas_TitleAndLobby : Panel_Lobby
    public Transform LobbyPanel { get; private set; }

    // 방 버튼 오브젝트
    public GameObject OriginalRoomObject { get; private set; }

    // 방 버튼을 표시할 scroll view
    public Transform RoomListContent { get; private set; }

    // 방 그림을 표시할 부분
    private Transform roomInfo;

    // 방 생성 버튼 클릭 시 나타나는 창
    public Transform CreateRoomPopup { get; private set; }

    // 대기 요청 창
    public Transform WaitPopup { get; private set; }

    // 캐싱한 룸 리스트 (Key : RoomInfo.Name / Value : RoomInfo)
    public Dictionary<string, RoomInfo> cachedRoomDictionary;

    // 로비의 Player 이름 표시하는 TMP_Text
    public TMP_Text playerLobbyName;

    // 로비의 접속한 Player 수 표시하는 TMP_Text
    public TMP_Text playerLobbyNumbers;
    #endregion


    #region 로비-Photon

    // 로비에 들어오면 호출되는 callback 메서드
    public override void OnJoinedLobby()
    {
        // Title UI 를 작게 만들고 Lobby UI 를 띄운다
        TitlePanel.localScale = Vector3.zero;
        LobbyPanel.localScale = Vector3.one;
        Debug.Log("Join Success");

        // 로비에 들어올 때마다 기존 방 목록 UI 를 지워주고 새로 리스트를 캐싱하고 다시 UI 를 생성한다
        DeleteRoomDisplay(RoomListContent);
        cachedRoomDictionary.Clear();
        UpdateRoomDisplay();

        // 플레이어의 이름 표시
        playerLobbyName.text = playerNickName;
    }


    // 로비를 떠날시 호출되는 callback 메서드 (방을 생성하거나, 방에 참여 시)
    public override void OnLeftLobby()
    {
        // 로비를 떠날 때마다 기존 방 목록 UI 를 지워주고 새로 리스트를 캐싱하고 다시 UI 를 생성한다
        DeleteRoomDisplay(RoomListContent);
        cachedRoomDictionary.Clear();
        //UpdateRoomDisplay();
    }
    #endregion


    #region 로비-RoomList

    // 로비에 있는 room 목록에 변동사항이 생길 때 호출되는 callback 메서드 (로비에 참여, 방 생성, 방 제거 시 변화)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 방 UI 를 삭제하는 custom 메서드
        DeleteRoomDisplay(RoomListContent);

        // cachedRoomDictionary 을 업데이트 해주는 custom 메서드
        UpdateCachedRoomDictionary(roomList);

        // 방 UI 를 새로 출력하는 custom 메서드
        UpdateRoomDisplay();
    }


    // cachedRoomDictionary 을 업데이트 해주는 custom 메서드
    public void UpdateCachedRoomDictionary(List<RoomInfo> roomList) 
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo room = roomList[i];

            // 만약 roomList 안의 room 이 Photon 에서 제공하는 Lobby Room List 에서 사라졌을 경우
            if (room.RemovedFromList == true)
            {
                // cachedRoomDictionary 에서 제거한다
                cachedRoomDictionary.Remove(room.Name);
            }
            else
            {
                // cachedRoomDictionary 에 더한다
                cachedRoomDictionary[room.Name] = room;
            }
        }
    }


    // 방 UI 를 삭제하는 custom 메서드
    public void DeleteRoomDisplay(Transform roomContentList)
    {
        // cachedRoomList 에 아무 것도 없는 상태라면
        if (cachedRoomDictionary.Count == 0) 
        {
            /*Do Nothing*/
        }
        // cachedRoomList 에 무언가 저장되어 있던 상태라면
        else if (cachedRoomDictionary.Count > 0) 
        {
            // cachedRoomList 의 크기만큼 반복하여 roomContentList 하위에 생성된
            // 자식 오브젝트-room(버튼) 을 삭제한다
            for (int i = 0; i < cachedRoomDictionary.Count; i++) 
            {
                Destroy(roomContentList.GetChild(i).gameObject);
            }
        }
    }


    // 방 UI 를 새로 출력하는 custom 메서드
    public void UpdateRoomDisplay() 
    {
        // cachedRoomList 에 아무 것도 없는 상태라면
        if (cachedRoomDictionary.Count == 0)
        {
            /*Do Nothing*/
        }
        // cachedRoomList 에 무언가 저장되어 있던 상태라면
        else if (cachedRoomDictionary.Count > 0)
        {
            // cachedRoomList 를 모두 순회하여 roomContentList 하위에
            // room(버튼) 을 생성한다
            foreach (var cachedRoom in cachedRoomDictionary.Values)
            {
                GameObject roomButton = Instantiate(OriginalRoomObject, RoomListContent);
                // room(버튼) 에 RoomButton 스크립트를 넣는다
                roomButton.AddComponent<RoomButton>();
                // room(버튼) 의 thisRoomInfo 에 cachedRoom 정보를 담는다
                roomButton.GetComponent<RoomButton>().thisRoomInfo = cachedRoom;
            }
        }
    }
    #endregion

    //Lobby =================================================================================================
}
