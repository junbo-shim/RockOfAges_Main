
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Catapult : HoldObstacleBase
{
    public float detectionRadius = 5f; // 원형 감지 범위 반지름
    public LayerMask Rock; // 감지할 레이어 설정
    public float rotationSpeed = 1.0f; // 회전 속도 조절 변수

    public Transform throwPoint; // 돌을 던질 위치
    public GameObject rockPrefab; // 던질 돌의 프리팹
    private Quaternion initialRotation; // 투석기의 초기 로테이션
    private bool canThrowRock = true; // 돌을 던질 수 있는 상태인지 여부를 나타내는 변수
    private Vector3 targetPosition; // 감지한 객체의 위치
    public Animator animator; // 애니메이터 컴포넌트에 대한 참조
    private bool hasThrownRock = false; // 돌을 이미 발사했는지 여부를 나타내는 변수

    void Start()
    {
        Init();

        animator = GetComponent<Animator>();
        // 투석기의 초기 로테이션을 저장합니다.
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // 투석기의 현재 위치
        Vector3 catapultPosition = transform.position;

        // 원형 감지 범위 내의 모든 Collider 가져오기
        Collider[] colliders = Physics.OverlapSphere(catapultPosition, detectionRadius, Rock);
        //if (!hasThrownRock)
        {
            foreach (Collider collider in colliders)
            {
                // 감지한 객체의 위치를 전역 변수로 변경
                targetPosition = collider.transform.position;

                // 투석기를 감지한 객체(Rock)를 향하도록 회전시키기
                Vector3 directionToTarget = (targetPosition - catapultPosition).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

                animator.SetTrigger("Attack");

                // 투석기의 초기 로테이션을 기준으로 회전
                targetRotation *= initialRotation;

                //x와 z 로테이션을 초기 로테이션으로 고정
                targetRotation.eulerAngles = new Vector3(initialRotation.eulerAngles.x, targetRotation.eulerAngles.y, initialRotation.eulerAngles.z);

                // 회전 속도를 적용하여 부드럽게 회전
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                // 투석기와 돌 사이의 각도 계산
                float angleToRock = Vector3.Angle(transform.forward, directionToTarget);

                // 일직선 상에 있는지 확인하고 애니메이션을 실행하거나 종료합니다.
                float angleThreshold = 10f; // 각도 임계값을 설정합니다. 필요한 경우 이 값을 조절할 수 있습니다.
                if (angleToRock <= angleThreshold)
                {
                    animator.SetBool("Attack", true); // "IsAligned"는 애니메이터 불린 변수의 이름입니다. 원하는 이름으로 변경하세요.
                }
                else
                {
                    animator.SetBool("Attack", false);
                }
            }
        }
    }
    void OnDrawGizmos()
    {
        // 원형 감지 범위의 색상을 설정합니다.
        Gizmos.color = Color.red;

        // 원형 감지 범위를 그립니다.
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    void ThrowRock()
    {
        if (rockPrefab != null && throwPoint != null && canThrowRock)
        {
            // 돌을 throwPoint 위치에서 생성하고 발사합니다.
            GameObject rock = Instantiate(rockPrefab, throwPoint.position, throwPoint.rotation);
            Debug.Log(rock);

            // 투석기와 돌 사이의 방향 및 거리 계산
            Vector3 directionToTarget = (targetPosition - throwPoint.position).normalized;
            float distanceToTarget = (targetPosition - throwPoint.position).magnitude;

            // 발사 속도를 거리에 비례하게 설정합니다.
            float forceMultiplier = 10f;
            float force = Mathf.Sqrt(distanceToTarget * Physics.gravity.magnitude * forceMultiplier);

            // 돌을 해당 방향으로 발사합니다.
            Vector3 launchVelocity = (directionToTarget * force);
            rock.GetComponent<Rigidbody>().velocity = launchVelocity;

            //StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        // 돌을 던질 수 없는 상태로 설정
        canThrowRock = false;

        // 5초 대기
        yield return new WaitForSeconds(5f);

        // 다시 돌을 던질 수 있는 상태로 설정
        canThrowRock = true;
    }
}