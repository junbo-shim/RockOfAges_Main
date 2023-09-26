using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WOPObstacle : HoldObstacleBase
{
    // 공
    private new Transform transform;

    private void Awake()
    {
        Init();
    }

    //초기화
    protected override void Init()
    {
        status = new ObstacleStatus(status);
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
        PhotonNetwork.Destroy(this.gameObject);
    }

    private void TurnAroundObstacle()
    {
        transform.rotation *= Quaternion.Euler(0f, 0.2f, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            Dead();
        }
    }
}
