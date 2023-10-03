using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WOPObstacle : HoldObstacleBase
{
    // 공
    private new Transform transform;

    //초기화
    protected override void Init()
    {
        base.Init();
        obstacleRigidBody = GetComponent<Rigidbody>();
        obstacleRigidBody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        obstacleRigidBody.isKinematic = true;
        transform = GetComponent<Transform>();
        currHealth = status.Health;
    }


    private void Update()
    {
        TurnAroundObstacle();
    }

    //죽음
    protected override void Dead()
    {
        Invoke("DestroyPhotonViewObject", 1f);
    }

    private void TurnAroundObstacle()
    {
        transform.rotation *= Quaternion.Euler(0f, 2f, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            Dead();
        }
    }
}
