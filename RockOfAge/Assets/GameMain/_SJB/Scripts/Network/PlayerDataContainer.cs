using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;


public class PlayerDataContainer : MonoBehaviourPun, IPunObservable
{
    public int otherPlayerReady;
    public string Player1Name;
    public string Player2Name;
    public string Player3Name;
    public string Player4Name;

    public string Player1ViewID;
    public string Player2ViewID;
    public string Player3ViewID;
    public string Player4ViewID;

    public string Player1Num;
    public string Player2Num;
    public string Player3Num;
    public string Player4Num;

    



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) 
    {
        
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        otherPlayerReady = default;

        // 만약 master 의 dataContainer 라면
        if (PhotonNetwork.IsMasterClient == true) 
        {
            
        }
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        if(SceneManager.GetActiveScene().name == NetworkManager.Instance.GameScene)
        {
            SavePlayerNames();
            SaveViewID();
            SaveNumberAndTeam();
        }
    }
    #endregion

    public void SavePlayerNames() 
    {
        Player1Name = PhotonNetwork.CurrentRoom.CustomProperties["Player1"].ToString();
        Player2Name = PhotonNetwork.CurrentRoom.CustomProperties["Player2"].ToString();
        Player3Name = PhotonNetwork.CurrentRoom.CustomProperties["Player3"].ToString();
        Player4Name = PhotonNetwork.CurrentRoom.CustomProperties["Player4"].ToString();
    }

    public void SaveViewID() 
    {
        Player1ViewID = PhotonNetwork.CurrentRoom.CustomProperties[Player1Name].ToString();
        Player2ViewID = PhotonNetwork.CurrentRoom.CustomProperties[Player2Name].ToString();
        Player3ViewID = PhotonNetwork.CurrentRoom.CustomProperties[Player3Name].ToString();
        Player4ViewID = PhotonNetwork.CurrentRoom.CustomProperties[Player4Name].ToString();
    }

    public void SaveNumberAndTeam() 
    {
        Player1Num = PhotonNetwork.CurrentRoom.CustomProperties[Player1ViewID].ToString();
        Player2Num = PhotonNetwork.CurrentRoom.CustomProperties[Player2ViewID].ToString();
        Player3Num = PhotonNetwork.CurrentRoom.CustomProperties[Player3ViewID].ToString();
        Player4Num = PhotonNetwork.CurrentRoom.CustomProperties[Player4ViewID].ToString();
    }

    public void ResetPlayerTeamAndNumber(int player1ViewID, int player2ViewID, int player3ViewID, int player4ViewID)
    {
        // 플레이어 자리 위치 저장 값 초기화
        for (int i = 0; i < NetworkManager.Instance.playerSeats.Length; i++) 
        {
            NetworkManager.Instance.playerSeats[i] = false;
        }

        // 플레이어 아이디정보 초기화

        // 플레이어 정보 초기화

        // 플레이어 팀 정보 초기화
    }
    
    #region master client 가 start button 을 눌렀을 때 loadlevel 하는 메서드
    [PunRPC]
    public void StartGame() 
    {   
        // 로드할 씬의 이름 작성
        PhotonNetwork.LoadLevel("0921");
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }
    #endregion
}
