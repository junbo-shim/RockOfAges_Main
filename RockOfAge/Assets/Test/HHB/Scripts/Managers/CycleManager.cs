using System.Collections;
using UnityEngine;

public enum UserState
{ 
    UNITSELECT = 0 , DEFENCE = 1, ATTACK = 2, ENDING = 3 
}

public enum RockState 
{ 
    ROCKSELECT = 0, ROCKCREATING = 1, ROCKCREATED = 2
}


public class CycleManager : MonoBehaviour
{
    public static CycleManager cycleManager;

    #region 변수
    public int userState;
    public int rockState;
    // 공격에서 공이 선택됨 bool
    public bool attackRockSelected = false;
    ////! 서버 team1 team2 체력
    //public float team1Hp = 1000f;
    //public float team2Hp = 1000f;
    ////! 서버 player gold
    //public int gold = 1000;
    #endregion

    public void Awake()
    {
        cycleManager = this;
        userState = (int)UserState.UNITSELECT;
        rockState = (int)RockState.ROCKSELECT;
    }

    private void Update()
    {
        GameCycle();
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
        if (userState == (int)UserState.UNITSELECT)
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
                    UIManager.uiManager.ChangeStateUnitSelectToRockSelect();
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
    // 공격 싸이클에서 방어 싸이클로 전환하는 함수
    public void ChangeCycleAttackToDefence()
    {
        if (userState == (int)UserState.ATTACK)
        {
            userState = (int)UserState.DEFENCE;
        }
        else { Debug.Log("GAME LOGIC ERROR"); }
    }
    //} ChangeCycleAttackToDefence()
    #endregion


    #region DefenceCycle
    //{ UpdateDefenceCycle()
    public void UpdateDefenceCycle()
    {
        int userRock = ItemManager.itemManager.userRockChoosed[0];
        if (userRock != 0 && userState == (int)UserState.DEFENCE && rockState == (int)RockState.ROCKCREATED)
        {
            ChangeStateDefenceToAttack();
        }
        else { return; }
    }

    public void ChangeStateDefenceToAttack()
    {
        // 소환시간초과시 C 누르면
        if (Input.GetKeyDown(KeyCode.C))
        {
            ResourceManager.Instance.InstatiateUserSelectedRock();
            userState = (int)UserState.ATTACK;
            rockState = (int)RockState.ROCKSELECT;
            ItemManager.itemManager.userRockChoosed[0] = 0;
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
            yield return new WaitForSeconds(coolDown);
            rockState = (int)RockState.ROCKCREATED;        
        }
    }





    #endregion


    #region GameEndCycle
    //{ UpdateGameEndCycle()
    public void UpdateGameEndCycle()
    {
        if (userState == (int)UserState.ENDING)
        { 
        
        }
    }
    //} UpdateGameEndCycle()
    #endregion
}
