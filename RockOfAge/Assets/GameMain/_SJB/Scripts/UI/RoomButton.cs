using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class RoomButton : MonoBehaviourPun
{
    // 각 클라이언트 별로 NetworkManager 에서 수동 업데이트하는 과정에서 담는 cachedRoom
    public RoomInfo thisRoomInfo;

    // 버튼에 표기할 cachedRoom 의 이름
    public TMP_Text displayName;
    // 버튼에 표기할 현재 플레이어 수 / 최대 플레이어 수
    public TMP_Text displayPlayers;

    // 이 클래스가 컴포넌트로 Add 될 때 호출
    void OnEnable()
    {
        // 이 버튼이 생성될 때 하위에 있는 TMP_Text 요소를 가져온다
        displayName = gameObject.transform.Find("Text_RoomName").GetComponent<TMP_Text>();
        displayPlayers = gameObject.transform.Find("Text_RoomPlayer").GetComponent<TMP_Text>();

        // 이 버튼이 생성될 때 PressRoomButton 메서드를 Listener 에 Add 해준다
        gameObject.GetComponent<Button>().onClick.AddListener(PressRoomButton);
    }

    void Update()
    {
        // 이름 값을 받아온다
        displayName.text = thisRoomInfo.Name;

        // 계속해서 플레이어 수 변화를 추적한다
        displayPlayers.text = thisRoomInfo.PlayerCount.ToString() + "/" + thisRoomInfo.MaxPlayers.ToString();
    }

    // 버튼을 눌렀을 시 호출되는 메서드
    public void PressRoomButton() 
    {
        // 이 버튼이 저장 중인 RoomInfo 의 이름으로 특정 방에 참여한다
        PhotonNetwork.JoinRoom(thisRoomInfo.Name);

        // Lobby 의 버튼들을 2초간 홀드
        ButtonManager.Instance.PauseLobbyButtons();
        Invoke("LoadRoomPopup", 2f);
    }

    // Invoke 를 사용하기 위해서 ButtonManager 하위 메서드를 래핑한 메서드
    private void LoadRoomPopup() 
    {
        ButtonManager.Instance.ResetLobbyButtons();
        ButtonManager.Instance.OpenRoomPanel();
    }
}
