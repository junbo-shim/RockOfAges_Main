using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringBoard : HoldObstacleBase, IHitObjectHandler
{

    [SerializeField]
    private GameObject colliderParts;
    private bool isAttacked = false;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            if (!isAttacked)
            {
                ActiveAttack();
            }
            else
            {
                Push(collision);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            if (!isAttacked)
            {
                obstacleAnimator.SetBool("Idle", false);
                ActiveAttack();
            }
            else
            {
                Push(collision);
            }
        }

    }

    protected override void ActiveAttack()
    {
        if (obstacleAnimator.GetBool("Idle"))
        {
            isAttacked = true;
            obstacleAnimator.SetTrigger("Active");
        }
    }
    protected override void EndAttack()
    {
        obstacleAnimator.SetBool("Idle", true);
    }

    public void Push(Collision collision)
    {
        Vector3 eulerAngle = colliderParts.transform.localEulerAngles - Vector3.right * 20 + Vector3.up * 40;
        collision.gameObject.GetComponent<Rigidbody>().AddForce(eulerAngle.normalized * 1000f, ForceMode.Acceleration);
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
