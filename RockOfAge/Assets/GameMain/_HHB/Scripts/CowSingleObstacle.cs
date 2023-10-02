using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CowSingleObstacle : HoldObstacleBase
{
    [SerializeField]
    private Collider[] _colliders;
    private Transform cow;
    public bool isSticked;
    public int groundCount;
    private bool delayBool;

    public AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    protected override void Init()
    {
        base.Init();
        cow = GetComponent<Transform>();
        status = new ObstacleStatus(status);
        obstacleMeshFilter = GetComponent<MeshFilter>();
        obstacleRenderer = GetComponent<Renderer>();
        obstacleRigidBody = GetComponent<Rigidbody>();
        _colliders = GetComponents<Collider>();
        foreach(var collider in _colliders)
        {
            if (collider.isTrigger)
            {

            }
            else
            {
                obstacleCollider = collider;
            }
        }
        currHealth = status.Health;
        isSticked = false;
        delayBool = false;
        transform.parent = obstacleParent;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }

        Transform rock = collision.transform;
        if (!isSticked && collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            Physics.IgnoreCollision(collision.collider, obstacleCollider);
            transform.localPosition -= (transform.position - rock.position) * .3f;
            cow.SetParent(rock);
            Destroy(obstacleRigidBody);
            isSticked = true;
            delayBool = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isBuildComplete)
        {
            if (delayBool && isSticked && other.gameObject.layer == LayerMask.NameToLayer("Terrains"))
            {
                if (groundCount <= 1)
                {
                    StartCoroutine(DelayBool());
                }
                else
                {
                    StartCoroutine(DestroyCow());
                }
            }
        }
    }

    IEnumerator DestroyCow()
    {
        cow.parent = null;
        yield return new WaitForSeconds(1f);
        PhotonNetwork.Destroy(gameObject);
    }

    IEnumerator DelayBool()
    {
        groundCount++;
        delayBool = false;
        yield return new WaitForSeconds(3f);
        delayBool = true;
    }
}
