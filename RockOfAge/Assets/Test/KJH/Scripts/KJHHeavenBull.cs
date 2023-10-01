using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KJHHeavenBull : MonoBehaviour
{
    /*감지범위내에 있는지 확인
     감지범위 안에 있다면 3초후 공격
    -공격타이밍에 땅에있다면 머드에 묵임
    -공격타이밍에 땅에 없으면 공격 안당함*/



    // 감지 범위를 설정하는 변수
    public float detectionRadius = 5f;
    // 감지할 대상의 레이어를 지정하는 변수
    public LayerMask Rock;
    // 머드 프리팹
    public GameObject mudPrefab;

    public float mudDuration = 1f;
    // 가장 가까운 돌의 위치
    private Transform nearRock;
    // 공격 목표물 위치
    private Vector3 targetPosition;
    // 공격 여부 확인
    private bool isAttack = false;
    private bool isAttacking = false;
    private GameObject mudObject;
    private Animator animator;


    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        DetectRock();
    }
    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, Rock);
        if (colliders.Length > 0)
        {
            // 감지 범위 내에 돌이 있을 때 애니메이션을 실행
            animator.SetTrigger("Attack");
        }

        // Attack 애니메이션이 재생 중일 때 공격 실행
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if (!isAttacking)
            {
                isAttacking = true;
                StartCoroutine(AttackAfterAnimation());
            }
        }
    }
    public void DetectRock()
    {
        // 주변에 있는 돌을 감지하고 배열에 저장
        Collider[] colls = Physics.OverlapSphere(transform.position, detectionRadius, Rock);
        // nearDistance 변수 초기화 (가장 까가운 돌을 감지하기 위한 변수)
        float nearDistance = Mathf.Infinity;

        bool isRockDetected = false;

        foreach (Collider coll in colls)
        {
            targetPosition = coll.transform.position;
            // 두 Vector3 위치 사이의 거리를 계산하는 Unity 엔진의 함수 coll = Rcok
            float distance = Vector3.Distance(transform.position, coll.transform.position);
            if (coll.gameObject.layer == LayerMask.NameToLayer("Rock"))
            {
                if (distance < nearDistance)
                {
                    nearDistance = distance;
                    nearRock = coll.transform;
                    LookRock(coll.transform);
                    isRockDetected = true;
                }
            }
        }

        if (!isRockDetected)
        {
            isAttack = false;
        }
    }

    public void LookRock(Transform target)
    {
        if (nearRock != null)
        {
            // 현재 객체와 돌 사이의 방향 벡터 계산
            Vector3 direction = target.position - transform.position;
            // y 축 회전을 고려하지 않도록 설정
            direction.y = 0;
            // 현재 객체를 계산된 방향으로 회전시킴
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // 부드럽게 회전하기위해 보간 사용
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 0.7f);
        }
    }
    private IEnumerator AttackAfterAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Attack 애니메이션이 끝나면 공격 실행
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, Rock);
        if (colliders.Length > 0)
        {
            AttackRock();
        }

        // 공격이 끝났으므로 다음 애니메이션을 기다리기 위해 isAttacking을 false로 설정
        isAttacking = false;
    }
    public void AttackRock()
    {
        Debug.Log("공격했나?");
        // 머드가 똑바로 생성이 안되서 강제 회전
        Vector3 mudDirection = new Vector3(0, 1f, 0);
        // 땅에 붙이기위해 y값 수정
        Vector3 mudPosition = new Vector3(targetPosition.x, targetPosition.y - 0.3f, targetPosition.z);
        Quaternion mudRotation = Quaternion.LookRotation(mudDirection);
        mudObject = Instantiate(mudPrefab, mudPosition, mudRotation);

        Rigidbody mudRb = mudObject.GetComponent<Rigidbody>();
        if (mudRb != null)
        {
            mudRb.isKinematic = true;
        }
        // 1초 뒤에 머드 프리팹 삭제
        StartCoroutine(DestroyMudObject(mudObject, mudDuration));

        Rigidbody rockRb = nearRock.GetComponent<Rigidbody>();
        if (rockRb != null)
        {
            rockRb.isKinematic = true;
        }
    }
    IEnumerator DestroyMudObject(GameObject mudObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(mudObject);

        // 머드 프리팹이 삭제된 후에 바위의 Rigidbody를 다시 움직이게 설정
        Rigidbody rockRb = nearRock.GetComponent<Rigidbody>();
        if (rockRb != null)
        {
            rockRb.isKinematic = false;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
