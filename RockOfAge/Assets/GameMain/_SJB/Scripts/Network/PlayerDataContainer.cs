using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;

public class PlayerDataContainer : MonoBehaviourPun, IPunObservable
{
    #region 필드
    public string PlayerName;
    public string PlayerViewID;
    public string ViewIDActorNum;
    public string PlayerTeamNum;

    public float playerGold;
    #endregion


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) 
    {
        if (stream.IsWriting)
        {
            stream.SendNext(PlayerName);
            stream.SendNext(PlayerViewID);
            stream.SendNext(ViewIDActorNum);
            stream.SendNext(PlayerTeamNum);
        }
        else
        {
            PlayerName = (string)stream.ReceiveNext();
            PlayerViewID = (string)stream.ReceiveNext();
            ViewIDActorNum = (string)stream.ReceiveNext();
            PlayerTeamNum = (string)stream.ReceiveNext();
        }
    }


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        PlayerName = PhotonNetwork.NickName;
        PlayerViewID = photonView.ViewID.ToString();
        ViewIDActorNum = photonView.OwnerActorNr.ToString();
        gameObject.tag = "DataContainer";
    }


    #region 씬 변화시 callback
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    // Scene 이 로드될 때 호출되는 callback
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        // 만약 로드된 Scene 이 GameScene 이라면
        if (SceneManager.GetActiveScene().name == NetworkManager.Instance.GameScene)
        {
            foreach(var tagItem in GameObject.FindGameObjectsWithTag("DataContainer"))
            {
                NetworkManager.Instance.dataContainers.Add(tagItem.GetComponent<PlayerDataContainer>());
            }
        }
    }
    #endregion


    // 플레이어 데이터를 저장하는 RPC custom 메서드
    [PunRPC]
    public void SavePlayerData() 
    {
        // 만약 photonView 가 달린 object 의 조작권이 나에게 있다면
        if (photonView.IsMine == true) 
        {
            // custom room property 저장을 위한 hashtable 을 할당한다
            ExitGames.Client.Photon.Hashtable roomSetting = PhotonNetwork.CurrentRoom.CustomProperties;

            // Key : ViewIDActorNum (방 내부 고유값) / Value : PlayerViewID (방 내부 고유값) 을 저장한다
            roomSetting.Add(ViewIDActorNum, PlayerViewID);

            // 버튼에 담긴 ActorNumber 값을 지역변수로 할당한다
            int player1Number = ButtonManager.Instance.player1Button.GetComponent<TeamButton>().playerIdentifier;
            int player2Number = ButtonManager.Instance.player2Button.GetComponent<TeamButton>().playerIdentifier;
            int player3Number = ButtonManager.Instance.player3Button.GetComponent<TeamButton>().playerIdentifier;
            int player4Number = ButtonManager.Instance.player4Button.GetComponent<TeamButton>().playerIdentifier;

            // 만약 나의 ActorNum 과 저장된 ActorNum 이 같다면
            if (ViewIDActorNum == player1Number.ToString()) 
            {
                //  Key : PlayerViewID (방 내부 고유값) / Value : Player1_Team1 을 저장한다
                roomSetting.Add(PlayerViewID, ButtonManager.Instance.player1Button.name);
            }
            else if (ViewIDActorNum == player2Number.ToString()) 
            {
                //  Key : PlayerViewID (방 내부 고유값) / Value : Player2_Team1 을 저장한다
                roomSetting.Add(PlayerViewID, ButtonManager.Instance.player2Button.name);
            }
            else if (ViewIDActorNum == player3Number.ToString())
            {
                //  Key : PlayerViewID (방 내부 고유값) / Value : Player3_Team2 을 저장한다
                roomSetting.Add(PlayerViewID, ButtonManager.Instance.player3Button.name);
            }
            else if (ViewIDActorNum == player4Number.ToString())
            {
                //  Key : PlayerViewID (방 내부 고유값) / Value : Player4_Team2 을 저장한다
                roomSetting.Add(PlayerViewID, ButtonManager.Instance.player4Button.name);
            }

            // PlayerTeamNum 에 Player'n'_Team'n' 값을 저장한다
            //PlayerTeamNum = PhotonNetwork.CurrentRoom.CustomProperties[PlayerViewID].ToString();
        }
    }

    #region master client 가 start button 을 눌렀을 때 loadlevel 하는 메서드
    [PunRPC]
    public void StartGame() 
    {
        // 데이터를 저장한다
        photonView.RPC("SavePlayerData", RpcTarget.All);

        // 로드할 씬의 이름 작성
        PhotonNetwork.LoadLevel("0921");
        // 현재 방은 닫힘 상태로 만든다
        PhotonNetwork.CurrentRoom.IsVisible = false;
    }
    #endregion
}
