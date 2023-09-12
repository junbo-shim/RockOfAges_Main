
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildViewer : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private BuildHighLight highLight;
    private BuildColorHighLight colorHighLight;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        highLight = GetComponentInChildren<BuildHighLight>();
        colorHighLight = GetComponentInChildren<BuildColorHighLight>();
    }

    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!차후 target의 class를 ObstacleBase로 변경할것
    void ChangeTarget(GameObject target)
    {
        MeshFilter _meshFilter = target.GetComponent<MeshFilter>();
        if (_meshFilter == null)
        {
            Debug.Assert(_meshFilter == null, "타겟 메쉬필터 없음");
            return;
        }

        // sourceMeshFilter에서 원본 Mesh를 참조
        Mesh sourceMesh = _meshFilter.sharedMesh;

        // 새로운 Mesh를 생성하고 원본 Mesh를 복사
        Mesh copyMesh = new Mesh();
        copyMesh.vertices = sourceMesh.vertices;
        copyMesh.triangles = sourceMesh.triangles;
        copyMesh.normals = sourceMesh.normals;
        copyMesh.uv = sourceMesh.uv;

        // 복사된 Mesh를 새로운 MeshFilter에 할당
        meshFilter.sharedMesh = copyMesh;

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!차후 상황에 맞게 변경할것.
        TestObstacle _target = target.GetComponent<TestObstacle>();
        highLight.ChangeHighLight(_target.size);
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        HideViewer();
    }


    public void UpdateMouseMove(bool canBuild)
    {
        colorHighLight.UpdateColorHighLightColor(canBuild);
    }

    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!차후 obstacleBase로 바꿀것
    public void UpdateTargetChange(TestObstacle target)
    {
        colorHighLight.UpdateColorHighLightSize(target.size);
        ChangeTarget(target.gameObject);
    }
    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


    public float GetHeight()
    {
        if (meshFilter != null)
        {
            // 메쉬 데이터 가져오기
            Mesh mesh = meshFilter.sharedMesh;

            if (mesh != null)
            {
                // 메쉬의 경계 상자 (Bounds) 가져오기
                Bounds bounds = mesh.bounds;

                // 높이 계산
                float height = bounds.size.y * .1f;

                // 결과 출력
                //Debug.Log("Object Height: " + height);

                return height;
            }
        }

        return 0f;
    }

    public void HideViewer()
    {
        transform.localScale = Vector3.zero;
    }
    public void HideViewer(bool enable)
    {
        if (enable)
        {
            transform.localScale = Vector3.zero;
        }
        else
        {
            transform.localScale = Vector3.one * .1f;
        }
    }


    public void ShowViewer()
    {
        transform.localScale = Vector3.one * .1f;
    }
    public void ShowViewer(bool enable)
    {
        if (enable)
        {
            transform.localScale = Vector3.one * .1f;
        }
        else
        {
            transform.localScale = Vector3.zero;
        }
    }

}
