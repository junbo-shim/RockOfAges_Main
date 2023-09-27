using TMPro;
using UnityEngine;
using UnityEngine.UI;

// endingUI
public partial class UIManager : MonoBehaviour
{
    #region 변수
    [Header("UI")]
    public GameObject attackUI;

    #region 플레이어 이름
    // playerName 1~4
    [Header("PLAYERNAME")]
    public TextMeshProUGUI player1NameTxt;
    public TextMeshProUGUI player2NameTxt;
    public TextMeshProUGUI player3NameTxt;
    public TextMeshProUGUI player4NameTxt;
    #endregion

    #region 플레이어 점수
    // playerScore 1~4
    [Header("PLAYERSCORE")]
    public TextMeshProUGUI player1ScoreTxt;
    public TextMeshProUGUI player2ScoreTxt;
    public TextMeshProUGUI player3ScoreTxt;
    public TextMeshProUGUI player4ScoreTxt;
    #endregion

    #region 플레이어 초상화 이미지
    // playerIcon 1~4
    [Header("PLAYERICON")]
    public Image player1Icon;
    public Image player2Icon;
    public Image player3Icon;
    public Image player4Icon;
    #endregion

    #region 플레이어 위치표시
    // playerImg(내꺼라는 표시)
    [Header("PLAYERINDICATOR")]
    public Image player1userImg;
    public Image player2userImg;
    public Image player3userImg;
    public Image player4userImg;
    #endregion

    // 승자 - 왕관, 패자 - 체스
    [Header("WIN,LOSE IMAGE")]
    public Image crownImg;
    public Image chessImg;


    #endregion

    #region Functions

    public void PrintResult()
    { 
    
    
    }
    //// string을 넣으면 뒤에 숫자만 받아오는 함수
    //public int ConvertStringToInt(string player, out int playerNumber)
    //{
    //    string str = player;
    //    string result = Regex.Replace(str, @"[^0-9]", "");
    //    playerNumber = int.Parse(result);
    //    return playerNumber;
    //}


    //public void PrintPlayerName(string player)
    //{
    //    int playerNumber;
    //    ConvertStringToInt(player, out playerNumber);
    //    switch (playerNumber) 
    //    {
    //        case 1:
    //            //player1NameTxt.text = PhotonNetwork.CurrentRoom.CustomProperties[""];
    //            break;
    //        case 2:
    //            //player1NameTxt.text = PhotonNetwork.CurrentRoom.CustomProperties[""];
    //            break;
    //        case 3:
    //            //player1NameTxt.text = PhotonNetwork.CurrentRoom.CustomProperties[""]; 
    //            break;
    //        case 4:
    //            //player1NameTxt.text = PhotonNetwork.CurrentRoom.CustomProperties[""];
    //            break;
    //    }
    //}

    //public void PrintPlayerScore(string player)
    //{
    //    int playerNumber;
    //    ConvertStringToInt(player, out playerNumber);
    //    switch (playerNumber)
    //    {
    //        case 1:
    //            //player1NameTxt.text = PhotonNetwork.CurrentRoom.CustomProperties[""];
    //            break;
    //        case 2:
    //            //player1NameTxt.text = PhotonNetwork.CurrentRoom.CustomProperties[""];
    //            break;
    //        case 3:
    //            //player1NameTxt.text = PhotonNetwork.CurrentRoom.CustomProperties[""]; 
    //            break;
    //        case 4:
    //            //player1NameTxt.text = PhotonNetwork.CurrentRoom.CustomProperties[""];
    //            break;
    //    }
    //}

    //public void PrintPlayerIcon(string player)
    //{
    //    int playerNumber;
    //    ConvertStringToInt(player, out playerNumber);
    //    switch (playerNumber)
    //    {
    //        case 1:
    //            //player1NameTxt.text = PhotonNetwork.CurrentRoom.CustomProperties[""];
    //            break;
    //        case 2:
    //            //player1NameTxt.text = PhotonNetwork.CurrentRoom.CustomProperties[""];
    //            break;
    //        case 3:
    //            //player1NameTxt.text = PhotonNetwork.CurrentRoom.CustomProperties[""]; 
    //            break;
    //        case 4:
    //            //player1NameTxt.text = PhotonNetwork.CurrentRoom.CustomProperties[""];
    //            break;
    //    }

    //}

    //public void PrintUserImg(string player)
    //{
    //    int playerNumber;
    //    ConvertStringToInt(player, out playerNumber);
    //    switch (playerNumber)
    //    {
    //        case 1:
    //            //player1NameTxt.text = PhotonNetwork.CurrentRoom.CustomProperties[""];
    //            break;
    //        case 2:
    //            //player1NameTxt.text = PhotonNetwork.CurrentRoom.CustomProperties[""];
    //            break;
    //        case 3:
    //            //player1NameTxt.text = PhotonNetwork.CurrentRoom.CustomProperties[""]; 
    //            break;
    //        case 4:
    //            //player1NameTxt.text = PhotonNetwork.CurrentRoom.CustomProperties[""];
    //            break;
    //    }

    //}

    public void PrintCrownOrChess()
    {
        int result = CycleManager.cycleManager.resultState;
        int win = (int)Result.WIN;
        int lose = (int)Result.LOSE;
        Vector3 scale = Vector3.one;
        if (result == win)
        {
            crownImg.transform.localScale = scale;  
        }
        else if (result == lose)
        { 
            chessImg.transform.localScale = scale;
        }
    }


    #endregion

    //----------------- 준보형 이거 형이 뜯어가세요
    // 버튼이니깐 OnClick으로 하시면 될 꺼 같습니다
    public void BackToLobby()
    { 
        // 로비로 돌아가는 기능
    }

    public void QutGame()
    { 
        Application.Quit();
    }
}
