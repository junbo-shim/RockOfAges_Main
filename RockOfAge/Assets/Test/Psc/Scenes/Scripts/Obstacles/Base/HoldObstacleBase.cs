using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldObstacleBase : ObstacleBase
{

    private void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        obstacleRigidBody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        obstacleRigidBody.velocity = Vector3.zero;
        obstacleRigidBody.isKinematic = true;
    }
    protected virtual void ActiveAttack() {}
}
