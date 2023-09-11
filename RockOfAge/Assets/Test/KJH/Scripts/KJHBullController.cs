using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System;

public class KJHBullController : MonoBehaviour
{
    public float detectionRange = 5.0f;
    public float chargeSpeed = 10.0f;
    public LayerMask Rock;

    public float health = 100f; // 체력 변수
    public float attackPower = 10f; // 공격력 변수
    public float chargeCool = 5.0f;

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

    private void Update() // Update 메서드를 선언합니다.
    {
        if (!isCharging) // 돌진 중이 아닐 경우
        {
            lastChargeTime += Time.deltaTime;
            if (lastChargeTime >= chargeCool) // 쿨타임이 지났을 경우
            {
                // 자기자리으로 돌아감.
                DetectRock(); // 돌을 탐지합니다.
            }
        }
        else
        {
            ChargeTowardsRock(); // 돌을 향해 돌진합니다.
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

        if (distanceToTarget > 1f) // 돌에 도달하기 전까지 돌진
        {
            bullRigidbody.velocity = direction * chargeSpeed;
        }
        else // 돌에 도달했을 때 돌진 상태 초기화 및 충돌 처리
        {
            chargeCount++;
            Rigidbody rockRigidbody = targetRock.GetComponent<Rigidbody>();
            if (rockRigidbody == null)
            {
                targetRock.gameObject.AddComponent<Rigidbody>();
                rockRigidbody = targetRock.GetComponent<Rigidbody>();
            }

            Vector3 forceDirection = (targetRock.position - transform.position).normalized;
            rockRigidbody.AddForce(forceDirection * attackPower, ForceMode.Impulse);
            lastChargeTime = Time.time; // 쿨타임 설정
            bullRigidbody.velocity = Vector3.zero; // 황소의 속도를 0으로 설정
        }
    }

    void ResetCharge() // 돌진 상태를 초기화하는 메서드를 선언합니다.
    {
        isCharging = false;
    }
    void OnCollisionEnter(Collision collision) // 충돌이 발생했을 때의 메서드를 선언합니다.
    {
        RockBase rock = collision.gameObject.GetComponent<RockBase>();

        if (rock != null)
        {
            Rigidbody rockRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Rigidbody bullRigidbody = GetComponent<Rigidbody>();

            if (bullRigidbody != null)
            {
                bullRigidbody.velocity = Vector3.zero;
            }

            TakeDamage(1f);
            if (rockRigidbody != null)
            {
                rockRigidbody.velocity = bullRigidbody.velocity;
            }
        }
        lastChargeTime = 0f;
        ResetCharge();
    }

}