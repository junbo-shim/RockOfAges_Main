using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using System;

public class KJHBullController : MonoBehaviour
{
    public float detectionRange = 5.0f;
    public float chargeSpeed = 10.0f;
    public LayerMask Rock;

    public float health = 100f; // 체력 변수
    public float attackPower = default; // 공격력 변수
    public float chargeCool = 5.0f;
    public Transform headTransform; // 모루황소 머리의 Transform을 참조합니다.

    private float lastChargeTime = 0f;
    private Rigidbody bullRigidbody;
    private bool isCharging = false;
    private Transform targetRock;
    private Animator animator;
    private int chargeCount = 0;

    private void Start() // Start 메서드를 선언합니다.
    {
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트를 가져옵니다.
        bullRigidbody = GetComponent<Rigidbody>(); // 리지드바디 컴포넌트를 가져옵니다.

    }

    private void Update()
    {
        if (!isCharging)
        {
            lastChargeTime += Time.deltaTime;
            if (lastChargeTime >= chargeCool)
            {
                DetectRock();
            }
        }
        else
        {
            ChargeTowardsRock();
        }

        // 머리가 돌을 바라보게 합니다.
        if (targetRock != null)
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

    private void DetectRock() // 돌을 탐지하는 메서드를 선언합니다.
    {
        Collider[] rocks = Physics.OverlapSphere(transform.position, detectionRange, Rock);
        if (rocks.Length > 0 && rocks[0] != null && chargeCount < 1)
        {
            targetRock = rocks[0].transform;
            isCharging = true;
        }
    }

    private void ChargeTowardsRock() // 돌을 향해 돌진하는 메서드를 선언합니다.
    {
        Vector3 direction = (targetRock.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(targetRock.position, transform.position);
        bullRigidbody.velocity = direction * chargeSpeed;
    }



    void ResetCharge() // 돌진 상태를 초기화하는 메서드를 선언합니다.
    {
        lastChargeTime = 0f;
        chargeCount = 0;
        isCharging = false;
    }
    void OnCollisionEnter(Collision collision) // 충돌이 발생했을 때의 메서드를 선언합니다.
    {
        RockBase rock = collision.gameObject.GetComponent<RockBase>();

        if (rock != null && chargeCount < 1)
        {
            chargeCount++;
            Rigidbody rockRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Rigidbody bullRigidbody = GetComponent<Rigidbody>();

            if (bullRigidbody != null)
            {
                bullRigidbody.velocity = Vector3.zero;
            }

            TakeDamage(1f);
            if (rockRigidbody != null)
            {
                Vector3 forceDirection = (targetRock.position - transform.position).normalized;
                rockRigidbody.velocity = Vector3.zero;
                rockRigidbody.AddForce(forceDirection * attackPower, ForceMode.VelocityChange);
                rockRigidbody.AddForce(Vector3.up * 10f, ForceMode.VelocityChange);

            }
            ResetCharge();
        }

    }

}