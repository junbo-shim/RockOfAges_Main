using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleManager : MonoBehaviour
{
    public bool selection = true;
    public bool attack = false;
    public bool defence = false;
    public bool isGameOver = false;

    private void Update()
    {
        UpdateSelectionCycle();
        UpdateCommonUICycle();
    }

    //{ UpdateSelectionCycle()
    // 선택 사이클
    // 공하나 이상 선택 & 유저 enter -> defence
    public void UpdateSelectionCycle()
    {
        if (selection == true)
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
                    selection = false;
                    defence = true;
                    UIManager.uiManager.ShutDownUserSelectUI();
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

    //{ UpdateDefenceCycle()
    // 게임종료시까지 켜져 있는 UI
    public void UpdateCommonUICycle()
    {
        if (isGameOver == false && selection == false)
        { 
            UIManager.uiManager.TurnOnCommonUI();
            UIManager.uiManager.GetRotationKey();        
        }
    }
    //} UpdateDefenceCycle()


}
