using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KJHRockMove : RockBase, IHitObjectHandler
{



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

    }
   
    override public float Attack()
    {

     float attackPower;
        // 공격력을 현재 속도에 비례하게 계산합니다.
        float currentSpeed = rb.velocity.magnitude;
        attackPower = attackPowerBase * (rockStatus.Health + currentSpeed);
        return attackPower;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            IHitObjectHandler hitObj = collision.gameObject.GetComponent<IHitObjectHandler>();
            if(hitObj != null )
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