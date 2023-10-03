using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;


public class TeamButton : MonoBehaviourPun, IPunObservable
{
    // 팀 선택 버튼에 표시될 플레이어의 이름
    public TMP_Text playerName;

    // 팀 선택 버튼에 표시될 플레이어의 초상화
    public Image playerSprite;

    // 팀 선택 버튼에 표시될 플레이어의 레디 확인용 체크 이미지
    public Image readyCheck;

    // 이 버튼을 누른 사람의 아이디 변수
    public int playerIdentifier;



    // 실시간으로 공유될 (master 가 생성한 원본과 다른 client 들이 가진 복제본이 공유하는)
    // 변수 전송용 photon 동기화 메서드
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerIdentifier);
            stream.SendNext(playerName.text);
            stream.SendNext(readyCheck.enabled);
        }
        else
        {
            playerIdentifier = (int)stream.ReceiveNext();
            playerName.text = (string)stream.ReceiveNext();
            readyCheck.enabled = (bool)stream.ReceiveNext();
        }
    }


    private void Awake()
    {
        // 이 gameObject 가 가진 button 컴포넌트에 접근하여 AddListener 로 메서드 할당
        gameObject.GetComponent<Button>().onClick.AddListener(PressPlayerSeat);

        // 각 변수에 이 스크립트가 달린 gameObject 하위 요소들을 찾아서 할당
        playerName = gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        playerSprite = gameObject.transform.Find("Image").GetComponent<Image>();
        readyCheck = gameObject.transform.Find("Check").GetComponent<Image>();

        // 체크 이미지는 꺼둔다
        readyCheck.enabled = false;
        // playerIdentifier 의 초기값은 -1
        playerIdentifier = -1;
    }


    // 버튼을 눌렀을 때 호출되는 custom 메서드
    public void PressPlayerSeat()
    {
        // 만약 playerIdentifier 에 누가 눌렀는지 정보가 없다면
        if (playerIdentifier == -1)
        {
            // 내 ActorNumber 를 담아서 RPC 메서드를 모두에게 쏜다
            photonView.RPC("CheckIdentifierNone", RpcTarget.All,
                PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.LocalPlayer.NickName);
        }
        // 만약 playerIdentifier 누군가 눌렀는지 정보가 담겨있다면
        else if (playerIdentifier != -1)
        {
            // 그리고 그 playerIdentifier 가 내 ActorNumber 라면
            if (playerIdentifier == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                // 내 ActorNumber 를 담아서 RPC 메서드를 모두에게 쏜다
                photonView.RPC("CheckIdentifierSomething", RpcTarget.All,
                    PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }

    #region playerIdentifier 값이 비어 있을 경우
    // playerIdentifier 값이 비어 있을 경우에 모두에게 누른 사람의 ActorNumber 송신하는 RPC custom 메서드
    [PunRPC]
    public void CheckIdentifierNone(int myActorNum, string myName)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            /*Do Nothing*/
        }
        else if (PhotonNetwork.IsMasterClient == true)
        {
            // 버튼의 playerIdentifier 변수에 myActorNum 를 담는다
            playerIdentifier = myActorNum;
            // 버튼의 playerName 에 플레이어 이름 표시
            playerName.text = myName;
            // 버튼의 Ready 상태를 true 로 변환
            readyCheck.enabled = true;
            // 모든 사람들에게 interactable false 로 만드는 RPC 발사
            photonView.RPC("TurnButtonFalse", RpcTarget.All, myActorNum);
        }
    }


    // 다른 사람들에게 interactable false 로 만드는 RPC custom 메서드
    [PunRPC]
    public void TurnButtonFalse(int myActorNum)
    {
        // 내가 버튼을 누른 사람이라면
        if (myActorNum == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            /*Do Nothing*/
        }
        else
        {
            gameObject.GetComponent<Button>().interactable = false;
        }
    }
    #endregion


    #region playerIdentifier 값이 차있을 경우
    // playerIdentifier 값이 차있을 경우에 모두에게 누른 사람의 ActorNumber 송신하는 RPC custom 메서드
    [PunRPC]
    public void CheckIdentifierSomething(int myActorNum)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            /*Do Nothing*/
        }
        else if (PhotonNetwork.IsMasterClient == true)
        {
            // 버튼의 playerIdentifier 변수를 -1 로 초기화한다
            playerIdentifier = -1;
            // 버튼의 playerName 에 플레이어 이름 제거
            playerName.text = null;
            // 버튼의 Ready 상태를 false 로 변환
            readyCheck.enabled = false;
            // 모든 사람들에게 interactable true 로 만드는 RPC 발사
            photonView.RPC("TurnButtonTrue", RpcTarget.All, myActorNum);
        }
    }


    // 다른 사람들에게 interactable true 로 만드는 RPC custom 메서드
    [PunRPC]
    public void TurnButtonTrue(int myActorNum)
    {
        // 내가 버튼을 누른 사람이라면
        if (myActorNum == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            /*Do Nothing*/
        }
        else
        {
            gameObject.GetComponent<Button>().interactable = true;
        }
    }
    #endregion
}
