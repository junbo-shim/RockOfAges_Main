using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KJHRockMove : RockBase
{


    private float attackPower;

    void Start()
    {
        Init();
    }

    void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // 공격력을 현재 속도에 비례하게 계산합니다.
        float currentSpeed = rb.velocity.magnitude;
        attackPower = attackPowerBase * (rockStatus.Health + currentSpeed);
    }
   
    override public void Attack(GameObject target)
    {
        target.GetComponent<Target>().TakeDamage(attackPower);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            Attack(collision.gameObject);
        }
    }
}