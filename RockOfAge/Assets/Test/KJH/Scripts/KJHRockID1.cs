using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KJHRockID1 : RockBase, IHitObjectHandler
{
    public float gravityStrength = 100f; // 중력 세기 (기본값: 지구 중력)

    void Awake()
    {
        rockStatus = new RockStatus(rockStatus);
    }
    void Start()
    {
        Init(); // RockBase 클래스의 Init 메서드를 호출합니다.
    }
    void FixedUpdate()
    {
        Move(); // RockBase 클래스의 Move 메서드를 호출합니다.
        if (!IsGround())
        {
            // 중력 힘을 직접 적용
            Vector3 gravityForce = Vector3.down * gravityStrength;
            rRb.AddForce(gravityForce, ForceMode.Acceleration);
        }
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump(); // RockBase 클래스의 Jump 메서드를 호출합니다.
        }
    }

    public override float Attack()
    {
        float attackPower;
        // 공격력을 현재 속도에 비례하게 계산합니다.
        float currentSpeed = rRb.velocity.magnitude;
        attackPower = attackPowerBase * (rockStatus.Health + currentSpeed);
        return attackPower;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            IHitObjectHandler hitObj = collision.gameObject.GetComponent<IHitObjectHandler>();
            if (hitObj != null)
            {
                Attack();
                hitObj.Hit((int)Attack());
            }
        }
    }

    public void Hit(int damage)
    {
        rockStatus.Health -= damage;
    }

    public void HitReaction()
    {
        throw new System.NotImplementedException();
    }
}