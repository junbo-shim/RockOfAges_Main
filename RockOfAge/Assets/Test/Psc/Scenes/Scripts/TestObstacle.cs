

using UnityEngine;

public class TestObstacle : MonoBehaviour
{
    public TestObstacle Build(Vector3 position, Quaternion rotate)
    {
        return Instantiate(this, position, rotate);
    }
}
