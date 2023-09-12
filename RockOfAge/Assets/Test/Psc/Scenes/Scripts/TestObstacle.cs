

using UnityEngine;

public class TestObstacle : MonoBehaviour
{
    public Vector2Int size = default;
    private MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    public TestObstacle Build(Vector3 position, Quaternion rotate)
    {
        TestObstacle obstacle = Instantiate(this, position+Vector3.up* GetHeight(), rotate);
        obstacle.transform.localScale = Vector3.one * .1f;

        return obstacle;
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
}
