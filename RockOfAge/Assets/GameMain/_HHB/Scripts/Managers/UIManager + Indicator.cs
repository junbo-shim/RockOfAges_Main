using System.Collections.Generic;
using UnityEngine;


// indicator
public partial class UIManager : MonoBehaviour
{
    [Header("INDICATORS")]
    public GameObject endIndicator;
    public GameObject startIndicator;
    public GameObject enemyRock1Indicator;
    public GameObject enemyRock2Indicator;
    private Vector2 leftUp;
    private Vector2 rightUp;
    private Vector2 leftDown;
    private Vector2 rightDown;

    public void Update()
    {
        TurnOnConditions();
        PrintMyGold(NetworkManager.Instance.myDataContainer.GetComponent<PlayerDataContainer>().playerGold);
    }


    public void GetDirection()
    {
        leftUp = (Vector2.left + Vector2.up).normalized;
        leftDown = (Vector2.left + Vector2.down).normalized;
        rightUp = (Vector2.right + Vector2.up).normalized;
        rightDown = (Vector2.right + Vector2.down).normalized;
    }

    public void TurnOnConditions()
    {
        if (CycleManager.cycleManager.userState == (int)UserState.DEFENCE)
        {
            //------------------------- rockIndicator 시간남으면 ------------------------------
            //만약 상대 돌이 있다면 enemyRock1Indicator, enemyRock2Indicator2를 켜야합니다.
            //if(//조건)
            //{
            // 돌들이 update에서 항상 자기 좌표를 쏴야합니다.
            // Global_PSC.ConvertVec3ToScreenVec2(Vector3 공위치)를 통해서 지정해주고
            //enemyRock1Indicator.SetActive(true);
            // GetDistanceAndSwitchIndicator(enemyRock1Indicator, midScreen , rock1Position);
            //enemyRock2Indicator.SetActive(true);
            // GetDistanceAndSwitchIndicator(enemyRock1Indicator, midScreen , rock2Position);
            //}
            //else
            //{
            //enemyRock1Indicator.SetActive(false);
            //enemyRock2Indicator.SetActive(false);
            //}
            //------------------------- rockIndicator 시간남으면 ------------------------------

            //-----------------------------Start, End 지점 Indicator-------------------------
            //if(team2)
                //Vector3 startPos = Global_PSC.ConvertVec3ToScreenVec2(ResourceManager.team2StartPos);
                //Vector3 endPos = Global_PSC.ConvertVec3ToScreenVec2(ResourceManager.team2EndPos);
                //Vector3 midScreen = Global_PSC.ConvertVec3ToScreenVec2(CameraManager.myCameraPosition);
                //List<GameObject> findEndIndicator = Global_PSC.FindAllTargets("DefenceUI", "EndIndicator");
                //GameObject myEndIndicator = findEndIndicator[0];
                //RootIndicator rootEndIndicator = myEndIndicator.GetComponent<RootIndicator>();
                //rootEndIndicator.movePoint = endPos;
                //List<GameObject> findStartIndicator = Global_PSC.FindAllTargets("DefenceUI", "StartIndicator");
                //GameObject myStartIndicator = findStartIndicator[0];
                //RootIndicator rootStartIndicator = myStartIndicator.GetComponent<RootIndicator>();
                //rootStartIndicator.movePoint = startPos;
                //GetDistanceAndSwitchIndicator(endIndicator, midScreen, endPos);
                //GetDistanceAndSwitchIndicator(startIndicator, midScreen, startPos);

            //if(team1)
            //{ 
                Vector3 startPos = Global_PSC.ConvertVec3ToScreenVec2(ResourceManager.team1StartPos);
                Vector3 endPos = Global_PSC.ConvertVec3ToScreenVec2(ResourceManager.team1EndPos);
                Vector3 midScreen = Global_PSC.ConvertVec3ToScreenVec2(CameraManager.myCameraPosition);
                List<GameObject> findEndIndicator = Global_PSC.FindAllTargets("DefenceUI", "EndIndicator");
                GameObject myEndIndicator = findEndIndicator[0];
                RootIndicator rootEndIndicator = myEndIndicator.GetComponent<RootIndicator>();
                rootEndIndicator.movePoint = endPos;
                List<GameObject> findStartIndicator = Global_PSC.FindAllTargets("DefenceUI", "StartIndicator");
                GameObject myStartIndicator = findStartIndicator[0];
                RootIndicator rootStartIndicator = myStartIndicator.GetComponent<RootIndicator>();
                rootStartIndicator.movePoint = startPos;
                GetDistanceAndSwitchIndicator(endIndicator, midScreen, endPos);
                GetDistanceAndSwitchIndicator(startIndicator, midScreen, startPos);            
            //}
        }
    }

