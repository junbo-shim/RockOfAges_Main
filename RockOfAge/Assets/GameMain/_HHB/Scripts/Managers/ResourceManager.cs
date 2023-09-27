using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ResourceManager : GlobalSingleton<ResourceManager>
{
    // 돌 리소스
    public Dictionary<int, GameObject> rockResources = new Dictionary<int, GameObject>();
    // 유닛 리소스
    public Dictionary<int, GameObject> unitResources = new Dictionary<int, GameObject>();


    // ! photon
    //public GameObject userRockObject;
    public string playerTeamNumber;
    public Vector3 team1StartPoint;
    public Vector3 team2StartPoint;
    public PhotonView dataContainerView;
    
    //Vector3 startPointTeam1 = new Vector3(111.55f, 31.21f, 120f);

    protected override void Awake()
    {
        // ! Photon
        team1StartPoint = new Vector3(-107f, 40f, -107f);
        team2StartPoint = new Vector3(107f, 40f, 107f); 

        dataContainerView = NetworkManager.Instance.myDataContainer.GetComponent<PhotonView>();
        playerTeamNumber = PhotonNetwork.CurrentRoom.CustomProperties[dataContainerView.ViewID.ToString()].ToString();
        PackAwake();
    }

    //{ AwakePack()
    // 리소스에서 인잇
    public void PackAwake()
    {
        for (int i = 1; i < 4; i++)
        {
            //GameObject rockPrefab = Resources.Load<GameObject>("Rocks/" + i);
            GameObject rockPrefab = Resources.Load<GameObject>(i.ToString());
            if (rockPrefab != null)
            {
                rockResources[i] = rockPrefab;
            }
            else { Debug.Log("Failed To Load Rock" + i);}
        }
        for (int i = 11; i < 20; i++)
        {
            //GameObject unitPrefab = Resources.Load<GameObject>("Units/" + i);
            GameObject unitPrefab = Resources.Load<GameObject>(i.ToString());
            if (unitPrefab != null)
            {
                unitResources[i] = unitPrefab;
            }
            else { Debug.Log("Failed To Load Obstacle" + i); }
        }
    }
    //} AwakePack()


    //{ T Load<T>()
    // 경로를 불러오기위한 제너릭 함수
    public T Load<T>(string path_) where T : Object
    { 
        return Resources.Load<T>(path_);
    }
    //} T Load<T>()

    #region LEGACY
    // 경로에서 불러낸 이름을 int로 교환하는 함수
    //public bool ChangeStringToInt(string str_id_, out int int_id_)
    //{
    //    if (int.TryParse(str_id_, out int_id_))
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        Debug.Log("Failed To Convert String To Int");
    //        int_id_ = -1;
    //        return false;
    //    }
    //}
    #endregion

    //{ GetGameObjectByID()
    // id를 넣으면 GameObject를 리턴하는 함수
    public GameObject GetGameObjectByID(int id_)
    {
        //Debug.Log(id_);
        // 돌
        if (id_ >= 0 && id_ <= 10)
        {
            if (rockResources.ContainsKey(id_))
            {
                return rockResources[id_];
            }
            else { Debug.Log("RockResource Error"); return null; }
        }
        // 유닛
        if (id_ > 10)
        {
            //Debug.Log(id_);
            if (unitResources.ContainsKey(id_))
            {
                return unitResources[id_];
            }
            else { Debug.Log("unitResource Error"); return null; }
        }
        else { Debug.Log("Resource Error"); return null; }   
    }
    //} GetGameObjectByID()


    //{ GetRockCoolDownFromId()
    // id를 넣으면 cooltime을 리턴하는 함수
    public float GetRockCoolDownFromId(int id_, out float coolDown_)
    {
        GameObject gameObject = GetGameObjectByID(id_);
        RockBase rock = gameObject.GetComponent<RockBase>();        
        // rock
        if (id_ >= 0 && id_ <= 10)
        {
            coolDown_ = rock.rockStatus.Cooldown;
            return coolDown_;
        }
        else { Debug.Log("Failed To Get CoolDown"); return coolDown_ = 0f; }
    }
    //} GetRockCoolDownFromId()

    //{ GetUnitGoldAndBuildLimitFromID()
    // id넣으면 gold와 buildlimit를 리턴하는 함수
    public void GetUnitGoldAndBuildLimitFromID(int id_, out float gold_, out int buildLimit_)
    {
        GameObject gameObject = GetGameObjectByID(id_);
        if (id_ > 10)
        {
            ObstacleBase obstacle = gameObject.GetComponent<ObstacleBase>();
            gold_ = obstacle.status.Price;
            buildLimit_ = obstacle.status.BuildLimit;
        }
        else { Debug.Log("Failed To Get Gold & BuildLimit"); gold_ = 0f; buildLimit_ = 0; }
    }
    //} GetUnitGoldAndBuildLimitFromID()

    //! 서버
    //{ InstatiateUserSelectedRock()
    // id를 넣으면 그에 맞는 공을 생성해주는 함수
    public void InstatiateUserSelectedRock()
    {
        int id = ItemManager.itemManager.userRockChoosed[0];
        GameObject gameObject = GetGameObjectByID(id);

        GameObject userRockObject =
               PhotonNetwork.Instantiate(gameObject.name, Vector3.zero, Quaternion.identity);
        userRockObject.transform.localScale = Vector3.one * 0.1f;
        userRockObject.SetChildPosition(team1StartPoint, "RockObject");

        Debug.Log(playerTeamNumber);
        Debug.Log(playerTeamNumber.Split('_')[0]);

        string playerNum = playerTeamNumber.Split('_')[0];

        // ! Photon
        FindMyViewID();
        if (playerNum == "Player1" || playerNum == "Player2")
        {
            if (userRockObject.GetComponent<PhotonView>().IsMine == true)
            {
                CameraManager.Instance.SetRockCamera(userRockObject, team1StartPoint);
            }
        }
        else if (playerNum == "Player3" || playerNum == "Player4")
        { 
            if (userRockObject.GetComponent<PhotonView>().IsMine == true)
            {
                CameraManager.Instance.SetRockCamera(userRockObject, team2StartPoint);
            }
        }

        // ! Photon
        //CycleManager.cycleManager.CheckTeamAndSaveQueue(dataContainerView.ViewID.ToString(), userRockObject);

        // 팀 1인지 2인지 구별하는 if가 필요합니다
        // 팀2꺼 startPoint 없습니다. 밑은 1번팀꺼입니다
        #region Legacy
        //Vector3 startPointTransform = new Vector3(210f, 32f, 85f);
        //Vector3 cameraTransform = new Vector3(210f, 32f, 82f);
        //userRock.transform.position = startPointTransform;
        //Debug.LogFormat("스타트 포인트 : {0}",startPointTransform);
        //Debug.LogFormat("유저 락 포지션 : {0}",userRock.transform.position);
        //GameObject rockCamera = FindTopLevelGameObject("RockCamera");
        //GameObject rockCamera = FindTopLevelGameObject("NewRockCamera");
        //CinemachineVirtualCamera virtualRockCamera = rockCamera.GetComponent<CinemachineVirtualCamera>();
        //CinemachineFreeLook virtualRockCamera = rockCamera.GetComponent<CinemachineFreeLook>();
        //virtualRockCamera.transform.position = cameraTransform;
        //virtualRockCamera.Follow = userRock.transform;
        //virtualRockCamera.LookAt = userRock.transform;
        #endregion
    }


    // ! Photon
    private void FindMyViewID()
    {
        foreach (var mydata in PhotonNetwork.CurrentRoom.CustomProperties)
        {
            if (mydata.Key.ToString() == CycleManager.cycleManager.dataContainer.GetComponent<PhotonView>().ViewID.ToString())
            {
                playerTeamNumber = mydata.Value.ToString();
            }
        }
    }


    #region 검색용 함수
    public TextMeshProUGUI FindUnitTextById(int id_)
    {
        List<GameObject> textObjs = new List<GameObject>();
        textObjs = Global_PSC.FindAllTargets("DefenceUI", "unitSelect");

        GameObject targetText = default;

        foreach (var textObj in textObjs)
        {
            if (textObj.name.Contains(id_.ToString()) == true)
            {
                targetText = textObj;
            }
        }
        TextMeshProUGUI unitCountTxt = targetText.GetComponentInChildren<TextMeshProUGUI>();
        return unitCountTxt;
    }

    public GameObject FindUnitGameObjById(int id_)
    {
        List<GameObject> objs = new List<GameObject>();
        objs = Global_PSC.FindAllTargets("DefenceUI", "unitSelect");
        GameObject targetObj = default;

        foreach (var obj in objs)
        {
            if (obj.name.Contains(id_.ToString()) == true)
            {
                targetObj = obj;
            }
        }
        return targetObj;
    }
    #endregion
}

