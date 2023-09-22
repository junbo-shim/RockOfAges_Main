using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowObstacle : HoldObstacleBase
{
    [SerializeField]
    private Collider _collider;
    private Transform cow;
    public bool isSticked;
    public int groundCount;
    private bool delayBool;


    private void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        cow = GetComponent<Transform>();
        status = new ObstacleStatus(status);
        obstacleMeshFilter = GetComponent<MeshFilter>();
        obstacleRenderer = GetComponent<Renderer>();
        obstacleRigidBody = GetComponent<Rigidbody>();
        currHealth = status.Health;
        isSticked = false;
        delayBool = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Transform rock = collision.transform;
        if (!isSticked && collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            Physics.IgnoreCollision(collision.collider, _collider);
            transform.localPosition -= (transform.position - rock.position) * .3f;
            cow.SetParent(rock);
            isSticked = true;
            Destroy(obstacleRigidBody);
            delayBool = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (delayBool && isSticked && other.gameObject.layer == LayerMask.NameToLayer("Terrains"))
        {
            StartCoroutine(DelayBool());
            if (groundCount == 2)
            {
                StartCoroutine(DestroyCow());
            }
        }
    }

    IEnumerator DestroyCow()
    {
        cow.SetParent(null);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    IEnumerator DelayBool()
    {
        groundCount++;
        delayBool = false;
        yield return new WaitForSeconds(3f);
        delayBool = true;
    }
}
