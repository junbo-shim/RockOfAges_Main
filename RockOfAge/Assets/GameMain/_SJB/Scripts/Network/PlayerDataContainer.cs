using UnityEngine;
using Photon.Pun;

public class PlayerDataContainer : MonoBehaviourPun, IPunObservable
{
    public string playerID;
    public string playerName;

    public string roomName;

    public int otherPlayerReady;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        otherPlayerReady = default;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) 
    {
        
    }

    public void SavePlayerTeamAndNumber(int player1ViewID, int player2ViewID, int player3ViewID, int player4ViewID) 
    {

    }

    public void ResetPlayerTeamAndNumber(int player1ViewID, int player2ViewID, int player3ViewID, int player4ViewID) 
    {

    }
    
    #region player (boolIdx + 1) 자리 가겠다는 요청을 master client 에게 RPC 로 전달하는 메서드
    [PunRPC]
    public void SendPlayerPosition(int photonViewID, int boolIdx) // 포톤뷰ID 와 가고 싶은 자리의 인덱스를 매개변수로 받음
    {
        string inputKey = photonViewID.ToString();
        string seatName = "Player" + (boolIdx + 1);

        // 조건문 { 
        // 만약 player 자리가 차 있다면
        if (NetworkManager.Instance.playerSeats[boolIdx] == true)
        {
            Debug.Log("Seat Already Taken");
        }
        // 만약 player 자리가 비어있다면
        else if (NetworkManager.Instance.playerSeats[boolIdx] == false)
        {
            // roomSetting 해시테이블에 키로 photonViewID 가 존재하면
            if (NetworkManager.Instance.roomSetting.ContainsKey(inputKey))
            {
                string seatNumber = NetworkManager.Instance.roomSetting[inputKey].ToString();
                
                int index = int.Parse(seatNumber.Split("Player")[1]);
                Debug.Log(index);
                NetworkManager.Instance.playerSeats[index-1] = false;
                NetworkManager.Instance.roomSetting[inputKey] = seatName;
                Debug.LogFormat("View ID : {0} 가 Player{1} 에서 {2} 로 이동 했습니다.", photonViewID, index, seatName);
            }
            else
            {
                NetworkManager.Instance.roomSetting.Add(inputKey, seatName);
                Debug.LogFormat("View ID : {0} 가 {1} 로 이동 했습니다.", photonViewID, seatName);
            }
            PhotonNetwork.CurrentRoom.SetCustomProperties(NetworkManager.Instance.roomSetting);
            NetworkManager.Instance.playerSeats[boolIdx] = true;

            #region Legacy
            /* // 만약 roomSetting 해시테이블에 아무 정보도 저장되어있지 않다면 해시테이블에 값을 넣는다
             if (NetworkManager.Instance.roomSetting.Count == 0) 
             {
                 inputkey = photonViewID.ToString();
                 seatName = "player1";
             }
             // 만약 roomSetting 해시테이블에 어떤 정보라도 들어있다면
             else if (NetworkManager.Instance.roomSetting.Count != 0) 
             {
                 // 반복문 {
                 // roomSetting 의 Key 값들을 하나씩 찾아본다
                 foreach (var key in NetworkManager.Instance.roomSetting.Keys)
                 {
                     // 만약 key 중에서 photonViewID 와 같은 것이 없다면 그냥 해시테이블에 값을 넣는다
                     if (key.ToString() != photonViewID.ToString())
                     {

                         inputkey = photonViewID.ToString();
                         seatName = "player1";

                         Debug.Log(photonViewID);
                         Debug.Log(NetworkManager.Instance.roomSetting[photonViewID.ToString()]);
                         Debug.Log(NetworkManager.Instance.playerSeats[0]);
                     }
                     // 만약 key 중에서 photonViewID 와 같은 것이 있다면 기존 자리를 bool 을 초기화한다
                     else if (key.ToString() == photonViewID.ToString())
                     {
                         inputkey = photonViewID.ToString();
                         seatName = "player1";

                         Debug.Log(photonViewID);
                         Debug.Log(NetworkManager.Instance.roomSetting[photonViewID.ToString()]);
                         Debug.Log(NetworkManager.Instance.playerSeats[boolIdx]);
                         Debug.Log(NetworkManager.Instance.playerSeats[0]);
                     }
                 }
                 // 반복문 }
             }*/
            #endregion
        }
        #region Legacy2
        //// 조건문 { 
        //// 만약 player2 자리가 차 있다면
        //if (NetworkManager.Instance.playerSeats[1] == true)
        //{
        //    Debug.Log("Seat Already Taken");
        //}
        //// 만약 player2 자리가 비어있다면
        //else if (NetworkManager.Instance.playerSeats[1] == false)
        //{
        //    // 만약 roomSetting 해시테이블에 아무 정보도 저장되어있지 않다면 해시테이블에 값을 넣는다
        //    if (NetworkManager.Instance.roomSetting.Count == 0)
        //    {
        //        NetworkManager.Instance.roomSetting.Add(photonViewID.ToString(), "player2");
        //        PhotonNetwork.CurrentRoom.SetCustomProperties(NetworkManager.Instance.roomSetting);
        //        NetworkManager.Instance.playerSeats[1] = true;

        //        Debug.Log(photonViewID);
        //        Debug.Log(NetworkManager.Instance.roomSetting[photonViewID.ToString()]);
        //        Debug.Log(NetworkManager.Instance.playerSeats[1]);
        //    }
        //    // 만약 roomSetting 해시테이블에 어떤 정보라도 들어있다면
        //    else if (NetworkManager.Instance.roomSetting.Count != 0)
        //    {
        //        // 반복문 {
        //        // roomSetting 의 Key 값들을 하나씩 찾아본다
        //        foreach (var key in NetworkManager.Instance.roomSetting.Keys)
        //        {
        //            // 만약 key 중에서 photonViewID 와 같은 것이 없다면 그냥 해시테이블에 값을 넣는다
        //            if (key.ToString() != photonViewID.ToString())
        //            {
        //                NetworkManager.Instance.roomSetting[photonViewID.ToString()] = "player2";
        //                PhotonNetwork.CurrentRoom.SetCustomProperties(NetworkManager.Instance.roomSetting);
        //                NetworkManager.Instance.playerSeats[1] = true;

        //                Debug.Log(photonViewID);
        //                Debug.Log(NetworkManager.Instance.roomSetting[photonViewID.ToString()]);
        //                Debug.Log(NetworkManager.Instance.playerSeats[1]);
        //            }
        //            // 만약 key 중에서 photonViewID 와 같은 것이 있다면 기존 자리를 bool 을 초기화한다
        //            else if (key.ToString() == photonViewID.ToString())
        //            {
        //                //NetworkManager.Instance.roomSetting.Remove(photonViewID.ToString());
        //                NetworkManager.Instance.roomSetting[photonViewID.ToString()] = "player2";
        //                PhotonNetwork.CurrentRoom.SetCustomProperties(NetworkManager.Instance.roomSetting);
        //                NetworkManager.Instance.playerSeats[boolIdx] = false;
        //                NetworkManager.Instance.playerSeats[1] = true;

        //                Debug.Log(photonViewID);
        //                Debug.Log(NetworkManager.Instance.roomSetting[photonViewID.ToString()]);
        //                Debug.Log(NetworkManager.Instance.playerSeats[boolIdx]);
        //                Debug.Log(NetworkManager.Instance.playerSeats[1]);
        //            }
        //        }
        //        // 반복문 }
        //    }
        //}
        //// 조건문 } 
        #endregion
        // 조건문 }
    }
    #endregion

    #region master client 의 otherPlayerReady 값을 RPC 로 수정하는 메서드
    [PunRPC]
    public void ChangeMasterReadyValue(bool isReadyValue, string photonViewID, bool isMine)
    {
        Debug.Log(photonViewID);
        Debug.Log(isMine);

        if (isReadyValue == false)
        {
            Debug.Log(isReadyValue);
            NetworkManager.Instance.masterDataContainer.otherPlayerReady =
                NetworkManager.Instance.masterDataContainer.otherPlayerReady - 1;
        }
        else if (isReadyValue == true)
        {
            Debug.Log(isReadyValue);
            NetworkManager.Instance.masterDataContainer.otherPlayerReady =
                NetworkManager.Instance.masterDataContainer.otherPlayerReady + 1;
        }
    }
    #endregion

    #region master client 가 start button 을 눌렀을 때 loadlevel 하는 메서드
    [PunRPC]
    public void StartGame() 
    {   
        // 로드할 씬의 이름 작성
        //PhotonNetwork.LoadLevel("GameMain");
        PhotonNetwork.LoadLevel("0921");
    }
    #endregion
}
