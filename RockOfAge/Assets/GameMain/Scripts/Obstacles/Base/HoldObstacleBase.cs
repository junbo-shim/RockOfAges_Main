using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldObstacleBase : ObstacleBase
{
    //해당 클래스는 움직이지 않는 obstacle에 상속 바랍니다.

    private void Awake()
    {
        Init();
    }

    //움직이지 않기 때문에 constraint에서 이동을 막고
    //y축 기반으로만 회전 가능하게 한다.
    protected override void Init()
    {
        base.Init();
        obstacleRigidBody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        obstacleRigidBody.velocity = Vector3.zero;
        obstacleRigidBody.isKinematic = true;
    }
}
