using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringBoardObstacle : HoldObstacleBase, IHitObjectHandler
{
    private GameObject colliderParts = default;
    private bool isAttacked = false;

    public AudioSource audioSource;

    //init
    private void Awake()
    {
        base.Init();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //스프링이 튀어오를때 각도가 변하는 bone의 위치
        colliderParts = transform.GetChild(0).GetChild(1).GetChild(1).gameObject;
        //초기 animation 상태
        ActiveIdle();
    }

    //화면상에 보여질때 처음에는 흰색으로 시작.
    private void OnEnable()
    {
        StartBuild(BUILD_TIME);
    }

    //충돌처리
    private void OnCollisionEnter(Collision collision)
    {
        audioSource.Play();
        CheckState(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        CheckState(collision);
    }

    //현재 상태에 따라서 공격 시도
    private void CheckState(Collision collision)
    {
        //건설이 완료되지않았을 경우엔 작동하지않음
        if (!isBuildComplete)
        {
            return;
        }

        //돌 레이어를 가진 오브젝트라면
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            //공격중 상태가 아니라면 공격애니메이션 작동(스프링 튀어오르기)
            if (!isAttacked && obstacleAnimator.GetBool("Idle"))
            {
                ActiveAttack();
            }

            //공격중 상태라면 실제 계산 실행
            if (isAttacked)
            {
                Push(collision);
            }
        }
    }


    //공격 애니메이션 실행
    //해당 오브젝트는 공격 애니메이션이 실행되는 동안만 PUSH를 한다.
    protected override void ActiveAttack()
    {
        isAttacked = true;
        obstacleAnimator.SetBool("Idle", false);
        obstacleAnimator.SetTrigger("Active");
    }

    protected override void DeactiveAttack()
    {
        isAttacked = false;
    }

    private void ActiveIdle()
    {
        obstacleAnimator.SetBool("Idle", true);
    }

    //각도가 변하는 발판 bone을 기반으로 해당 발판의 특정 각도로 힘을 줘서 밀어낸다.
    public void Push(Collision collision)
    {
        Vector3 eulerAngle = colliderParts.transform.localEulerAngles - Vector3.right * 20 + Vector3.up * 40;
        collision.gameObject.GetComponent<Rigidbody>().AddForce(eulerAngle.normalized * 50f, ForceMode.Acceleration);
    }

    //장애물이 hit 당할시에
    public void Hit(int damage)
    {
        if (!isBuildComplete)
        {
            Delete();
        }

        Debug.Log("장애물 히트");
        if (!isAttacked)
        {
            return;
        }

        HitReaction();

        currHealth -= damage;
        if (currHealth <= 0)
        {
            Die();
        }
    }

    public void HitReaction()
    {
        throw new System.NotImplementedException();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
