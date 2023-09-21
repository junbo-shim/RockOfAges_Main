using System.Collections.Generic;
using UnityEngine;


public class ItemManager : MonoBehaviour
{
    public static ItemManager itemManager;
    #region 변수
    public int[] userRockChoosed = new int[1];
    public List<int> rockSelected = new List<int>();
    public List<int> unitSelected = new List<int>();
    public int rockCount = 0;
    public int unitCount = 0;
    public int capacity = 8;
    #endregion

    private void Awake()
    {
        itemManager = this;
        DontDestroyOnLoad(itemManager);
        userRockChoosed[0] = -1;
    }

    #region Functions
    //{ CheckUserListCapacity 
    // 아이템이 갯수가 초과하는지 확인하는 함수
    public bool CheckUserListCapacity(int count_)
    {
        float userItemCount = rockCount * 2 + unitCount + count_;
        //Debug.Log(userItemCount);
        if (userItemCount > capacity)
        {
            return false;
        }
        else { return true; }
    }
    //} CheckUserListCapacity 
    

    //{ CheckItemList()
    // 아이템을 가지고 있는지 검증하는 함수
    public bool CheckItemList(int id_)
    {
        // 유저가 가지고 있다면
        if (unitSelected.Contains(id_) || rockSelected.Contains(id_))
        {
            return true;
        }
        else // 가지고 있지 않다면 
        {
            return false;
        }
    }
    //} CheckItemList()

    //{ RockRePrintHolder()
    // (돌)출력 삭제와 삭제시 출력위치를 재조정
    public void RockRePrintHolder()
    {
        // 출력된 것이 1개 밖에 없을 때
        if (rockCount <= 1)
        {
            GameObject rockHolder = GameObject.Find("RockHolder");
            if (rockHolder != null)
            {
                rockCount--;
                Destroy(rockHolder);
            }
        }
        if (rockCount > 1)
        { 
            GameObject rocks = GameObject.Find("Rocks");
            if (rocks != null)
            {
                Transform[] rockHolders = rocks.GetComponentsInChildren<Transform>();

                foreach (Transform child in rockHolders)
                {
                    if (child.name == "RockHolder")
                    {
                        GameObject destroyObj = child.gameObject;
                        Destroy(destroyObj);
                        rockCount = 0;
                    }
                }
            }
            foreach (int id in rockSelected)
            {
                RockButton rockButton = FindObjectOfType<RockButton>();
                rockButton.InstantiateRockHolder(id);
            }
        }
    }
    //} RockRePrintHolder()

    //{ UnitRePrintHolder()
    // (유닛)출력 삭제와 삭제시 출력위치를 재조정
    public void UnitRePrintHolder()
    {
        // 출력된 것이 1개 밖에 없을 때
        if (unitCount <= 1)
        {
            GameObject unitHolder = GameObject.Find("UnitHolder");
            if (unitHolder != null)
            {
                unitCount--;
                Destroy(unitHolder);
            }
        }
        if (unitCount > 1)
        {
            GameObject units = GameObject.Find("Units");
            if (units != null)
            {
                Transform[] rockHolders = units.GetComponentsInChildren<Transform>();

                foreach (Transform child in rockHolders)
                {
                    if (child.name == "UnitHolder")
                    {
                        GameObject destroyObj = child.gameObject;
                        Destroy(destroyObj);
                        unitCount = 0;
                    }
                }
            }
            foreach (int id in unitSelected)
            {
                UnitButton unitButton = FindObjectOfType<UnitButton>();
                unitButton.InstantiateUnitHolder(id);
            }
        }
    }
    //} UnitRePrintHolder()
    #endregion
}