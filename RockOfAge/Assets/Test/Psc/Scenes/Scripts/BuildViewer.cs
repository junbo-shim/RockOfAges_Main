
using UnityEngine;

public class BuildViewer : MonoBehaviour
{
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void ChangeTarget(GameObject target)
    {
        MeshFilter _meshFilter = target.GetComponent<MeshFilter>();
        if (_meshFilter == null)
        {
            Debug.Assert(_meshFilter == null, "타겟 메쉬필터 없음");
            return;
        }

        // sourceMeshFilter에서 원본 Mesh를 가져옵니다.
        Mesh sourceMesh = _meshFilter.sharedMesh;

        // 새로운 Mesh를 생성하고 원본 Mesh를 복사합니다.
        Mesh copyMesh = new Mesh();
        copyMesh.vertices = sourceMesh.vertices;
        copyMesh.triangles = sourceMesh.triangles;
        copyMesh.normals = sourceMesh.normals;
        copyMesh.uv = sourceMesh.uv;

        // 복사된 Mesh를 새로운 MeshFilter에 할당합니다.
        meshFilter.sharedMesh = copyMesh;
        HideViewer();
    }

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
