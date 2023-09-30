using Cinemachine;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public enum UserState
{ 
    UNITSELECT = 0 , DEFENCE = 1, ATTACK = 2, ENDING = 3 
}

public enum RockState 
{ 
    ROCKSELECT = 0, ROCKCREATING = 1, ROCKCREATED = 2
}

public enum Result
{ 
    WIN = 0, LOSE = 1, NOTDEFINED = 2
}


public class CycleManager : MonoBehaviour
{
    public static CycleManager cycleManager;

    #region 변수
    public int userState;
    public int rockState;
    public int resultState;
    // 공격에서 공이 선택됨 bool
    public bool attackRockSelected = false;
    // team1 team2 체력
    public float team1Hp = 600f;
    public float team2Hp = 600f;
    ////! 서버 player gold
    //public int gold = 1000;
    // enter check bool 나중에 좋은 방법으로 바꾸기
    private bool _isEntered = false;


    // ! Photon
    public PlayerDataContainer dataContainer;
    public PhotonView dataContainerView;
    public string playerTeamNumber;
    #endregion


    public void Awake()
    {
        //if (!_isEntered)
        //{
        //    StartCoroutine(FixEnterRoutine());
            
        //}
        cycleManager = this;
        userState = (int)UserState.UNITSELECT;
        rockState = (int)RockState.ROCKSELECT;
        resultState = (int)Result.NOTDEFINED;

        // ! Photon
        dataContainer = NetworkManager.Instance.myDataContainer;
        dataContainerView = NetworkManager.Instance.myDataContainer.GetComponent<PhotonView>();
        FindMyViewID();
        SetCameraLayerMask(playerTeamNumber);
    }

    private void Update()
    {
        //if (!_isEntered) 
        //{
        //    return;
        //}

        // ! Photon
        if (dataContainerView.IsMine == true) 
        {
            if (resultState == (int)Result.NOTDEFINED)
            { 
                GameCycle();
            }
        }
    }

    // ! Photon : 플레이어의 ViewID(Key) 를 통해 PlayerNum_TeamNum(Value) 를 읽어오기 위한 메서드
    private void FindMyViewID()
    {
        foreach (var mydata in PhotonNetwork.CurrentRoom.CustomProperties)
        {
            if (mydata.Key.ToString() == dataContainer.GetComponent<PhotonView>().ViewID.ToString())
            {
                playerTeamNumber = mydata.Value.ToString();
            }
        }
    }


    #region GameCycle
    //{ GameCycle()
    public void GameCycle()
    {
        // 업데이트
        UpdateSelectionCycle(); // 유닛 선택창
        // start 구독 이벤트에서 
        UpdateCommonUICycle(); // m 눌렀을 때 효과 
        UpdateDefenceCycle();// c 눌렀을 때 효과
    }
    //} GameCycle()
    #endregion


