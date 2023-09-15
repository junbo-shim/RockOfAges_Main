using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineMeshBuilder : MonoBehaviour
{
    //build시에 사용할 material
    public List<Material> materials = default;

    //spline을 통해서 만들어낼 side의 정점
    List<Vector3> rightPoint = default;
    List<Vector3> leftPoint = default;

    //spline의 정보를 가지고 있는 container
    //자동으로 불러옴
    private SplineContainer[] splineContainer = default;

    //분할할 spline 갯수 
    [SerializeField]
    private int resolution = 100;
    //옆면의 높이
    [SerializeField]
    private float height = 4f;
    //ground의 폭의 반(실제 크기는 m_width*2)
    [SerializeField]
    private float width = 3f;

    //플레이할 맵의 팀 갯수
    //일단 테스트용으로 1개만 생성
    public const int TEAM_COUNT = 1;


    private void Awake()
    {
        //팀 숫자만큼 배열 크기 지정
        splineContainer = new SplineContainer[TEAM_COUNT];

        //팀 숫자만큼 for문
        //해당 for문은 제일 상단에서부터 검색해 splineContainer를 가져온다
        for(int teamIndex = 0; teamIndex < TEAM_COUNT; teamIndex++)
        {
            splineContainer[teamIndex] = GameObject.Find("Team"+(teamIndex + 1)).transform.Find("BaseTerrains").GetComponentInChildren<SplineContainer>();
        }
    }

    void Start()
    {
        //시작시 생성 시작.
        //아마 추후에는 해당 클래스 자체가 editor영역으로 변경되서 맵 object만 생성/추출해 사용할것
        for(int teamIndex = 0; teamIndex < TEAM_COUNT; teamIndex++)
        {
            Generate(teamIndex);
        }
    }

    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!테스트 코드
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Generate(0);
        }
    }
    /// //////////////////////////////////////////////

    void Generate(int teamIndex)
    {
        //spline에서 나눠진만큼 생성한다.
        for (int splinceIndex = 0; splinceIndex < splineContainer[teamIndex].Splines.Count; splinceIndex++)
        {
            //구조가독성을 위한 부모클래스 생성
            GameObject partsParent = new GameObject();

            //데이터 초기화
            partsParent.name = "SplineParts_" + splinceIndex;
            Global_PSC.InitLocalTransformData(partsParent.transform, splineContainer[teamIndex].transform.parent);

            //실행
            Build(splineContainer[teamIndex].Splines[splinceIndex], partsParent);
        }

    }


    //다음의 단계를 따른다.
    //1. 정점 구하기
    //2. 해당 메쉬를 저장할 클래스 생성
    //3. 메쉬 만들기
    void Build(Spline spline, GameObject parent)
    {
        //1. 정점 구하기
        GetVerts(spline);

        //2. 해당 메쉬를 저장할 클래스 생성
        GameObject up = ConcreateGameObject(materials[0], parent);
        //3. 메쉬만들기
        BuildUp(up, spline.Closed);

        GameObject right = ConcreateGameObject(materials[1], parent);
        BuildSide(right, rightPoint, spline.Closed);

        GameObject left = ConcreateGameObject(materials[1], parent);
        //culling을 위해서 순서 변경
        leftPoint.Reverse();
        BuildSide(left, leftPoint, spline.Closed);

        //로딩 완료시
        BuildManager.instance.InitTerrainData();
    }

    //spline의 데이터를 기반으로 정점 생성
    void GetVerts(Spline spline)
    {
        // 오른쪽/왼쪽 정점을 저장한 list 새로 생성
        rightPoint = new List<Vector3>();
        leftPoint = new List<Vector3>();

        // 생성할 정점의 간격을 정한다.
        int splineCount = spline.Count;
        if (resolution < splineCount)
        {
            splineCount = resolution;
        }
        float step = (splineCount / (float)resolution);

        //각 간격마다의 정점 생성
        for (int i = 0; i < resolution; i++)
        {
            float t = step * i;

            //해당 spline의 변위값(t)에 대한 정보를 가져온다.
            SampleSplineWidth(spline, t, out Vector3 tmpRightPoint, out Vector3 tmpLeftPoint);

            //가져온 데이터 저장
            rightPoint.Add(tmpRightPoint);
            leftPoint.Add(tmpLeftPoint);
        }
    }

    //해당 spline의 변위값(time)에 대한 정보를 가져온다.
    public void SampleSplineWidth(Spline spline, float time, out Vector3 rightPoint, out Vector3 leftPoint)
    {
        //Evaluate에 사용할 변수들
        float3 position;
        float3 forward;
        float3 up;

        //변위값(time)에 대한 spline의 위치를 가져온다.
        spline.Evaluate(time, out position, out forward, out up);

        //float3->vector3
        Vector3 positionVector = position;

        //윗방향(up)과 앞방향(forward)의 외적(cross,옆방향)을 구한다. 
        Vector3 right = Vector3.Cross(forward, up).normalized;

        //현재 spline의 위치에서 옆방향으로 width만큼 떨어진 point를 계산해서 저장.
        rightPoint = positionVector + (right * width);
        leftPoint = positionVector + (-right * width);
    }

    //메쉬를 저장할 object 생성
    GameObject ConcreateGameObject(Material material, GameObject parent)
    {
        //mesh와 관련된 component 붙여주기
        GameObject result = new GameObject();
        result.AddComponent<MeshFilter>();
        result.AddComponent<MeshRenderer>().material = material;
        result.AddComponent<MeshCollider>();

        //해당 object의 local정보 초기화
        Global_PSC.InitLocalTransformData(result.transform, parent.transform);
        result.layer = LayerMask.NameToLayer("Terrains");

        return result;
    }


    //위쪽 mesh생성(땅)
    //right와 left의 point 정보를 모두 사용한다.
    void BuildUp(GameObject terrain, bool isClosed)
    {
        MeshFilter meshFilter = terrain.GetComponent<MeshFilter>();
        MeshCollider meshCollider = terrain.GetComponent<MeshCollider>();

        Mesh mesh = new Mesh();
     
        //mesh의 구성요소중 vertic과 triangle의 정보를 저장할 공간 생성
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        //triangle의 순서를 표현해줄 offset값
        int offset = 0;

        //left나 right 둘중에 뭐를 쓰던 상관없다.
        //가독성을 위해 따로 저장
        int length = leftPoint.Count;

        //계산 시작
        for(int i = 1; i <=length; i++)
        {
            //이전 point와 현재 point를 가져온다.
            Vector3 p1 = rightPoint[i - 1];
            Vector3 p2 = leftPoint[i - 1];
            Vector3 p3;
            Vector3 p4;

            //만약 마지막 point일 경우
            if (i == length)
            {
                //loop가 아니라면 끝낸다.
                if (!isClosed)
                {
                    break;
                }
                //아니라면 첫 point를 가져온다.
                p3 = rightPoint[0];
                p4 = leftPoint[0];
            }
            else
            { 
                p3 = rightPoint[i];
                p4 = leftPoint[i];
            }

            //기본적으로 해당 mesh는 평면이며, 순차적으로 구성된다.(이전 정점에 연결할 필요가없다.)
            //또한 좌표를 중복 사용하기 때문에 triangle을 구성할때 이전 좌표를 고려하지않고 offset만큼 이동해서 구성해도된다. 

            offset = 4 * (i - 1);

            //만들어질 정점은 다음과 같다.
            //left  right
            //1     0
            //3/5   2/4
            //7/9   6/8
            //11    10

            //시계방향으로 구성
            int t1 = offset + 0;
            int t2 = offset + 2;
            int t3 = offset + 3;

            //시계방향으로 구성
            int t4 = offset + 3;
            int t5 = offset + 1;
            int t6 = offset + 0;

            //list대로 추가한다.
            verts.AddRange(new List<Vector3> { p1, p2, p3, p4 });
            tris.AddRange(new List<int> { t1,t2,t3,t4,t5,t6 });

        }

        //mesh에 넣는다.
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();

        //bound와 normal 재구성
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //mesh 넣는다.
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

    }


    //위쪽 mesh생성(땅)
    //right와 left의 point 정보중 하나만 사용한다.(height만큼 내려보낸다.)
    //전체적인 구조는 땅을 생성할때의 구조와 같다.
    void BuildSide(GameObject terrain, List<Vector3> point, bool isClosed)
    {
        MeshFilter meshFilter = terrain.GetComponent<MeshFilter>();
        MeshCollider meshCollider = terrain.GetComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        int offset = 0;

        int length = point.Count;

        for (int i = 1; i <= length; i++)
        {
            Vector3 p1 = point[i-1]-Vector3.up* height;
            Vector3 p2 = point[i-1];
            Vector3 p3;
            Vector3 p4;

            if (i == length)
            {
                if (!isClosed)
                {
                    break;
                }
                p3 = point[0] - Vector3.up * height;
                p4 = point[0];
            }
            else
            {
                p3 = point[i] - Vector3.up * height;
                p4 = point[i];
            }

            offset = 4 * (i - 1);

            int t1 = offset + 0;
            int t2 = offset + 2;
            int t3 = offset + 3;


            int t4 = offset + 3;
            int t5 = offset + 1;
            int t6 = offset + 0;

            verts.AddRange(new List<Vector3> { p1, p2, p3, p4 });
            tris.AddRange(new List<int> { t1, t2, t3, t4, t5, t6 });

        }
        

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

    }

}
