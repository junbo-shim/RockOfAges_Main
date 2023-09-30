using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;
using Photon.Pun.Demo.PunBasics;
using System.Xml.Linq;

public class PlayerDataContainer : MonoBehaviourPun, IPunObservable
{
    public int otherPlayerReady;
    public string MyName { get; private set; }
    public string Player1Name { get; private set; }
    public string Player2Name { get; private set; }
    public string Player3Name { get; private set; }
    public string Player4Name { get; private set; }

    public string Player1ViewID { get; private set; }
    public string Player2ViewID { get; private set; }
    public string Player3ViewID { get; private set; }
    public string Player4ViewID { get; private set; }

    public string Player1Num { get; private set; }
    public string Player2Num { get; private set; }
    public string Player3Num { get; private set; }
    public string Player4Num { get; private set; }

    public float MyGold { get; private set; } = 1000f;
    public float Player1Gold { get; private set; }
    public float Player2Gold { get; private set; }
    public float Player3Gold { get; private set; }
    public float Player4Gold { get; private set; }

    public int MyScore { get; private set; }
    public int Player1Score { get; private set; }
    public int Player2Score { get; private set; }
    public int Player3Score { get; private set; }
    public int Player4Score { get; private set; }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) 
    {
        
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        MyName = NetworkManager.Instance.playerNickName;
        MyScore = NetworkManager.Instance.playerScore;

        otherPlayerReady = default;
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
            SavePlayerGold();

            // UIManager Awake 보다 늦게 돌아서 여기서 아예 메서드 사용
            PlayerDataContainer tempContainer = gameObject.GetComponent<PlayerDataContainer>();
            NetworkManager.Instance.myDataContainer = tempContainer;
            UIManager.uiManager.InitDataContainer();
            // UI 플레이어 이름 출력
            UIManager.uiManager.PrintPlayerText(tempContainer.Player1Name, tempContainer.Player2Name, 
                tempContainer.Player3Name, tempContainer.Player4Name);
            UIManager.uiManager.PrintMyGold(tempContainer.MyGold);
        }
    }
    #endregion

    // 플레이어들의 닉네임 저장하는 메서드 (Key : PlayerSeatNumber / Value : PlayerName)
    public void SavePlayerNames() 
    {
        Player1Name = PhotonNetwork.CurrentRoom.CustomProperties["Player1"].ToString();
        Player2Name = PhotonNetwork.CurrentRoom.CustomProperties["Player2"].ToString();
        Player3Name = PhotonNetwork.CurrentRoom.CustomProperties["Player3"].ToString();
        Player4Name = PhotonNetwork.CurrentRoom.CustomProperties["Player4"].ToString();
    }
    // 플레이어들의 ViewID 를 저장하는 메서드 (Key : PlayerName / Value : PlayerViewID)
    public void SaveViewID() 
    {
        Player1ViewID = PhotonNetwork.CurrentRoom.CustomProperties[Player1Name].ToString();
        Player2ViewID = PhotonNetwork.CurrentRoom.CustomProperties[Player2Name].ToString();
        Player3ViewID = PhotonNetwork.CurrentRoom.CustomProperties[Player3Name].ToString();
        Player4ViewID = PhotonNetwork.CurrentRoom.CustomProperties[Player4Name].ToString();
    }
    // 플레이어들의 개인번호와 팀 번호를 저장하는 메서드 (Key : PlayerViewID / Value : PlayerNumber_PlayerTeamNumber)
    public void SaveNumberAndTeam() 
    {
        Player1Num = PhotonNetwork.CurrentRoom.CustomProperties[Player1ViewID].ToString();
        Player2Num = PhotonNetwork.CurrentRoom.CustomProperties[Player2ViewID].ToString();
        Player3Num = PhotonNetwork.CurrentRoom.CustomProperties[Player3ViewID].ToString();
        Player4Num = PhotonNetwork.CurrentRoom.CustomProperties[Player4ViewID].ToString();
    }
    // 플레이어들의 MyGold 변수를 master 계산을 위해 각각 번호로 분배하는 메서드
    public void SavePlayerGold() 
    {
        if (PhotonNetwork.IsMasterClient == true) 
        {
            Player1Gold = MyGold;
            Player2Gold = MyGold;
            Player3Gold = MyGold;
            Player4Gold = MyGold;
        }
    }
    // 플레이어들의 개인 점수를 PlayFab DB 에서 불러오는 메서드
    public void SavePlayerScore() 
    {
        
    }


    public void ResetAllData(int player1ViewID, int player2ViewID, int player3ViewID, int player4ViewID)
    {
        // 플레이어 자리 위치 저장 값 초기화
        for (int i = 0; i < NetworkManager.Instance.playerSeats.Length; i++) 
        {
            NetworkManager.Instance.playerSeats[i] = false;
        }

        // 플레이어 아이디정보 초기화

        // 플레이어 정보 초기화

        // 플레이어 팀 정보 초기화

        // 골드 초기화

        // 점수 초기화
    }
    
    #region player (boolIdx + 1) 자리 가겠다는 요청을 master client 에게 RPC 로 전달하는 메서드
    [PunRPC]
    public void SendPlayerPosition(string senderNickName, int senderViewID, int boolIdx) // 포톤뷰ID 와 가고 싶은 자리의 인덱스를 매개변수로 받음
    {
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
            string teamNumber = default;
            int index = boolIdx + 1;

            if (index <= 2)
            {
                teamNumber = "_Team1";
            }
            else if (index >= 2) 
            {
                teamNumber = "_Team2";
            }

            // roomSetting 해시테이블에 키로 photonViewID 가 존재하면
            if (NetworkManager.Instance.roomSetting.ContainsKey(senderViewID))
            {
                NetworkManager.Instance.playerSeats[index-1] = false;
                NetworkManager.Instance.roomSetting[senderViewID.ToString()] = seatName + teamNumber;
                NetworkManager.Instance.roomSetting[seatName] = senderNickName;
                NetworkManager.Instance.roomSetting[senderNickName] = senderViewID.ToString();
                Debug.LogFormat("{0} 가 Player{1} 에서 {2} 로 이동 했습니다.", senderNickName, index, seatName);
            }
            else
            {
                //NetworkManager.Instance.roomSetting.Add(senderNickName, seatName);
                NetworkManager.Instance.roomSetting[senderViewID.ToString()] = seatName + teamNumber;
                NetworkManager.Instance.roomSetting[seatName] = senderNickName;
                NetworkManager.Instance.roomSetting[senderNickName] = senderViewID.ToString();
                Debug.LogFormat("{0} 가 {1} 로 이동 했습니다.", senderNickName, seatName);
            }
            PhotonNetwork.CurrentRoom.SetCustomProperties(NetworkManager.Instance.roomSetting);
            NetworkManager.Instance.playerSeats[boolIdx] = true;
            //photonView.RPC("ChangeID", RpcTarget.All, photonView.ViewID, 0);

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
            NetworkManager.Instance.masterDataContainer.otherPlayerReady =
                NetworkManager.Instance.masterDataContainer.otherPlayerReady - 1;
            //photonView.RPC("ChangeCheck", RpcTarget.All, photonViewID, isReadyValue);
        }
        else if (isReadyValue == true)
        {
            NetworkManager.Instance.masterDataContainer.otherPlayerReady =
                NetworkManager.Instance.masterDataContainer.otherPlayerReady + 1;
            //photonView.RPC("ChangeCheck", RpcTarget.All, photonViewID, isReadyValue);
        }
    }
    #endregion

    #region master client 가 start button 을 눌렀을 때 loadlevel 하는 메서드
    [PunRPC]
    public void StartGame() 
    {   
        // 로드할 씬의 이름 작성
        PhotonNetwork.LoadLevel("0921");
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }
    #endregion

    #region master 가 플레이어 아이디 변경하는 메서드
    //[PunRPC]
    //public void ChangeID(int photonViewID, int seatIdx, int beforeSeatIdx) 
    //{
    //    Button button1 = ButtonManager.Instance.player1Button;
    //    Button button2 = ButtonManager.Instance.player2Button;
    //    Button button3 = ButtonManager.Instance.player3Button;
    //    Button button4 = ButtonManager.Instance.player4Button;

    //    if (seatIdx == 0) 
    //    {
    //        if (beforeSeatIdx == -1) 
    //        {
    //            button1.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = 
    //                photonViewID.ToString();            
    //        }
    //        else if (beforeSeatIdx != -1) 
    //        {

    //            button1.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text =
    //                photonViewID.ToString();
    //        }
    //    }
    //    else if (seatIdx == 1) 
    //    {
    //        if (beforeSeatIdx == -1)
    //        {
    //            button2.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text =
    //                photonViewID.ToString();
    //        }
    //        else if (beforeSeatIdx != -1)
    //        {
    //            button2.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text =
    //                photonViewID.ToString();
    //        }
    //    }
    //    else if (seatIdx == 2)
    //    {
    //        if (beforeSeatIdx == -1)
    //        {
    //            button3.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text =
    //                photonViewID.ToString();
    //        }
    //        else if (beforeSeatIdx != -1)
    //        {
    //            button3.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text =
    //                photonViewID.ToString();
    //        }
    //    }
    //    else if (seatIdx == 3)
    //    {
    //        if (beforeSeatIdx == -1)
    //        {
    //            button4.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text =
    //                photonViewID.ToString();
    //        }
    //        else if (beforeSeatIdx != -1)
    //        {
    //            button4.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text =
    //                photonViewID.ToString();
    //        }
    //    }
    //}
    #endregion

    #region master 가 플레이어가 레디를 눌렀을 시 체크를 변경하는 메서드
    //[PunRPC]
    //public void ChangeCheck(int photonViewID, bool readyValue) 
    //{
    //    Button button1 = ButtonManager.Instance.player1Button;
    //    Button button2 = ButtonManager.Instance.player2Button;
    //    Button button3 = ButtonManager.Instance.player3Button;
    //    Button button4 = ButtonManager.Instance.player4Button;

    //    if (button1.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text == photonViewID.ToString()) 
    //    {
    //        if (readyValue == true)
    //        {
    //            button1.gameObject.transform.Find("Check").GetComponent<Image>().color = Color.white;
    //        }
    //        else if (readyValue == false)
    //        {
    //            button1.gameObject.transform.Find("Check").GetComponent<Image>().color = Color.clear;
    //        }
    //    }
    //    else if (button2.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text == photonViewID.ToString()) 
    //    {
    //        if (readyValue == true)
    //        {
    //            button2.gameObject.transform.Find("Check").GetComponent<Image>().color = Color.white;
    //        }
    //        else if (readyValue == false)
    //        {
    //            button2.gameObject.transform.Find("Check").GetComponent<Image>().color = Color.clear;
    //        }
    //    }
    //    else if (button3.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text == photonViewID.ToString())
    //    {
    //        if (readyValue == true)
    //        {
    //            button3.gameObject.transform.Find("Check").GetComponent<Image>().color = Color.white;
    //        }
    //        else if (readyValue == false)
    //        {
    //            button3.gameObject.transform.Find("Check").GetComponent<Image>().color = Color.clear;
    //        }
    //    }
    //    else if (button4.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text == photonViewID.ToString())
    //    {
    //        if (readyValue == true)
    //        {
    //            button4.gameObject.transform.Find("Check").GetComponent<Image>().color = Color.white;
    //        }
    //        else if (readyValue == false)
    //        {
    //            button4.gameObject.transform.Find("Check").GetComponent<Image>().color = Color.clear;
    //        }
    //    }
    //}
    #endregion

    #region master 가 플레이어 아이디 변경하는 메서드
    //[PunRPC]
    //public void ChangeID(int photonViewID, int seatIdx, int beforeSeatIdx) 
    //{
    //    Button button1 = ButtonManager.Instance.player1Button;
    //    Button button2 = ButtonManager.Instance.player2Button;
    //    Button button3 = ButtonManager.Instance.player3Button;
    //    Button button4 = ButtonManager.Instance.player4Button;

    //    if (seatIdx == 0) 
    //    {
    //        if (beforeSeatIdx == -1) 
    //        {
    //            button1.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = 
    //                photonViewID.ToString();            
    //        }
    //        else if (beforeSeatIdx != -1) 
    //        {

    //            button1.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text =
    //                photonViewID.ToString();
    //        }
    //    }
    //    else if (seatIdx == 1) 
    //    {
    //        if (beforeSeatIdx == -1)
    //        {
    //            button2.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text =
    //                photonViewID.ToString();
    //        }
    //        else if (beforeSeatIdx != -1)
    //        {
    //            button2.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text =
    //                photonViewID.ToString();
    //        }
    //    }
    //    else if (seatIdx == 2)
    //    {
    //        if (beforeSeatIdx == -1)
    //        {
    //            button3.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text =
    //                photonViewID.ToString();
    //        }
    //        else if (beforeSeatIdx != -1)
    //        {
    //            button3.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text =
    //                photonViewID.ToString();
    //        }
    //    }
    //    else if (seatIdx == 3)
    //    {
    //        if (beforeSeatIdx == -1)
    //        {
    //            button4.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text =
    //                photonViewID.ToString();
    //        }
    //        else if (beforeSeatIdx != -1)
    //        {
    //            button4.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text =
    //                photonViewID.ToString();
    //        }
    //    }
    //}
    #endregion

    #region master 가 플레이어가 레디를 눌렀을 시 체크를 변경하는 메서드
    //[PunRPC]
    //public void ChangeCheck(int photonViewID, bool readyValue) 
    //{
    //    Button button1 = ButtonManager.Instance.player1Button;
    //    Button button2 = ButtonManager.Instance.player2Button;
    //    Button button3 = ButtonManager.Instance.player3Button;
    //    Button button4 = ButtonManager.Instance.player4Button;

    //    if (button1.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text == photonViewID.ToString()) 
    //    {
    //        if (readyValue == true)
    //        {
    //            button1.gameObject.transform.Find("Check").GetComponent<Image>().color = Color.white;
    //        }
    //        else if (readyValue == false)
    //        {
    //            button1.gameObject.transform.Find("Check").GetComponent<Image>().color = Color.clear;
    //        }
    //    }
    //    else if (button2.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text == photonViewID.ToString()) 
    //    {
    //        if (readyValue == true)
    //        {
    //            button2.gameObject.transform.Find("Check").GetComponent<Image>().color = Color.white;
    //        }
    //        else if (readyValue == false)
    //        {
    //            button2.gameObject.transform.Find("Check").GetComponent<Image>().color = Color.clear;
    //        }
    //    }
    //    else if (button3.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text == photonViewID.ToString())
    //    {
    //        if (readyValue == true)
    //        {
    //            button3.gameObject.transform.Find("Check").GetComponent<Image>().color = Color.white;
    //        }
    //        else if (readyValue == false)
    //        {
    //            button3.gameObject.transform.Find("Check").GetComponent<Image>().color = Color.clear;
    //        }
    //    }
    //    else if (button4.gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text == photonViewID.ToString())
    //    {
    //        if (readyValue == true)
    //        {
    //            button4.gameObject.transform.Find("Check").GetComponent<Image>().color = Color.white;
    //        }
    //        else if (readyValue == false)
    //        {
    //            button4.gameObject.transform.Find("Check").GetComponent<Image>().color = Color.clear;
    //        }
    //    }
    //}
    #endregion

    #region Gold 사용하는 메서드
    public void UseGold(string senderViewID, float haveGold, float price) 
    {
        float gold = default;
        float resultGold = default;

        // gold 변수에 viewID 에 맞는 player'N'Gold 변수를 꺼내서 담는다
        gold = CheckAndReturnPlayerGold(senderViewID, haveGold);
        // 골드 감산
        gold -= price;
        // resultGold 변수에 viewID 에 맞추어 다시 player'N'Gold 변수를 꺼내서 저장한다
        resultGold = CheckAndReturnPlayerGold(senderViewID, gold);

        // senderViewID 가 계산을 담당하는 photonViewID 즉, master 와 같을 경우 자기 UI 변경
        if (senderViewID == photonView.ViewID.ToString()) 
        {
            UIManager.uiManager.PrintMyGold(resultGold);
        }

        // 계산을 마친 결과값을 master 가 sender 에게 다시 보내는 RPC
        photonView.RPC("SaveReturnGold", RpcTarget.Others, senderViewID, resultGold);
    }
    #endregion

    #region Gold 사용과 저장을 위해 master 에서 PhotonViewID 를 통해 확인하는 메서드 (return : 해당 ViewID 의 Gold 변수)
    public float CheckAndReturnPlayerGold(string viewID, float gold)
    {
        string value = PhotonNetwork.CurrentRoom.CustomProperties[viewID].ToString();
        string playerValue = value.Split('_')[0];
        int playerNum = int.Parse(playerValue.Split()[6]);

        switch (playerNum)
        {
            case 1:
                Player1Gold = gold;
                return Player1Gold;
            case 2:
                Player2Gold = gold;
                return Player2Gold;
            case 3:
                Player3Gold = gold;
                return Player3Gold;
            case 4:
                Player4Gold = gold;
                return Player4Gold;
            default:
                return -1;
        }
    }
    #endregion

    #region master 에서 다른 client 들에게 계산결과를 보내 MyGold 에 변화량을 저장하는 RPC
    [PunRPC]
    public void SaveReturnGold(string viewID, float result)
    {
        if (viewID == photonView.ViewID.ToString()) 
        {
            MyGold = result;
        }

        UIManager.uiManager.PrintMyGold(MyGold);
    }
    #endregion
}