    #region SelectCycle
    //{ UpdateSelectionCycle()
    // 선택 사이클
    // 공하나 이상 선택 & 유저 enter -> defence
    public void UpdateSelectionCycle()
    {
        // ! Photon
        if (dataContainerView.IsMine == true)
        {
            if (_isEntered == false && userState == (int)UserState.UNITSELECT)
            {
                // 공 하나 이상 선택시 문자출력 설정
                if (CheckUserBall() == true)
                {
                    UIManager.uiManager.PrintReadyText();
                }
                else { UIManager.uiManager.PrintNotReadyText(); }

                // 엔터누를시
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    // 검증 후 다음 사이클로
                    if (CheckUserBall() == true)
                    {
                        _isEntered = true;
                        UIManager.uiManager.ChangeStateUnitSelectToRockSelect();
                        CameraManager.Instance.TurnOffSelectCamera();
                    }
                    else { return; }
                }
            }
        }
    }
    //} UpdateSelectionCycle()

    //{ CheckUserBall()
    // 공이 하나 이상 선택되었는지 검증하는 함수
    public bool CheckUserBall()
    {
        if (ItemManager.itemManager.rockSelected.Count >= 1)
        {
            return true;
        }
        else { return false; }
    }
    //} CheckUserBall()

    #endregion

    #region CommonUICycle
    // 게임종료시까지 켜져 있는 UI
    public void UpdateCommonUICycle()
    {
        // inputManager로 event화 가능
        // 유닛 선택단계와 엔딩 단계가 아니라면 항상 출력
        if (userState != (int)UserState.UNITSELECT || userState != (int)UserState.ENDING)
        { 
              UIManager.uiManager.GetRotationKey();
        }
    }

    #endregion

    #region AttackCycle
    //{ ChangeCycleAttackToDefence()
    // 공격에서 공선택으로 전환하는 함수
    public void ChangeCycleAttackToSelect()
    {
        if (userState == (int)UserState.ATTACK)
        {
            userState = (int)UserState.DEFENCE;
            rockState = (int)RockState.ROCKSELECT;
            UIManager.uiManager.ChangeAttackToSelect();
            CameraManager.Instance.TurnOnTopViewCamera();
        }
        else { Debug.Log("GAME LOGIC ERROR"); }
    }
    //} ChangeCycleAttackToDefence()
    #endregion


    #region DefenceCycle
    //{ UpdateDefenceCycle()
    public void UpdateDefenceCycle()
    {
        // ! Photon
        if (dataContainerView.IsMine == true)
        {
            // 소환시간초과시 C 누르면
            if (Input.GetKeyDown(KeyCode.C))
            {
                ChangeCycleDefenceToAttack();
            }
            else { return; }
        }
    }

    // ! Photon
    // PhotonViewID 의 앞자리만 판단하기 위해서 뒤 3자리를 버리게 하는 메서드
    public string DropLastThreeChar(string inputString) 
    {
        // 매개변수로 받아온 string 을 char 배열로 변환한다
        char[] stringToChar = inputString.ToCharArray();
        // char 배열에서 뒷 3 자리를 빼기 위해 새로운 char 배열의 크기를 -3 한다
        char[] resultArray = new char[stringToChar.Length - 3];
        
        // -3 한 배열의 크기만큼 반복문을 실행한다
        for (int i = 0; i < stringToChar.Length - 3; i++) 
        {
            // 새로운 배열에다가 받아온 string -> char[] 값을 대입한다 (짧아진 길이만큼 뒷 부분은 날아간다)
            resultArray[i] = stringToChar[i];
        }
        
        // result 에 결과값을 담고 return 한다
        string result = string.Join("", resultArray);

        return result;
    }

    // ! Photon
    // RockBase 의 CheckTeamAndDequeue 메서드와 세트 (매개변수로 나의 ViewID 와 생성된 돌 ViewID 를 전달)
    public void CheckTeamAndSaveQueue(string myViewID, GameObject createdRock) 
    {
        // myViewID 에서 Team 번호만 추출한다
        string myTeamNumber = PhotonNetwork.CurrentRoom.CustomProperties[myViewID].ToString().Split('_')[1];
        // 생성된 돌의 ViewID 를 string 으로 변환하고
        string rockViewID = createdRock.GetComponent<PhotonView>().ViewID.ToString();
        // 뒷 3자리를 버린 후 001 을 더하여 생성자의 ViewID 를 만든다
        string rockOwnerViewID = DropLastThreeChar(rockViewID) + "001";
        // 생성자의 ViewID 에서 Team 번호만 추출한다
        string rockTeamNumber = PhotonNetwork.CurrentRoom.CustomProperties[rockOwnerViewID].ToString().Split('_')[1];

        // 만약 돌의 생성자가 내 팀이 아니라면
        if (myTeamNumber != rockTeamNumber)
        {
            // enemyCameraQueue 의 크기가 0 일때만 바로 카메라로 생성된 돌을 따라간다
            if (CameraManager.Instance.enemyCameraQueue.Count == 0) 
            {
                CameraManager.Instance.SetEnemyCamera(createdRock);
            }
            // 생성된 돌을 enemyCameraQueue 에 추가한다
            CameraManager.Instance.enemyCameraQueue.Enqueue(createdRock);
        }
        //Debug.Log(createdRock.GetComponent<PhotonView>().ViewID.ToString());
        //Debug.Log(CameraManager.Instance.enemyCameraQueue.Count);
    }


    public void ChangeCycleDefenceToAttack()
    {
        int userRock = ItemManager.itemManager.userRockChoosed[0];
        if (userRock != -1 && userState == (int)UserState.DEFENCE && rockState == (int)RockState.ROCKCREATED)
        {
            ResourceManager.Instance.InstatiateUserSelectedRock();

            userState = (int)UserState.ATTACK;
            rockState = (int)RockState.ROCKSELECT;
            ItemManager.itemManager.userRockChoosed[0] = -1;
            UIManager.uiManager.SwitchUIManager("attackUI");
            UIManager.uiManager.SwitchUIManager("defenceUI");
            UIManager.uiManager.SwitchRockReadyInfo();
        }
    }


    public IEnumerator WaitForRock()
    {
        if (rockState == (int)RockState.ROCKSELECT)
        { 
            rockState = (int)RockState.ROCKCREATING;
            int id = ItemManager.itemManager.userRockChoosed[0];
            float coolDown = default;
            ResourceManager.Instance.GetRockCoolDownFromId(id, out coolDown);
            UIManager.uiManager.FillAmountRockRoutine(coolDown);
            yield return new WaitForSeconds(coolDown);
            rockState = (int)RockState.ROCKCREATED;
        }
    }
    #endregion

    #region GameEndCycle
    public void DefineWinner()
    { 
        userState = (int)UserState.ENDING;
        resultState = (int)Result.WIN;
        if (userState == (int)UserState.ENDING && resultState != (int)Result.NOTDEFINED)
        {
            CameraManager.Instance.TurnOnGameEndCamera();
            UIManager.uiManager.ShutDownAllUIExpectEnding();
            UIManager.uiManager.PrintResult();
        }
    }

    public void DefineLoser()
    {
        userState = (int)UserState.ENDING;
        resultState = (int)Result.LOSE;
        if (userState == (int)UserState.ENDING && resultState != (int)Result.NOTDEFINED)
        {
            CameraManager.Instance.TurnOnGameEndCamera();
            UIManager.uiManager.ShutDownAllUIExpectEnding();
            UIManager.uiManager.PrintResult();
        }
    }
    //} UpdateGameEndCycle()
    #endregion

    // 플레이어 이름을 넣으면 team에 맞는 카메라 레이어를 바꿔주는 함수
    public void SetCameraLayerMask(string player_)
    {
        // ! Photon
        string team = player_.Split('_')[1];
        #region Legacy
        //int teamNum = (int)((int.Parse(player_.Split("Player")[1]) + 1) * 0.5f);

        //char playerSplitNum = player_[6];
        //int temp = (int)char.GetNumericValue(playerSplitNum);
        //float temp2 = (temp + 1) * 0.5f;
        //int teamNum = (int)(temp2);
        //team = "Team" + teamNum;
        #endregion

        AddCullingMask(team);
        AddLayer(team);
    }

    public void AddCullingMask(string team_)
    {
        GameObject playerObj = Global_PSC.FindTopLevelGameObject("PlayerCamera");
        GameObject enemyObj = Global_PSC.FindTopLevelGameObject("EnemyCamera");
        Camera playerCam = playerObj.GetComponent<Camera>();
        Camera enemyCam = enemyObj.GetComponent<Camera>();

        int team1 = Global_PSC.FindLayerToName("Team1");
        int team2 = Global_PSC.FindLayerToName("Team2");

        // ! Photon : culling mask 변수화
        int maskDefault = Global_PSC.FindLayerToName("Default");
        int maskTransparentFX = Global_PSC.FindLayerToName("TransparentFX");
        int maskIgnoreRaycast = Global_PSC.FindLayerToName("Ignore Raycast");
        int maskWater = Global_PSC.FindLayerToName("Water");
        int maskUI = Global_PSC.FindLayerToName("UI");
        int maskStones = Global_PSC.FindLayerToName("Stones");
        int maskTerrains = Global_PSC.FindLayerToName("Terrains");
        int maskObstacles = Global_PSC.FindLayerToName("Obstacles");
        int maskWalls = Global_PSC.FindLayerToName("Walls");
        int maskOutLand = Global_PSC.FindLayerToName("OutLand");
        int maskCastle = Global_PSC.FindLayerToName("Castle");
        int maskRock = Global_PSC.FindLayerToName("Rock");

        // ! Photon : Team 에 따른 culling mask 추가
        if (team_ == "Team1")
        {
            playerCam.cullingMask |= team1;

            playerCam.cullingMask |= maskDefault;
            playerCam.cullingMask |= maskTransparentFX;
            playerCam.cullingMask |= maskIgnoreRaycast;
            playerCam.cullingMask |= maskWater;
            playerCam.cullingMask |= maskUI;
            playerCam.cullingMask |= maskStones;
            playerCam.cullingMask |= maskTerrains;
            playerCam.cullingMask |= maskObstacles;
            playerCam.cullingMask |= maskWalls;
            playerCam.cullingMask |= maskOutLand;
            playerCam.cullingMask |= maskCastle;
            playerCam.cullingMask |= maskRock;

            enemyCam.cullingMask |= team2;

            enemyCam.cullingMask |= maskDefault;
            enemyCam.cullingMask |= maskTransparentFX;
            enemyCam.cullingMask |= maskIgnoreRaycast;
            enemyCam.cullingMask |= maskWater;
            enemyCam.cullingMask |= maskUI;
            enemyCam.cullingMask |= maskStones;
            enemyCam.cullingMask |= maskTerrains;
            enemyCam.cullingMask |= maskObstacles;
            enemyCam.cullingMask |= maskWalls;
            enemyCam.cullingMask |= maskOutLand;
            enemyCam.cullingMask |= maskCastle;
            enemyCam.cullingMask |= maskRock;
        }
        // ! Photon : Team 에 따른 culling mask 추가
        else if (team_ == "Team2")
        {
            playerCam.cullingMask |= team2;

            playerCam.cullingMask |= maskDefault;
            playerCam.cullingMask |= maskTransparentFX;
            playerCam.cullingMask |= maskIgnoreRaycast;
            playerCam.cullingMask |= maskWater;
            playerCam.cullingMask |= maskUI;
            playerCam.cullingMask |= maskStones;
            playerCam.cullingMask |= maskTerrains;
            playerCam.cullingMask |= maskObstacles;
            playerCam.cullingMask |= maskWalls;
            playerCam.cullingMask |= maskOutLand;
            playerCam.cullingMask |= maskCastle;
            playerCam.cullingMask |= maskRock;

            enemyCam.cullingMask |= team1;

            enemyCam.cullingMask |= maskDefault;
            enemyCam.cullingMask |= maskTransparentFX;
            enemyCam.cullingMask |= maskIgnoreRaycast;
            enemyCam.cullingMask |= maskWater;
            enemyCam.cullingMask |= maskUI;
            enemyCam.cullingMask |= maskStones;
            enemyCam.cullingMask |= maskTerrains;
            enemyCam.cullingMask |= maskObstacles;
            enemyCam.cullingMask |= maskWalls;
            enemyCam.cullingMask |= maskOutLand;
            enemyCam.cullingMask |= maskCastle;
            enemyCam.cullingMask |= maskRock;
        }
    }

    public void AddLayer(string team_)
    {
        #region mainCameras
        GameObject[] playerCameras = new GameObject[9];
        playerCameras[0] = Global_PSC.FindTopLevelGameObject("PlayerCamera");
        playerCameras[1] = Global_PSC.FindTopLevelGameObject("TopViewCamera");
        playerCameras[2] = Global_PSC.FindTopLevelGameObject("ClickedTopViewCamera");
        playerCameras[3] = Global_PSC.FindTopLevelGameObject("RockCamera");
        playerCameras[4] = Global_PSC.FindTopLevelGameObject("SelectCamera");
        // 4,5,6 추가됨
        playerCameras[5] = Global_PSC.FindTopLevelGameObject("GameEndCameraTeam1"); // team1 성문
        playerCameras[6] = Global_PSC.FindTopLevelGameObject("GameEndCameraTeam2"); // tema2 성문
        playerCameras[7] = Global_PSC.FindTopLevelGameObject("GameEndResultCamera"); // 게임 엔드 카메라
        playerCameras[8] = Global_PSC.FindTopLevelGameObject("SelectFocus"); // 기물 선택 시 나오는 헬리캠
        #endregion

        #region subCameras
        GameObject[] enemyCameras = new GameObject[2];
        enemyCameras[0] = Global_PSC.FindTopLevelGameObject("EnemyCamera");
        enemyCameras[1] = Global_PSC.FindTopLevelGameObject("EnemyRockCamera");
        // 2번빠짐
        #endregion

        foreach (var playerCamera in playerCameras)
        {
            if (team_ == "Team1")
            {
                playerCamera.gameObject.layer = LayerMask.NameToLayer("Team1");
            }
            else
            { 
                playerCamera.gameObject.layer = LayerMask.NameToLayer("Team2");
            }
        }

        foreach (var enemyCamera in enemyCameras)
        {
            if (team_ == "Team1")
            {
                enemyCamera.gameObject.layer = LayerMask.NameToLayer("Team2");
            }
            else
            {
                enemyCamera.gameObject.layer = LayerMask.NameToLayer("Team1");
            }
        }
    }
}
