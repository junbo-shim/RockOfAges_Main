
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
        meshFilter.sharedMesh = target.GetComponent<MeshFilter>().sharedMesh;
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
                float height = bounds.size.y;

                // 결과 출력
                Debug.Log("Object Height: " + height);

                return height;
            }
        }

        return 0f;
    }
}
