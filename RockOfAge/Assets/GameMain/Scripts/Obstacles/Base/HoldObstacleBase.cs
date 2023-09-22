using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldObstacleBase : ObstacleBase
{
    //움직이지 않기 때문에 constraint에서 이동을 막고
    //y축 기반으로만 회전 가능하게 한다.

    // Awake에서 Init은 알아서 하시길 바랍니다.

    protected override void Init()
    {
        base.Init();
        obstacleRigidBody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        obstacleRigidBody.isKinematic = true;
    }
}
