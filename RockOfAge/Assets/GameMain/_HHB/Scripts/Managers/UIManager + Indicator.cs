using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

// indicator
public partial class UIManager : MonoBehaviour
{
    [Header("INDICATORS")]
    public GameObject endIndicator;
    public GameObject startIndicator;
    public GameObject enemyRock1Indicator;
    public GameObject enemyRock2Indicator;

    public void Update()
    {
        TurnOnConditions();
    }

    public void TurnOnConditions()
    {
        if (CycleManager.cycleManager.userState == (int)UserState.DEFENCE)
        {
            //만약 상대 돌이 있다면 enemyRock1Indicator, enemyRock2Indicator2를 켜야합니다.
            //if()
            //{
            //enemyRock1Indicator.SetActive(true);
            //enemyRock2Indicator.SetActive(true);
            //}
            //else
            //{
            //enemyRock1Indicator.SetActive(false);
            //enemyRock2Indicator.SetActive(false);
            //}
            //MoveIndicator(endIndicator,)
            //MoveIndicator(startIndicator,)
        }
    }

    public void MoveIndicator(GameObject indicator, GameObject nowOnCamera, Vector2 targetPosition)
    {

    }
}
