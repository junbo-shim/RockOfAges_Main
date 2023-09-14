using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // AwakePack
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
        //for (int i = 11; i < 2; i++)
        //{
        //    GameObject unitPrefab = Resources.Load<GameObject>("Units/" + i);
        //    if (unitPrefab != null) 
        //    {
        //        unitResources[i - 11] = unitPrefab;
        //    }
        //    else { Debug.Log("Failed To Load Obstacle" + i); }
        //}
    }
   


    // 경로를 불러오기위한 제너릭 함수
    public T Load<T>(string path_) where T : Object
    { 
        return Resources.Load<T>(path_);
    }

    // 경로에서 불러낸 이름을 int로 교환하는 함수
    public bool ChangeStringToInt(string str_id_, out int int_id_)
    {
        if (int.TryParse(str_id_, out int_id_))
        {
            return true;
        }
        else
        {
            Debug.Log("Failed To Convert String To Int");
            int_id_ = -1;
            return false;
        }
    }

    // id를 넣으면 GameObject를 리턴하는 함수
    public GameObject GetGameObjectByID(int id_)
    { 
        // 돌
        if (id_ >= 0)
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
            if (unitResources.ContainsKey(id_))
            {
                return unitResources[id_];
            }
            else { Debug.Log("unitResource Error"); return null; }
        }
        else { Debug.Log("Resource Error"); return null; }   
    }

    public float GetRockCoolDownFromId(int id_, GameObject gameObject, out float coolDown_)
    {
        Debug.Log(id_);
        Debug.Log(gameObject);
        RockBase rock = gameObject.GetComponent<RockBase>();        
        // rock
        if (id_ >= 0 && id_ <= 10)
        {
            coolDown_ = rock.rockStatus.Cooldown;
            Debug.Log(coolDown_);
            return coolDown_;
        }
        else { Debug.Log("Failed To Get CoolDown"); return coolDown_ = 0f; }
    }

    public void GetUnitGoldAndBuildLimitFromID(int id_, GameObject gameObject, out float gold_, out int buildLimit_)
    {
        if (id_ > 10)
        {
            ObstacleStatus obstacleStatus = gameObject.GetComponent<ObstacleStatus>();
            gold_ = obstacleStatus.Price;
            buildLimit_ = obstacleStatus.BuildLimit;
        }
        else { Debug.Log("Failed To Get Gold & BuildLimit"); gold_ = 0f; buildLimit_ = 0; }
    }

    public void GetUnitInfoFromID()
    { 
    
    }
}