    public void GetDistanceAndSwitchIndicator(GameObject myIndicator, Vector2 midScreen, Vector2 targetPosition)
    {
        if (Vector2.Distance(midScreen, targetPosition) > 100f)
        {
            myIndicator.SetActive(true);
            MoveIndicator(myIndicator, midScreen, targetPosition);
        }
        else { myIndicator.SetActive(false); }
    }

    public void MoveIndicator(GameObject userIndicator, Vector2 midScreen, Vector3 targetPosition)
    {
        Vector2 targetV2 = Global_PSC.ConvertVec3ToScreenVec2(targetPosition);
        Vector2 nomVec = (midScreen-targetV2).normalized;
        DefineIndicatorPosition(nomVec, userIndicator);
    }

    public void DefineIndicatorPosition(Vector2 nomVec, GameObject userIndicator)
    {
        float dotLeftUp = Vector2.Dot(nomVec, leftUp);
        float dotRightUp = Vector2.Dot(nomVec, rightUp);
        float dotLeftDown = Vector2.Dot(nomVec, leftDown);
        float dotRightDown = Vector2.Dot(nomVec, rightDown);

        bool isBetweenLeftUpRightUp = (dotLeftUp >= 0 && dotLeftUp <= 1) && (dotRightUp >= 0 && dotRightUp <= 1);
        bool isBetweenLeftUpLeftDown = (dotLeftUp >= 0 && dotLeftUp <= 1) && (dotLeftDown >= 0 && dotLeftDown <= 1);
        bool isBetweenRightUpRightDown = (dotRightUp >= 0 && dotRightUp <= 1) && (dotRightDown >= 0 && dotRightDown <= 1);
        bool isBetweenLeftDownRightDown = (dotLeftDown >= 0 && dotLeftDown <= 1) && (dotRightDown >= 0 && dotRightDown <= 1);

        //Debug.Log("leftdown :"+dotLeftDown);  
        //Debug.Log("leftup :"+dotLeftUp);
        //Debug.Log("rightdown :"+dotRightDown);
        //Debug.Log("rightup :"+dotRightUp);

        // 초기화
        Quaternion resetRotation = Quaternion.Euler(0, 0, 0);
        Vector2 resetPosition = Vector2.zero;
        RectTransform rectTransform = userIndicator.GetComponent<RectTransform>();
        rectTransform.transform.rotation = resetRotation;
        rectTransform.transform.position = resetPosition;

        // 경우에 따라 로테이션값과 위치값 받기
        Vector2 newPosition = default;
        Quaternion newRotation = default;
        //float disX = default;
        if (isBetweenLeftUpRightUp)
        {
            //Debug.Log("isBetweenLeftUpRightUp");
            // nomVec가 leftUp과 rightUp 사이에 있음
            newPosition.x = 1000f;
            newPosition.y = 150f;
            newRotation = Quaternion.Euler(0, 0, 180f);
        }
        else if (isBetweenLeftUpLeftDown)
        {
            //Debug.Log("isBetweenLeftUpLeftDown");
            // nomVec가 leftUp과 leftDown 사이에 있음
            newPosition.x = 1750f;
            newPosition.y = 400f;
            newRotation = Quaternion.Euler(0, 0, -90f);
        }
        else if (isBetweenRightUpRightDown)
        {
            //Debug.Log("isBetweenRightUpRightDown");
            // nomVec가 rightUp과 rightDown 사이에 있음
            newPosition.x = 150f;
            newPosition.y = 500f;
            newRotation = Quaternion.Euler(0, 0, 90f);
        }
        else if (isBetweenLeftDownRightDown)
        {
            //Debug.Log("isBetweenLeftDownRightDown");
            // nomVec가 leftDown과 rightDown 사이에 있음
            newPosition.x = 1000f;
            newPosition.y = 900f;
            newRotation = Quaternion.Euler(0, 0, 0f);
            if (userIndicator.name == "EndIndicator")
            {
                newPosition.x += 200f;
            }
            else { newPosition.x += 100f; }
        }

        //값적용
        rectTransform.transform.position = newPosition;
        rectTransform.rotation *= newRotation;

        //mainCamera.ScreenToWorldPoint
        //mainCamera.WorldToViewportPoint
    }
}
