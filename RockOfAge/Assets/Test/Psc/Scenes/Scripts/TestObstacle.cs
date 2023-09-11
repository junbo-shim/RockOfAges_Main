

using UnityEngine;

public class TestObstacle : MonoBehaviour
{
    public Vector2Int size = default;

    private void Awake()
    {
        
    }

    public TestObstacle Build(Vector3 position, Quaternion rotate)
    {
        TestObstacle obstacle = Instantiate(this, position, rotate);
        obstacle.transform.localScale = Vector3.one * .1f * size.x;

        return obstacle;
    }
}
