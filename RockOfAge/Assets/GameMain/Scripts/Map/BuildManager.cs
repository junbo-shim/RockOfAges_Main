
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;
    public ObstacleBase buildTarget;

    //!!!!!!!!!!!!!!!!!!!!!!!!!테스트 코드
    //해당 정보는 item manager에 저장될 예정??
    //상의 해 봐야함
    public GameObject gridTest;
    public Material red;
    public Material white;


    //커서의 현재 그리드위치
    private Vector3Int currCursorGridIndex = Vector3Int.zero;
    //건물이 바라보는 방향
    private BuildRotateDirection whereLookAt;
    //좌클릭한 곳의 좌표
    private Vector3Int clickGridIndex = Vector3Int.zero;
    //좌클릭한 곳의 좌표
    private Vector3Int endGridIndex = Vector3Int.zero;


    //빌드 뷰어
    //렌더러만 가진 Viewer 오브젝트
    private BuildViewer viewer;

    //해당 지형의 건설 가능 상태를 저장 
    private BitArray buildState;
    private Vector3 gridOffset;

    public bool isLeftClick = false;

    //맵 사이즈 
    public static readonly int MAP_SIZE_X = 256;
    public static readonly int MAP_SIZE_Z = 256;
    public static readonly int MAP_SIZE_Y = 50;
    public static readonly Vector3 FIXED_GRID_OFFSET = (Vector3.one - Vector3.up) * .5f;

    const int ONCE_ROTATE_EULER_ANGLE = 90;
    public static readonly Vector3Int OUT_VECTOR = new Vector3Int(-90000, 0, 0);


    private void Awake()
    {
        instance = this;

        buildState = new BitArray(MAP_SIZE_X * MAP_SIZE_Z);
        viewer = GetComponentInChildren<BuildViewer>();
        viewer.HideViewer();
    }


    //현재 게임 싸이클 상태에 따라서 진입한다. 
    //커서의 위치에 따른 현재 grid 값을 재조정하고
    //현재 grid 위치에 terrain이 있을 경우,
    //해당 지형이 건설 가능한지를 판단한다.
    void Update()
    {
        //현재 마우스의 좌표를 변경
        ChangeCurrGrid();


        //좌표 갱신
        if (Input.GetMouseButtonDown(0))
        {
            isLeftClick=true;
            //시작 좌표 저장
            clickGridIndex = currCursorGridIndex;
        }
        else if (Input.GetMouseButton(0))
        {
            //드래그 좌표 저장

            endGridIndex = currCursorGridIndex;
        }


        //테스트 코드
        if (buildTarget == null)
        {return;}
        ChangeBuildPosition();

        if (CanBuild())
        {
            if (!IsUIClick())
            {
                //우선순위 우클릭->좌클릭
                if (Input.GetMouseButtonDown(1))
                {
                    isLeftClick = false;
                    clickGridIndex = OUT_VECTOR;
                    endGridIndex = OUT_VECTOR;
                    buildTarget = null;
                    //취소
                }
                
                else if (Input.GetMouseButtonUp(0))
                {
                    //빌드 시작
                    Vector3 position;
                    Quaternion rotation = Quaternion.Euler(0, (int)whereLookAt * ONCE_ROTATE_EULER_ANGLE, 0);
                    GetViewerPosition(clickGridIndex, rotation, out position, out rotation);

                    ObstacleBase build = buildTarget.Build(position, rotation);
                    build.name = buildTarget.name + "_" + currCursorGridIndex.z + "_" + currCursorGridIndex.x;
                    SetBitArrays(clickGridIndex, buildTarget.status.Size);
                    isLeftClick = false;
                    clickGridIndex = OUT_VECTOR;
                    endGridIndex = OUT_VECTOR;
                }

                

            }


        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeBuildRotation(-1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeBuildRotation(1);
        }

        //현재 상태에 따라서 해당 스크립트를 처리할지 정한다.
        //DEFANCE 모드, 현재 위치의 그리드에 지형이 존재, 현재 건설 가능한지
        if (!IsDefance() || !IsTerrain())
        {
            viewer.HideViewer();
            return;
        }

        viewer.ShowViewer();
        viewer.UpdateMouseMove(CanBuild());

    }

    //씬 실행시 map 건설 가능 상태를 init함
    //terrain 비교시 tag로 team, 기본 건설 불가 타일등을 비교하는 부분 추가해야할거같음
    public void InitTerrainData()
    {
        //모든 좌표를 검사한다.
        buildState.SetAll(false);
        for (int z = -MAP_SIZE_Z / 2; z < MAP_SIZE_Z / 2; z++)
        {
            for (int x = -MAP_SIZE_X / 2; x < MAP_SIZE_X / 2; x++)
            {
                RaycastHit raycastHit;
                if (Physics.Raycast(new Vector3(x, MAP_SIZE_Y, z) + (Vector3.one - Vector3.up) * .5f, Vector3.down, out raycastHit, float.MaxValue, Global_PSC.FindLayerToName("Terrains")))
                {
                    //건설 불가 타일 검사
                    if (raycastHit.collider.CompareTag("Block"))
                    {
                        continue;
                    }

                    //건설 불가 타일 검사 통과시 
                    buildState.Set((z + MAP_SIZE_Z / 2) * MAP_SIZE_Z + (x + MAP_SIZE_X / 2), true);

                    ///////////////////////////////////////////////////////////테스트 코드
                    GameObject gameObject = Instantiate(gridTest, raycastHit.point + Vector3.up * 0.02f, Quaternion.FromToRotation(-Vector3.forward, raycastHit.normal));
                    gameObject.name = gridTest.name + "_" + z + "_" + x;
                    gameObject.transform.localScale = Vector3.one * 0.8f;
                    /////////////////////////////////////////////////////////////////////
                }
            }
        }
    }

    //size만큼의 건설 가능 데이터를 변경함
    void SetBitArrays(Vector3 grid, Vector2Int buildSize)
    {

        //짝수 크기와 홀수 크기마다 맵핑되는 그리드 영역이 달라지기 때문에 적당한 계산 값을 도출해냈음
        //크기 3 일 경우
        //1.5 ~ -1.5 -> 1 ~ -1

        //크기가 2 일 경우
        //1 ~ -1 -> 1 ~ 0
        for (int y = (int)(buildSize.y * .5f); y > -(buildSize.y * .5f); y--)
        {
            for (int x = (int)(buildSize.x * .5f); x > -(buildSize.x * .5f); x--)
            {
                //입력된 그리드를 기반으로 그리드 위치 변경
                Vector3 _grid = grid - Vector3Int.right * x - Vector3Int.forward * y;

                int size = (int)((_grid.z + MAP_SIZE_Z * .5f) * MAP_SIZE_Z + (_grid.x + MAP_SIZE_X * .5f));
                if (size < 0 || MAP_SIZE_X * MAP_SIZE_Z <= size)
                {
                    return;
                }
                //건설 가능 데이터 변경
                buildState.Set(size, false);

                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!Test 코드
                //gameobject에 접근

                GameObject floor = GameObject.Find("GridTestCube_" + _grid.z + "_" + _grid.x);
                if(floor!=null)
                    floor.GetComponent<MeshRenderer>().material = red;
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            }
        }

    }


    //각 grid를 순회하며 size만큼의 건설 가능 데이터를 변경함
    void SetBitArrays(List<Vector3> grids, Vector2Int buildSize)
    {
        for (int i = 0; i < grids.Count; i++)
        {
            SetBitArrays(grids[i], buildSize);
        }

    }

    //obstacle, 즉 장애물이 변경될시 불러온다.
    public void ChangeBuildTarget(ObstacleBase target)
    {
        buildTarget = target;

        //맵핑되는 그리드의 좌표를 변경하기 위해 offset 설정
        //짝수
        //0, 0
        //홀수
        //0.5, 0.5
        gridOffset = new Vector3((buildTarget.status.Size.x) % 2 * .5f, 0, (buildTarget.status.Size.y) % 2 * .5f);
        viewer.UpdateTargetChange(target);
        //gridOffset = new Vector3((int)(buildTarget.size.x + 1) % 2 * .5f, 0, (int)(buildTarget.size.y + 1) % 2 * .5f);

    }


    //마우스가 이동할때마다 불러온다.
    bool ChangeCurrGrid()
    {
        //마우스 커서는 기본적으로 (좌하단 0,0)을 기준으로 계산된다.
        //그렇기 때문에 마우스 커서를 가져온뒤 이를 월드 좌표로 변환한다.

        Vector3 mouseWorldPos = Global_PSC.GetWorldMousePositionFromMainCamera();

        Vector3Int _currCursorGridIndex;

        //짝수인지 홀수인지에 따라서 맵핑되는 그리드 좌표 달라짐.
        if (gridOffset.x == .5f)
        {
            //홀수
            _currCursorGridIndex = new Vector3Int(Mathf.FloorToInt(mouseWorldPos.x), 0, Mathf.FloorToInt(mouseWorldPos.z));
        }
        else
        {
            //짝수
            _currCursorGridIndex = new Vector3Int(Mathf.RoundToInt(mouseWorldPos.x), 0, Mathf.RoundToInt(mouseWorldPos.z));
        }

        //커서가 이전과 다를 경우 데이터를 갱신
        if (currCursorGridIndex != _currCursorGridIndex)
        {
            currCursorGridIndex = _currCursorGridIndex;
            return true;
        }
        return false;
    }

    //수정되야 한다.
    //drag 가능한 오브젝트 -> 클릭한 위치 기준으로 오브젝트가 그려져야한다.

    //그외
    //1. 클릭한 상태일 경우 - 클릭한 위치 기준으로 viwer가 그려져야 한다.
    //2. 클릭 안한 상태일 경우 - 바로바로 갱신 되어야 한다.

    //grid 정보가 바뀔때마다 불러온다.
    //viewer 위치를 갱신한다.
    void ChangeBuildPosition()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(currCursorGridIndex + gridOffset + Vector3.up * MAP_SIZE_Y, Vector3.down, out raycastHit, float.MaxValue, Global_PSC.FindLayerToName("Terrains")))
        {
            Vector3 newPosition = raycastHit.point + viewer.transform.up * (viewer.gameObject.GetHeight(.1f) * .5f);
            viewer.transform.position = newPosition;
        }
    }


    //수정할 필요가 있을수도?
    //해봐야 알듯?

    //특정키 누를시 방향 회전 시킨다.
    //이 정보는 유지된다.
    void ChangeBuildRotation(int diff)
    {
        if (whereLookAt == BuildRotateDirection.LEFT && diff == 1)
        {
            whereLookAt = BuildRotateDirection.UP;
        }
        else if (whereLookAt == BuildRotateDirection.UP && diff == -1)
        {
            whereLookAt = BuildRotateDirection.LEFT;
        }
        else
        {
            whereLookAt = whereLookAt + diff;
        }
        viewer.transform.localEulerAngles = Vector3.up * ONCE_ROTATE_EULER_ANGLE * (int)whereLookAt;
    }

    bool IsUIClick()
    {
        return false;
        return EventSystem.current.IsPointerOverGameObject();
    }

    //모드 참조
    bool IsDefance()
    {
        return true;

        if (CycleManager.cycleManager.userState == (int)UserState.DEFENCE)
        {
            return true;
        }
        return false;
    }

    //현재 그리드 위치에 건설 가능한 지형이 존재하는지 체크
    bool IsTerrain()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(currCursorGridIndex + gridOffset + Vector3.up * MAP_SIZE_Y, Vector3.down, out raycastHit, float.MaxValue, Global_PSC.FindLayerToName("Terrains")))
        {
            return true;
        }

        return false;
    }

    void SetClickPosition(Vector3Int position)
    {
        clickGridIndex = position;
    }

    //아이템의 limit상태와 해당 terrain의 건설 가능 상태를 &&한다.
    bool CanBuild()
    {
        return GetBuildEnable(currCursorGridIndex, buildTarget.status.Size) && GetItemLimitState();
    }

    bool GetViewerPosition(Vector3 position, Quaternion rotation, out Vector3 outPosition, out Quaternion outRotation)
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(position + gridOffset + Vector3.up * MAP_SIZE_Y, Vector3.down, out raycastHit, float.MaxValue, Global_PSC.FindLayerToName("Terrains")))
        {
            outPosition = raycastHit.point + Vector3.up * buildTarget.buildPositionY;
            outRotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal) * rotation;
            return true;
        }
        outPosition = OUT_VECTOR;
        outRotation = Quaternion.identity;
        return false;
    }

    //현재 건설된 아이템의 최대 건설 개수와 현재 건설 개수를 비교한다.
    //true : 현재 건설 개수가 최대 건설보다 낮다
    bool GetItemLimitState()
    {
        return true;
        // gold & limit
        float gold = default;
        int buildLimit = default;
        ResourceManager.Instance.GetUnitGoldAndBuildLimitFromID(buildTarget.status.Id, out gold, out buildLimit);
        GameObject unitButton = ResourceManager.Instance.FindUnitGameObjById(buildTarget.status.Id);
        int buildCount = unitButton.GetComponent<CreateButton>().buildCount;

        return (buildCount < buildLimit);
    }

    //현재 grid위치의 주변 위치의 terrain의 상태를 전부 비교
    bool GetBuildEnable(Vector3 grid, Vector2Int buildSize)
    {
        bool result = true;

        for (int y = (int)(buildSize.y * .5f); y > -buildSize.y * .5f; y--)
        {
            for (int x = (int)(buildSize.x * .5f); x > -buildSize.x * .5f; x--)
            {
                result = result && GetBuildEnable(grid - Vector3Int.right * (x) - Vector3Int.forward * (y));

            }
        }
        return result;
    }

    //그리드 좌표에 따라 건설 가능 데이터를 가져오는 메서드
    bool GetBuildEnable(Vector3 grid)
    {
        int size = (int)((grid.z + MAP_SIZE_Z * .5f) * MAP_SIZE_Z + (grid.x + MAP_SIZE_X * .5f));
        if(size < 0 || MAP_SIZE_X *MAP_SIZE_Z <= size)
        {
            return false;
        }
         
        return buildState.Get(size);
    }
}





public enum BuildRotateDirection
{
    UP = 0,
    RIGHT = 1,
    DOWN = 2,
    LEFT = 3
}