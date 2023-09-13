using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBase : MonoBehaviour
{
    public ObstacleStatus status;
    protected MeshFilter obstacleMeshFilter;
    protected Rigidbody obstacleRigidBody;
    protected Animator obstacleAnimator;

    public ObstacleBase Build(Vector3 position, Quaternion rotate)
    {
        ObstacleBase obstacle = Instantiate(this, position, rotate);
        obstacle.transform.localScale = Vector3.one * .1f;

        return obstacle;
    }

    protected virtual void Init()
    {
        status = new ObstacleStatus(status);
        obstacleMeshFilter = GetComponent<MeshFilter>();
        obstacleRigidBody = GetComponent<Rigidbody>();
        obstacleAnimator = GetComponent<Animator>();
    }

    protected virtual void SearchPlayer(){}



    protected virtual void Dead(){}
}
