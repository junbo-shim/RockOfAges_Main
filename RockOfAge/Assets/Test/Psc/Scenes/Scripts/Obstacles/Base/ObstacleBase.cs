using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBase : MonoBehaviour
{
    //obstacle의 베이스

    //스테이터스
    public ObstacleStatus status;

    //기본 데이터
    protected MeshFilter obstacleMeshFilter;
    protected Rigidbody obstacleRigidBody;
    protected Animator obstacleAnimator;

    //타겟
    protected GameObject target;

    //맵에 Build
    public ObstacleBase Build(Vector3 position, Quaternion rotate)
    {
        ObstacleBase obstacle = Instantiate(this, position, rotate);
        obstacle.transform.localScale = Vector3.one * .1f;

        return obstacle;
    }

    //초기화
    protected virtual void Init()
    {
        status = new ObstacleStatus(status);
        obstacleMeshFilter = GetComponent<MeshFilter>();
        obstacleRigidBody = GetComponent<Rigidbody>();
        obstacleAnimator = GetComponent<Animator>();
    }

    //타겟 서치
    //아마 상위에 없어도 될거로 추정됨
    protected virtual void SearchTarget(){}

    //죽음
    protected virtual void Dead(){}

    //공격 활성화
    protected virtual void ActiveAttack() { }
    protected virtual void EndAttack(){ }
    
}
