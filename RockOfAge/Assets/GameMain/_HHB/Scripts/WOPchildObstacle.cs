using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WOPchildObstacle : MonoBehaviour, IHitObjectHandler
{
   // private Rigidbody target;
    private Transform child;
    private ObstacleBase parent;
    private float hp;

    protected void Awake()
    {
        hp = 50f;
        child = GetComponent<Transform>();
        parent = transform.parent.GetComponentInParent<ObstacleBase>();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!parent.isBuildComplete)
        {
            return;

        }
        Rigidbody target = collision.gameObject.GetComponent<Rigidbody>();
        float obsForce = 200f;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            Vector3 targetNonVector = -new Vector3(target.transform.position.x, 0f, target.transform.position.z).normalized;
            Vector3 childNonVector = new Vector3(child.transform.position.y, 0f, child.transform.position.y).normalized;
            Vector3 yOffset = Vector3.up;
            Vector3 force = ((childNonVector + targetNonVector).normalized + yOffset * 0.02f) * obsForce;
            target.AddForce(force, ForceMode.Impulse);
            
        }
    }

    public void Hit(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
        HitReaction();
    }
    public void HitReaction()
    { 
    
    }
}
