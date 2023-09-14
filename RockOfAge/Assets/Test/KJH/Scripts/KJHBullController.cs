using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using System;

public class KJHBullController : MonoBehaviour, IHitObjectHandler
{
    public float detectionRange = default;
    public float chargeSpeed = default;
    public float health = 100f; // 체력 변수
    public float attackPower = default; // 공격력 변수
    public float chargeCool = default;
    public float walkSpeed = default;
    public LayerMask Rock;

    bool isDying = false;
    private int chargeCount = 0;
    private int rockCollisionCount = 0;// 돌과 충돌한 횟수를 추적하는 변수 추가
    private float lastChargeTime = 0f;
    private Vector3 lastRockPosition; // 돌의 마지막 위치를 저장할 변수 추가
    private Vector3 initialBullPosition;
    private bool hasCharged = false; // 모루황소가 돌진했는지 여부를 나타내는 변수 추가
    private bool isCharging = false;
    private bool isReturning = false;
    private Rigidbody bullRigidbody;
    private Transform targetRock;
    private Animator animator;

    private void Start() // Start 메서드를 선언합니다.
    {
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트를 가져옵니다.
        bullRigidbody = GetComponent<Rigidbody>(); // 리지드바디 컴포넌트를 가져옵니다.
        initialBullPosition = transform.position;
    }

    private void Update()
    {
        if(isDying == true)
        {
            return;
        }
        animator.SetBool("isCharging", isCharging);
        animator.SetBool("isReturning", isReturning);

        if (!isCharging && !isReturning)
        {
            lastChargeTime += Time.deltaTime;
            if (lastChargeTime >= chargeCool)
            {
                DetectRock();
                if (isCharging)
                {
                    Debug.LogFormat("감지했나?");

                    lastChargeTime = 0f;
                }
            }
        }
        else if (isCharging)
        {
            ChargeTowardsRock();
       
            float distanceToLastRockPosition = Vector3.Distance(transform.position, lastRockPosition);

            if (distanceToLastRockPosition <= 10.0f)
            {
                isCharging = false;
                isReturning = true;
            }
        }
        else if (isReturning)
        {
            ReturnToInitialPositionBackwards();

            float distanceToInitialPosition = Vector3.Distance(transform.position, initialBullPosition);

            if (distanceToInitialPosition <= 1.0f)
            {
                ResetCharge();
                isReturning = false;
            }
        }
        // 머리가 돌을 바라보게 합니다.
        if (targetRock != null && !isCharging && !isReturning)
        {
            transform.LookAt(new Vector3(targetRock.transform.position.x, transform.position.y, targetRock.transform.position.z));
        }
    }
    public void TakeDamage(float damage) // 데미지를 받는 메서드를 선언합니다.
    {
        health -= damage; // 체력을 감소시킵니다.
        if (health <= 0) // 체력이 0 이하일 경우
        {
            Destroy(gameObject); // 게임 오브젝트를 제거합니다.
        }
    }

    private void DetectRock()
    {
        float coneAngle = 45f; // 원뿔의 각도를 설정합니다. 필요에 따라 조정하세요.
        float halfConeAngle = coneAngle * 0.5f;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, detectionRange, transform.forward, detectionRange, Rock);
        float minAngle = float.MaxValue;
        Transform closestRock = null;

        foreach (RaycastHit hit in hits)
        {
            Vector3 hitDirection = (hit.transform.position - transform.position).normalized;
            float angleToTarget = Vector3.Angle(transform.forward, hitDirection);

            if (angleToTarget <= halfConeAngle && angleToTarget < minAngle)
            {
                minAngle = angleToTarget;
                closestRock = hit.transform;
            }
        }

        if (closestRock != null)
        {
            targetRock = closestRock;
            lastRockPosition = targetRock.position;
            isCharging = true;
        }
    }
    private void ChargeTowardsRock() // 돌을 향해 돌진하는 메서드를 선언합니다.
    {
        Vector3 direction = (lastRockPosition - transform.position).normalized;
        direction.y = 0; // y축 값을 0으로 설정합니다.
        float distanceToTarget = Vector3.Distance(lastRockPosition, transform.position);
        bullRigidbody.velocity = direction * chargeSpeed;
        // 황소가 돌을 바라보게 합니다.
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void ResetCharge()
    {
        targetRock = null;
        lastChargeTime = 0f;
        chargeCount = 0;
        isCharging = false;
    }

   
    void OnCollisionEnter(Collision collision)
    {
        RockBase rock = collision.gameObject.GetComponent<RockBase>();

        if (rock != null && chargeCount < 1)
        {
       Debug.LogFormat("충돌했나?");

            hasCharged = true;
            chargeCount++;
            isCharging = false;
            isReturning = true;

            Rigidbody rockRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Rigidbody bullRigidbody = GetComponent<Rigidbody>();

            if (bullRigidbody != null)
            {
                bullRigidbody.velocity = Vector3.zero;
            }

            if (rockRigidbody != null && targetRock != null)
            {
                Vector3 forceDirection = (targetRock.position - transform.position).normalized;
                rockRigidbody.velocity = Vector3.zero;
                rockRigidbody.AddForce(forceDirection * attackPower, ForceMode.VelocityChange);
                rockRigidbody.AddForce(Vector3.up * 15f, ForceMode.VelocityChange);
            }
            IHitObjectHandler hitObj = collision.gameObject.GetComponent<IHitObjectHandler>();
            if (hitObj != null)
            {
                hitObj.Hit((int)attackPower);
            }
            ResetCharge();
        }
        // 모루황소가 돌과 충돌했을 때 충돌 횟수를 증가시키고, 충돌 횟수가 2 이상이면 죽는 코드 추가
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            rockCollisionCount++;
            if (rockCollisionCount >= 2)
            {
                isDying = true;
                StartCoroutine(Die()); // 황소가 죽을 때 Die 코루틴을 실행합니다.
            }
        }
    }

    private void ReturnToInitialPositionBackwards()
    {
        Vector3 direction = (initialBullPosition - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(-direction);
        transform.position += direction * walkSpeed * Time.deltaTime;

        float distanceToInitialPosition = Vector3.Distance(transform.position, initialBullPosition);
        if (distanceToInitialPosition <= 1.0f)
        {
            isReturning = false;
        }
    }
    private IEnumerator Die()
    {
        animator.SetBool("isDying", true); // 애니메이터의 "isDying" 파라미터를 true로 설정합니다.

        // 포지션 고정
        Rigidbody bullRigidbody = GetComponent<Rigidbody>();
        if(bullRigidbody != null)
        {
            bullRigidbody.velocity = Vector3.zero;
        }
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // 애니메이션 클립의 길이만큼 대기합니다.
                                                                                         // 황소의 포지션을 고정합니다.
        if (bullRigidbody != null)
        {
            bullRigidbody.isKinematic = true; // 황소의 Rigidbody를 isKinematic 상태로 변경하여 포지션을 고정합니다.
        }
        // 애니메이션이 끝난 상태로 3초 대기
        animator.speed = 0f;
        yield return new WaitForSeconds(3f);
        Destroy(gameObject); // 애니메이션이 끝난 후에 게임 오브젝트를 파괴합니다.
    }
    public void Hit(int damage)
    {
        throw new NotImplementedException();
    }

    public void HitReaction()
    {
        throw new NotImplementedException();
    }
}


