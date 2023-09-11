

using UnityEngine;

public class TestObstacle : MonoBehaviour
{
    public TestObstacle Build(Vector3 position, Quaternion rotate)
    {
        TestObstacle obstacle = Instantiate(this, position, rotate);
        obstacle.transform.localScale = Vector3.one;

        return obstacle;
    }
}
