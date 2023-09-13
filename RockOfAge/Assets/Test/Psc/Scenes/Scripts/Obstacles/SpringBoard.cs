using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringBoard : HoldObstacleBase, IHitObjectHandler
{

    [SerializeField]
    private GameObject colliderObject;
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
                Vector3 eulerAngle = colliderObject.transform.localEulerAngles - Vector3.right*30;
                collision.gameObject.GetComponent<Rigidbody>().AddForce(eulerAngle.normalized * 1000f, ForceMode.Acceleration);
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

    public void Push(Collision collision)
    {
        Vector3 eulerAngle = colliderObject.transform.localEulerAngles - Vector3.right * 20 + Vector3.up * 40;
        collision.gameObject.GetComponent<Rigidbody>().AddForce(eulerAngle.normalized * 5000f, ForceMode.Acceleration);
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
