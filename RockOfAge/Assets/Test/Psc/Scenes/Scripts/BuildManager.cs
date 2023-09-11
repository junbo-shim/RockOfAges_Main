using ExitGames.Client.Photon.StructWrapping;
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
    public GameObject gridTest;


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

    public static readonly Vector3 GRID_OFFSET = new Vector3(.5f, 0, .5f);



    private void Awake()
    {
        buildState = new BitArray(MAP_SIZE_X * MAP_SIZE_Z);
        viewer = GetComponentInChildren<BuildViewer>();
        viewer.HideViewer();

        InitTerrainData();
        tmp.transform.localScale = Vector3.zero;
    }


    //현재 게임 싸이클 상태에 따라서 진입한다. 
    //커서의 위치에 따른 현재 grid 값을 재조정하고
    //현재 grid 위치에 terrain이 있을 경우,
    //해당 지형이 건설 가능한지를 판단한다.
    void Update()
    {
        ChangeCurrGrid();

        //현재 상태에 따라서 해당 스크립트를 처리할지 정한다.
        //DEFANCE 모드, 현재 위치의 그리드에 지형이 존재, 현재 건설 가능한지
        if (!IsDefance() || !IsTerrain() || !CanBuild())
        {
            viewer.HideViewer();
            return;
        }

        viewer.ShowViewer();

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("In");
            tmp.Build(currCursorGridIndex+GRID_OFFSET, Quaternion.Euler(0, (int)whereLookAt * ONCE_ROTATE_EULER_ANGLE, 0));
            buildState.Set((currCursorGridIndex.z + MAP_SIZE_Z / 2) * MAP_SIZE_Z + (currCursorGridIndex.x + MAP_SIZE_X / 2), false);
        }

    }

    //씬 실행시 map 건설 가능 상태를 init함
    //terrain 비교시 tag로 team, 기본 건설 불가 타일등을 비교하는 부분 추가해야할거같음
    //차후 회의후 추가
    void InitTerrainData()
    {
        buildState.SetAll(false);
        for (int z = -MAP_SIZE_Z / 2; z < MAP_SIZE_Z / 2; z++)
        {
            for (int x = -MAP_SIZE_X / 2; x < MAP_SIZE_X / 2; x++)
            {
                RaycastHit raycastHit;
                if (Physics.Raycast(new Vector3(x, MAP_SIZE_Y, z) + GRID_OFFSET, Vector3.down, out raycastHit, float.MaxValue, Global_PSC.FindLayerToName("Terrains")))
                {
                    GameObject gameObject = Instantiate(gridTest, raycastHit.point, Quaternion.identity);
                    buildState.Set((z + MAP_SIZE_Z / 2) * MAP_SIZE_Z + (x + MAP_SIZE_X / 2), true);
                    //Debug.LogError((i + MAP_SIZE_Z / 2) * MAP_SIZE_Z + (j + MAP_SIZE_X / 2)+"/"+buildState.Get((i + MAP_SIZE_Z / 2) * MAP_SIZE_Z + (j + MAP_SIZE_X / 2)));
                }
            }
        }
    }


    void ChangeCurrGrid()
    {
        //마우스 커서는 기본적으로 (좌하단 0,0)을 기준으로 계산된다.
        //그렇기 때문에 마우스 커서를 가져온뒤 이를 월드 좌표로 변환한다.

        Vector3 mouseWorldPos = Global_PSC.GetWorldMousePositionFromMainCamera(Camera.main.transform.position.y);

        Vector3Int _currCursorGridIndex = new Vector3Int(Mathf.FloorToInt(mouseWorldPos.x), 0, Mathf.FloorToInt(mouseWorldPos.z));
        if(currCursorGridIndex != _currCursorGridIndex)
        {
            currCursorGridIndex = _currCursorGridIndex;
            ChangeBuildPosition();
        }
    }

    void ChangeBuildTarget(TestObstacle target)
    {
        viewer.ChangeTarget(target.gameObject);
    }


    //grid 정보가 바뀔때마다 불러온다.
    //target의 위치를 갱신한다.
    void ChangeBuildPosition()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(currCursorGridIndex+ GRID_OFFSET + Vector3.up * MAP_SIZE_Y, Vector3.down, out raycastHit, float.MaxValue, Global_PSC.FindLayerToName("Terrains")))
        {
            Vector3 newPosition = raycastHit.point + transform.up * (viewer.GetHeight() / 2);
            transform.position = newPosition;
        }
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


    bool IsDefance()
    {
        return true;
    }

    //현재 그리드 위치에 건설 가능한 지형이 존재하는지 체크
    bool IsTerrain()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(currCursorGridIndex + GRID_OFFSET+ Vector3.up * MAP_SIZE_Y, Vector3.down, out raycastHit, float.MaxValue, Global_PSC.FindLayerToName("Terrains")))
        {
            return true;
        }

        return false;
    }

    //아이템의 limit상태와 해당 terrain의 건설 가능 상태를 &&한다.
    bool CanBuild()
    {
        return GetBuildEnable(currCursorGridIndex) && GetItemLimitState();
    }

    //현재 건설된 아이템의 최대 건설 개수와 현재 건설 개수를 비교한다.
    //true : 현재 건설 개수가 최대 건설보다 낮다
    bool GetItemLimitState()
    {
        return true;
    }

    //현재 grid위치의 주변 위치의 terrain의 상태를 전부 비교
    bool GetBuildEnable(Vector3Int grid, Vector2 buildSize)
    {
        //for

        return buildState.Get((grid.z + MAP_SIZE_Z / 2) * MAP_SIZE_Z + (grid.x + MAP_SIZE_X / 2));
    }
    bool GetBuildEnable(Vector3Int grid)
    {
        return GetBuildEnable(grid, Vector2.one);
    }
}





public enum BuildRotateDirection
{
    UP = 0,
    RIGHT = 1,
    DOWN = 2,
    LEFT = 3
}
