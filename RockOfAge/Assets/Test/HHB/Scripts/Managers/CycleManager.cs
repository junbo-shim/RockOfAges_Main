using PlayFab.GroupsModels;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public enum UserState
{ 
    UnitSelect = 0, RockSelect = 1, Defence = 2, WaitForRock = 3, Attack = 4, Ending = 5 
}

public class CycleManager : MonoBehaviour
{
    public static CycleManager cycleManager;

    #region 변수
    public int userState;
    // 공격에서 공이 선택됨 bool
    public bool attackRockSelected = false;
    // 공 생성 시간이 다 경과됨을 감지하는 bool
    public bool isRockCreated = false;
    ////! 서버 team1 team2 체력
    //public float team1Hp = 1000f;
    //public float team2Hp = 1000f;
    ////! 서버 player gold
    //public int gold = 1000; 
    #endregion

    public void Awake()
    {
        cycleManager = this;
        userState = (int)UserState.UnitSelect;
    }

    private void Update()
    {
        GameCycle();
    }

    #region GameCycle
    //{ GameCycle()
    public void GameCycle()
    {
        UpdateSelectionCycle();
        UpdateCommonUICycle();
        // 돌 선택 판별

        UpdateDefenceCycle();
        ChangeStateDefenceToAttack();

        //UpdateGameEndCycle();

    }
    //} GameCycle()
    #endregion


    #region SelectCycle
    //{ UpdateSelectionCycle()
    // 선택 사이클
    // 공하나 이상 선택 & 유저 enter -> defence
    public void UpdateSelectionCycle()
    {
        if (userState == (int)UserState.UnitSelect)
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
                    UIManager.uiManager.PrintRockSelectUI();
                    ItemManager.itemManager.userRockChoosed[0] = 0;
                    UIManager.uiManager.InstantiateRockImgForAttack();
                    UIManager.uiManager.InstantiateUnitImgForDenfence();
                    UIManager.uiManager.ShutDownUserSelectUI();
                    userState = (int)UserState.RockSelect;
                    UIManager.uiManager.TurnOnCommonUI();
                }
                else { return; }
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
    //{ UpdateDefenceCycle()
    // 게임종료시까지 켜져 있는 UI
    public void UpdateCommonUICycle()
    {
        // 유닛 선택단계와 엔딩 단계가 아니라면 항상 출력
        if (userState != (int)UserState.UnitSelect || userState != (int)UserState.Ending)
        { 
              UIManager.uiManager.GetRotationKey();
        }
    }
    //} UpdateDefenceCycle()
    #endregion

    #region AttackCycle
    //{ ChangeCycleAttackToDefence()
    // 공격 싸이클에서 방어 싸이클로 전환하는 함수
    public void ChangeCycleAttackToDefence()
    {
        if (userState == (int)UserState.Attack)
        {
            userState = (int)UserState.Defence;
        }
        else { Debug.Log("GAMELOGIC ERROR"); }
    }
    //} ChangeCycleAttackToDefence()
    #endregion


    #region DefenceCycle
    //{ UpdateDefenceCycle()
    public void UpdateDefenceCycle()
    {
        // 돌 선택이 되었을 때
        if (userState == (int)UserState.Defence)
        {
            userState = (int)UserState.WaitForRock;
            if (userState == (int)UserState.WaitForRock)
            {
                StartCoroutine(WaitForRock());
            }


        }
    }
    //} UpdateDefenceCycle()

    public void ChangeStateDefenceToAttack()
    {
        // 소환시간초과시 C 누르면
        if (isRockCreated == true && Input.GetKey(KeyCode.C))
        {
            Debug.Log("공격으로 전환");
            // 돌 소환하고 카메라 쫓게 해주고
            userState = (int)UserState.Attack;
            isRockCreated = false;
        }
    }


    IEnumerator WaitForRock()
    {
        float coolDown = default;
        ResourceManager.Instance.GetRockCoolDownFromId(ItemManager.itemManager.userRockChoosed[0],
            ResourceManager.Instance.GetGameObjectByID(ItemManager.itemManager.userRockChoosed[0]), out coolDown);
        Debug.LogFormat("선택된 돌 : {0}",ItemManager.itemManager.userRockChoosed[0]);
        Debug.LogFormat("GameObject Name : {0}", ResourceManager.Instance.GetGameObjectByID(ItemManager.itemManager.userRockChoosed[0]).name);
        yield return new WaitForSeconds(coolDown);
        isRockCreated = true;
    }


    #endregion


    #region GameEndCycle
    //{ UpdateGameEndCycle()
    public void UpdateGameEndCycle()
    {
        if (userState == (int)UserState.Ending)
        { 
        
        }
    }
    //} UpdateGameEndCycle()
    #endregion
}
