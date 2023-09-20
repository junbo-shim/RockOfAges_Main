using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WOPObstacle : HoldObstacleBase
{
    //초기화
    protected override void Init()
    {
        status = new ObstacleStatus(status);// Instantiate(status);
        obstacleMeshFilter = GetComponent<MeshFilter>();
        obstacleRigidBody = GetComponent<Rigidbody>();
        obstacleAnimator = GetComponent<Animator>();
        obstacleRenderer = GetComponentInChildren<Renderer>();
        currHealth = status.Health;
    }
    //죽음
    protected override void Dead() { }

    //공격 활성화
    protected override void ActiveAttack() { }
    protected override void DeactiveAttack() { }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
