using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleManager : MonoBehaviour
{
    public bool selection = true;
    public bool attack = false;
    public bool defence = false;

    private void Update()
    {
        UpdateSelectionCycle();
    }

    //{ UpdateSelectionCycle()
    // 공하나 이상 선택 & 유저 enter -> defence
    public void UpdateSelectionCycle()
    {
        if (selection == true)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (CheckUserBall() == true)
                {
                    UIManager.uiManager.PrintReadyText();
                    selection = false;
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

}
