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
        for (int i = 1; i < 4; i++)
        {
            GameObject rockPrefab = Resources.Load<GameObject>("Rocks/" + i);
            if (rockPrefab != null)
            {
                rockResources[i] = rockPrefab;
            }
            else { Debug.Log("Failed To Load Rock" + i);}
        }
        for (int i = 11; i < 19; i++)
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
        GameObject team1 = Global_PSC.FindTopLevelGameObject("Team1");
        GameObject userRock = Instantiate(gameObject, team1.transform);
        Vector3 startPointTransform = new Vector3(114f, 32f, 108f);
        CameraManager.Instance.SetRockCamera(userRock, startPointTransform);
    }

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
}
