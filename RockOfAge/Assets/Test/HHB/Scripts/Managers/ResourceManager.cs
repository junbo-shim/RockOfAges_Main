using Cinemachine;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResourceManager : GlobalSingleton<ResourceManager>
{
    // 돌 리소스
    public Dictionary<int, GameObject> rockResources = new Dictionary<int, GameObject>();
    // 유닛 리소스
    public Dictionary<int, GameObject> unitResources = new Dictionary<int, GameObject>();

    protected override void Awake()
    {
        PackAwake();
    }

    //{ AwakePack()
    // 리소스에서 인잇
    public void PackAwake()
    {
        for (int i = 1; i < 5; i++)
        {
            GameObject rockPrefab = Resources.Load<GameObject>("Rocks/" + i);
            if (rockPrefab != null)
            {
                rockResources[i] = rockPrefab;
            }
            else { Debug.Log("Failed To Load Rock" + i);}
        }
        for (int i = 11; i < 13; i++)
        {
            GameObject unitPrefab = Resources.Load<GameObject>("Units/" + i);
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
                //Debug.Log(unitResources[id_]);
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
            //Debug.Log(coolDown_);
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
        GameObject team1 = FindTopLevelGameObject("Team1");
        GameObject userRock = Instantiate(gameObject, team1.transform);
        // 팀 1인지 2인지 구별하는 if가 필요합니다
        // 팀2꺼 startPoint 없습니다. 밑은 1번팀꺼입니다

        Vector3 startPointTransform = new Vector3(210f, 32f, 85f);
        Vector3 cameraTransform = new Vector3(210f, 32f, 82f);
        userRock.transform.position = startPointTransform;
        GameObject rockCamera = FindTopLevelGameObject("RockCamera");
        //GameObject rockCamera = FindTopLevelGameObject("NewRockCamera");
        CinemachineVirtualCamera virtualRockCamera = rockCamera.GetComponent<CinemachineVirtualCamera>();
        //CinemachineFreeLook virtualRockCamera = rockCamera.GetComponent<CinemachineFreeLook>();
        virtualRockCamera.transform.position = cameraTransform;
        virtualRockCamera.Follow = userRock.transform;
        virtualRockCamera.LookAt = userRock.transform;
    }

    #region 검색용 함수
    public GameObject FindTopLevelGameObject(string name_)
    {
        GameObject[] rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in rootObjs) 
        {
            if (obj.name == name_)
            {
                return obj;
            }
        }
        return null;
    }

    public List<GameObject> FindAllTargets(string rootName, string targetName)
    {
        //Debug.Log("FindAllTargets 들어옴");
        GameObject root = FindTopLevelGameObject(rootName);
        List<GameObject> results = new List<GameObject>();

        if (root != null)
        {
            FindAllTargetsRecursive(root.transform, targetName, results);
        }
        else
        {
            Debug.LogWarning("Root GameObject not found.");
        }

        //foreach (GameObject obj in results)
        //{
        //    Debug.LogFormat("result : {0}",obj);
        //}

        return results;
    }

    private void FindAllTargetsRecursive(Transform rootTransform, string targetName, List<GameObject> results)
    {
        foreach (Transform childTransform in rootTransform)
        {
            GameObject childGameObject = childTransform.gameObject;
            if (childGameObject.name.StartsWith(targetName))
            {
                results.Add(childGameObject);
            }

            FindAllTargetsRecursive(childTransform, targetName, results);
        }

    }

    public TextMeshProUGUI FindUnitTextById(int id_)
    {
        //Debug.Log("FindUnitTextByID 들어옴");
        List<GameObject> textObjs = new List<GameObject>();
        textObjs = FindAllTargets("DefenceUI", "unitSelect");

        //foreach (GameObject objs in textObjs) 
        //{
        //    Debug.LogFormat("{0}", objs.name);
        //}

        GameObject targetText = default;

        foreach (var textObj in textObjs)
        {
            if (textObj.name.Contains(id_.ToString()) == true)
            {
                targetText = textObj;
            }
        }
        //Debug.LogFormat("{0}",targetText);

        TextMeshProUGUI unitCountTxt = targetText.GetComponentInChildren<TextMeshProUGUI>();
        return unitCountTxt;
    }

    public GameObject FindUnitGameObjById(int id_)
    {
       // Debug.Log("FindUnitGameObjById 들어옴");
        List<GameObject> objs = new List<GameObject>();
        objs = FindAllTargets("DefenceUI", "unitSelect");

        //foreach (GameObject obj in objs)
        //{
        //    Debug.LogFormat("{0}", obj.name);
        //}

        GameObject targetObj = default;

        foreach (var obj in objs)
        {
            if (obj.name.Contains(id_.ToString()) == true)
            {
                targetObj = obj;
            }
        }
        //Debug.LogFormat("{0}", targetObj);
        return targetObj;
    }
    #endregion

}
