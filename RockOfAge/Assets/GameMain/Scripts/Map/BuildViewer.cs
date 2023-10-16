
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildViewer : MonoBehaviour
{

    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private MeshFilter meshFilter;
    [SerializeField]
    private BuildHighLight highLight;
    [SerializeField]
    private BuildColorHighLight colorHighLight;
    [SerializeField]
    private DragViewer dragViewer;

    private void Awake()
    {
        meshFilter = GetComponentInChildren<MeshFilter>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        highLight = GetComponentInChildren<BuildHighLight>();
        colorHighLight = GetComponentInChildren<BuildColorHighLight>();
        dragViewer = transform.parent.GetComponentInChildren<DragViewer>();
    }

    void ChangeTarget(ObstacleBase target)
    {
        SkinnedMeshRenderer skinnedMeshRenderer = default;
        MeshFilter _meshFilter = target.GetComponent<MeshFilter>();
        if (_meshFilter == null)
        {
            _meshFilter = target.GetComponentInChildren<MeshFilter>();
            if (_meshFilter == null)
            {
                skinnedMeshRenderer = target.GetComponentInChildren<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer == null)
                {
                    Debug.Log("오류");
                    return;
                }
            }
        }

        //Debug.Log(skinnedMeshRenderer);
        //Debug.Log(_meshFilter);

        // sourceMeshFilter에서 원본 Mesh를 참조
        Mesh sourceMesh;
        if (_meshFilter != null)
        {
            meshFilter.transform.localRotation = _meshFilter.transform.localRotation;
            sourceMesh = _meshFilter.sharedMesh;
        }
        else
        {
            meshFilter.transform.localRotation = skinnedMeshRenderer.transform.localRotation;
            sourceMesh = skinnedMeshRenderer.sharedMesh;
        }

        // 새로운 Mesh를 생성하고 원본 Mesh를 복사
        Mesh copyMesh = new Mesh();
        copyMesh.vertices = sourceMesh.vertices;
        copyMesh.triangles = sourceMesh.triangles;
        copyMesh.normals = sourceMesh.normals;
        copyMesh.uv = sourceMesh.uv;


        // 복사된 Mesh를 새로운 MeshFilter에 할당
        meshFilter.sharedMesh = copyMesh;

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!차후 상황에 맞게 변경할것.
        ObstacleBase _target = target.GetComponent<ObstacleBase>();

        highLight.ChangeHighLight(_target.status.Size);
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        //drag.previewObject = Instantiate(_target).gameObject;

        HideViewer();
    }


    public void UpdateColor(bool canBuild)
    {
        colorHighLight.UpdateColorHighLightColor(canBuild);
        dragViewer.UpdateColor(canBuild);
    }

    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!차후 obstacleBase로 바꿀것
    public void UpdateTarget(ObstacleBase target)
    {
        colorHighLight.UpdateColorHighLightSize(target.status.Size);
        ChangeTarget(target);
    }
    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!



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

