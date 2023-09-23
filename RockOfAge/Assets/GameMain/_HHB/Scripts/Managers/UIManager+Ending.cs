using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

// endingUI
public partial class UIManager : MonoBehaviour
{

    #region 변수
    public GameObject attackUI;

    #region 플레이어 이름
    // playerName 1~4
    public TextMeshProUGUI player1NameTxt;
    public TextMeshProUGUI player2NameTxt;
    public TextMeshProUGUI player3NameTxt;
    public TextMeshProUGUI player4NameTxt;
    #endregion

    #region 플레이어 점수
    // playerScore 1~4
    public TextMeshProUGUI player1ScoreTxt;
    public TextMeshProUGUI player2ScoreTxt;
    public TextMeshProUGUI player3ScoreTxt;
    public TextMeshProUGUI player4ScoreTxt;
    #endregion

    #region 플레이어 초상화 이미지
    // playerIcon 1~4
    public Image player1Icon;
    public Image player2Icon;
    public Image player3Icon;
    public Image player4Icon;
    #endregion

    #region 플레이어 위치표시
    // playerImg(내꺼라는 표시)
    public Image player1userImg;
    public Image player2userImg;
    public Image player3userImg;
    public Image player4userImg;
    #endregion
    #endregion

    #region Functions

    // string을 넣으면 뒤에 숫자만 받아오는 함수
    public int ConvertStringToInt(string player, out int playerNumber)
    {
        string str = player;
        string result = Regex.Replace(str, @"[^0-9]", "");
        playerNumber = int.Parse(result);
        return playerNumber;
    }

    public void PrintPlayerName(string player)
    {
        int playerNumber;
        ConvertStringToInt(player, out playerNumber);
        switch (playerNumber) 
        {
            case 1:
                //player1NameTxt.text = 
                break;
            case 2:
                //player2NameTxt.text = 
                break;
            case 3:
                //player3NameTxt.text = 
                break;
            case 4:
                //player4NameTxt.text = 
                break;
        }
    }

    public void PrintPlayerScore(string player)
    { 
    
    }

    public void PrintPlayerIcon(string player)
    { 
    
    
    }

    public void PrintUserImg(string player)
    { 
    
    
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
