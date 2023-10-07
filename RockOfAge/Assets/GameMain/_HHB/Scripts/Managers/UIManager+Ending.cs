using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
    [Header("WIN, LOSE IMAGE")]
    public Image crownImg;
    public Image chessImg;

    [Header("WIN, LOSE FLAG COLOR")]
    public Material winGreen;
    public Material loseRed;
    public GameObject[] benchs;

    // ! Photon 
    private Transform gameEndUI;
    private Transform buttonHolder;
    private Button lobbyButton;
    private Button exitButton;
    private Transform resultHolder;
    private Transform resultImageHolder;
    public TextMeshProUGUI victory;

    public Button quitButton;
    public Button userLobbyButton;

    #endregion

    #region Functions

//심준보
    // ! Photon : 엔딩 UI 요소 할당 메서
   // public void GetEndUI() 
  //  {
        //gameEndUI = GameObject.Find("GameEndUI").transform;
       // buttonHolder = gameEndUI.Find("EndHolder").Find("ButtonHolder");
       // lobbyButton = buttonHolder.Find("LobbyButton").GetComponent<Button>();
       // exitButton = buttonHolder.Find("ExitButton").GetComponent<Button>();
   // }
   
   //홍한범
    //public void GetEndUI() 
    //{
    //    gameEndUI = GameObject.Find("GameEndUI").transform;
    //    buttonHolder = gameEndUI.Find("ButtonHolder");
    //    lobbyButton = buttonHolder.Find("LobbyButton").GetComponent<Button>();
    //    exitButton = buttonHolder.Find("ExitButton").GetComponent<Button>();
    //}

    //public void AssignEndUIButtons() 
    //{
    //    lobbyButton.onClick.AddListener(BackToLobby);
    //    exitButton.onClick.AddListener(QutGame);
    //}



    public void PrintResult()
    {
        PrintCrownOrChess();
        ChangeFlagMaterialColor();
        //string player = NetworkManager.Instance.myDataContainer.GetComponent<PlayerDataContainer>().PlayerTeamNum.Split('_')[0];
        //int playerNumber;
        //ConvertStringToInt(player, out playerNumber);
        PrintUserResult(NetworkManager.Instance.dataContainers);
    }


    public void PrintUserResult(List<PlayerDataContainer> dataContainers)
    {
        PrintPlayerName(dataContainers);
        PrintPlayerScore(dataContainers);
        PrintPlayerIcon(NetworkManager.Instance.myDataContainer.GetComponent<PlayerDataContainer>());
        PrintUserImg(dataContainers);
    }



    // string을 넣으면 뒤에 숫자만 받아오는 함수
    public int ConvertStringToInt(string player, out int playerNumber)
    {
        string str = player;
        string result = System.Text.RegularExpressions.Regex.Replace(str, @"[^0-9]", "");
        playerNumber = int.Parse(result);
        return playerNumber;
    }

    public void PrintVicOrLose(string team, string winnerTeam)
    {
        if (team == winnerTeam)
        {
            victory.text = "Victory";
        }
        else 
        {
            victory.text = "Lose";
        }
    }

    #region Legacy
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
    #endregion

    public void PrintPlayerName(List<PlayerDataContainer> dataContainers)
    {
        foreach (var container in dataContainers)
        {
            if (container.PlayerTeamNum.Contains("Player1"))
            {
                player1NameTxt.text = container.PlayerName;
            }
            else if (container.PlayerTeamNum.Contains("Player2"))
            {
                player2NameTxt.text = container.PlayerName;
            }
            else if (container.PlayerTeamNum.Contains("Player3"))
            {
                player3NameTxt.text = container.PlayerName;
            }
            else if (container.PlayerTeamNum.Contains("Player4"))
            {
                player4NameTxt.text = container.PlayerName;
            }
        }
    }

    public void PrintPlayerScore(List<PlayerDataContainer> dataContainers)
    {
        foreach (var container in dataContainers)
        {
            if (container.PlayerTeamNum.Contains("Player1"))
            {
                player1ScoreTxt.text = container.playerScore.ToString();
            }
            else if (container.PlayerTeamNum.Contains("Player2"))
            {
                player2ScoreTxt.text = container.playerScore.ToString();
            }
            else if (container.PlayerTeamNum.Contains("Player3"))
            {
                player3ScoreTxt.text = container.playerScore.ToString();
            }
            else if (container.PlayerTeamNum.Contains("Player4"))
            {
                player4ScoreTxt.text = container.playerScore.ToString();
            }
        }
    }

    public void PrintPlayerIcon(PlayerDataContainer myDataContainer)
    {
        if (myDataContainer.PlayerTeamNum.Contains("Player1")) 
        {
            player1userImg.gameObject.SetActive(true);
        }
        else if (myDataContainer.PlayerTeamNum.Contains("Player2")) 
        {
            player2userImg.gameObject.SetActive(true);
        }
        else if (myDataContainer.PlayerTeamNum.Contains("Player3")) 
        {
            player3userImg.gameObject.SetActive(true);
        }
        else if (myDataContainer.PlayerTeamNum.Contains("Player4")) 
        {
            player4userImg.gameObject.SetActive(true);
        }
    }

    public void PrintUserImg(List<PlayerDataContainer> dataContainers)
    {
        foreach (var container in dataContainers)
        {
            if (container.PlayerTeamNum.Contains("Player1"))
            {
                player1NameTxt.text = container.PlayerName;
            }
            else if (container.PlayerTeamNum.Contains("Player2"))
            {
                player2NameTxt.text = container.PlayerName;
            }
            else if (container.PlayerTeamNum.Contains("Player3"))
            {
                player3NameTxt.text = container.PlayerName;
            }
            else if (container.PlayerTeamNum.Contains("Player4"))
            {
                player4NameTxt.text = container.PlayerName;
            }
        }
    }

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

    public void ChangeFlagMaterialColor()
    {
        int result = CycleManager.cycleManager.resultState;
        int win = (int)Result.WIN;
        int lose = (int)Result.LOSE;

        if (result == win)
        {
            foreach (GameObject bench in benchs)
            { 
                MeshRenderer meshRenderer = bench.GetComponent<MeshRenderer>();
                meshRenderer.materials[1] = winGreen;
            }
        }
        else if (result == lose)
        {
            foreach (GameObject bench in benchs)
            {
                MeshRenderer meshRenderer = bench.GetComponent<MeshRenderer>();
                meshRenderer.materials[1] = loseRed;
            }
        }
    }


    #endregion

    public void BackToLobby()
    {
        // 만약 잘 안먹히면 주석 풀고 사용
        //PhotonNetwork.Disconnect();
        SceneManager.LoadScene(NetworkManager.Instance.PhotonScene);
        //PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.JoinLobby();
    }

    public void QuitGame()
    { 
        Application.Quit();
    }
}
