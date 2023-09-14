using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringBoard : HoldObstacleBase, IHitObjectHandler
{
    private GameObject colliderParts = default;
    private bool isAttacked = false;

    private void Start()
    {
        colliderParts = transform.GetChild(0).GetChild(1).GetChild(1).gameObject;
        ActiveIdle();
    }


    private void OnCollisionEnter(Collision collision)
    {
        CheckState(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        CheckState(collision);
    }
    private void CheckState(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            if (!isAttacked && obstacleAnimator.GetBool("Idle"))
            {
                ActiveAttack();
            }

            if (isAttacked)
            {
                Push(collision);
            }
        }
    }

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

    public void Push(Collision collision)
    {
        Vector3 eulerAngle = colliderParts.transform.localEulerAngles - Vector3.right * 20 + Vector3.up * 40;
        collision.gameObject.GetComponent<Rigidbody>().AddForce(eulerAngle.normalized * 500f, ForceMode.Acceleration);
    }

    public void Hit(int damage)
    {
        if (!isAttacked)
        {
            return;
        }
    }

    public void HitReaction()
    {
        throw new System.NotImplementedException();
    }
}
