using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    //!!!!!!!!!!!!!!!!!!!!!!!!!테스트 코드
    //해당 정보는 item manager에 저장될 예정??
    //상의 해 봐야함
    public TestObstacle tmp;


    //커서의 현재 그리드위치
    public Vector3Int currCursorGridIndex = Vector3Int.zero;
    //건물이 바라보는 방향
    BuildRotateDirection whereLookAt;
    public static readonly int ONCE_ROTATE_EULER_ANGLE = 90;


    //빌드 뷰어
    //렌더러만 가진 Viewer 오브젝트
    BuildViewer viewer;

    //해당 지형의 건설 가능 상태를 저장 
    BitArray buildState;
    //맵 사이즈 
    public static readonly int MAP_SIZE_X = 256;
    public static readonly int MAP_SIZE_Z = 256;
    public static readonly int MAP_SIZE_Y = 50;




    private void Awake()
    {
        buildState = new BitArray(MAP_SIZE_X * MAP_SIZE_Z);
        viewer = GetComponentInChildren<BuildViewer>();
        viewer.transform.localScale = Vector3.zero;

        InitTerrainData();
    }


    //현재 게임 싸이클 상태에 따라서 진입한다. 
    //커서의 위치에 따른 현재 grid 값을 재조정하고
    //현재 grid 위치에 terrain이 있을 경우,
    //해당 지형이 건설 가능한지를 판단한다.
    void Update()
    {
        //현재 상태에 따라서 해당 스크립트를 처리할지 정한다.
        //DEFANCE 모드, 현재 위치의 그리드에 지형이 존재, 현재 건설 가능한지
        if (!IsDefance() || !IsTerrain() || !CanBuild())
        {
            return;
        }
        /*
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycastHit;
                if (Physics.Raycast(ray, out raycastHit, float.MaxValue, LayerMask.NameToLayer("Terrains")))
                {
                    copyObject.enabled = true;
                    SetCurrBuildPosition(Input.mousePosition, out copyObject);
                }
        */


        if (Input.GetMouseButtonDown(1))
        {
        }

    }


    //씬 실행시 map 건설 가능 상태를 init함
    //terrain 비교시 tag로 team, 기본 건설 불가 타일등을 비교하는 부분 추가해야할거같음
    //차후 회의후 추가

    void InitTerrainData()
    {
        buildState.SetAll(false);
        for (int i = -MAP_SIZE_Z / 2; i < MAP_SIZE_Z / 2; i++)
        {

            for (int j = -MAP_SIZE_X / 2; j < MAP_SIZE_X / 2; j++)
            {
                Debug.Log(i + ", " + j);

                RaycastHit raycastHit;
                if (Physics.Raycast(new Vector3(i, MAP_SIZE_Y, j), Vector3.down, out raycastHit, float.MaxValue, LayerMask.NameToLayer("Terrains")))
                {
                    Debug.Log("Hit");
                    buildState.Set((i + MAP_SIZE_Z / 2) * MAP_SIZE_Z + (j + MAP_SIZE_X / 2), true);

                }
            }
        }
    }

    void ChangeBuilding(TestObstacle target)
    {
        viewer.ChangeTarget(target.gameObject);
    }


    //특정키 누를시 방향 회전 시킨다.
    //이 정보는 유지된다.
    void ChangeBuildRotation()
    {
        if (whereLookAt != BuildRotateDirection.LEFT)
        {
            whereLookAt = whereLookAt + 1;
        }
        else
        {
            whereLookAt = 0;
        }
        viewer.transform.localEulerAngles = Vector3.up * ONCE_ROTATE_EULER_ANGLE * (int)whereLookAt;
    }

    void ChangeBuildPosition()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(currCursorGridIndex + Vector3.up * MAP_SIZE_Y, Vector3.down, out raycastHit, float.MaxValue, LayerMask.NameToLayer("Terrains")))
        {
            Vector3 newPosition = raycastHit.point + transform.up * (viewer.GetHeight() / 2);
            transform.position = newPosition;
        }
    }

    void ChangeCurrGrid()
    {
        Vector3 cursorPos = Input.mousePosition;
        currCursorGridIndex = new Vector3Int(Mathf.RoundToInt(cursorPos.x), 0, Mathf.RoundToInt(cursorPos.y));
        Debug.Log(currCursorGridIndex.x + " / " + currCursorGridIndex.y);
    }


    bool IsDefance()
    {
        return false;
    }

    //현재 그리드 위치에 건설 가능한 지형이 존재하는지 체크
    bool IsTerrain()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(currCursorGridIndex + Vector3.up * MAP_SIZE_Y, Vector3.down, out raycastHit, float.MaxValue, LayerMask.NameToLayer("Terrains")))
        {
            return true;
        }

        return false;
    }

    //아이템의 limit상태와 해당 terrain의 건설 가능 상태를 &&한다.
    bool CanBuild()
    {
        return GetBuildEnable() && GetItemLimitState();
    }

    //현재 건설된 아이템의 최대 건설 개수와 현재 건설 개수를 비교한다.
    //true : 현재 건설 개수가 최대 건설보다 낮다
    bool GetItemLimitState()
    {
        return true;
    }

    //현재 grid위치의 주변 위치의 terrain의 상태를 전부 비교
    bool GetBuildEnable(Vector2 buildSize)
    {
        return false;
    }
    bool GetBuildEnable()
    {
        return GetBuildEnable(Vector2.one);
    }
}





public enum BuildRotateDirection
{
    UP = 0,
    RIGHT = 1,
    DOWN = 2,
    LEFT = 3
}
