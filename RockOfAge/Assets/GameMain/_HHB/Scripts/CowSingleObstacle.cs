using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CowSingleObstacle : MoveObstacleBase
{
    [SerializeField]
    private Collider[] _colliders;
    private Transform cow;
    public bool isSticked;
    public int groundCount;
    private bool delayBool;

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

    [PunRPC]
    public void SetActive()
    {
        isSticked = true;
        delayBool = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isSticked && collision.gameObject.layer == LayerMask.NameToLayer("Rock") && collision.collider.name=="RockObject")
        {
            Transform rock = collision.transform;
            if (audioSource != null)
            {
                audioSource.Play();
            }
            Physics.IgnoreCollision(collision.collider, obstacleCollider);
            transform.localPosition -= (transform.position - rock.position) * .4f;
            cow.SetParent(rock);
            Destroy(obstacleRigidBody);
            photonView.RPC("SetActive", RpcTarget.All);
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
        gameObject.AddComponent<Rigidbody>();
        yield return new WaitForSeconds(1f);
        PhotonNetwork.Destroy(gameObject);
    }

    IEnumerator DelayBool()
    {
        groundCount++;
        delayBool = false;
        yield return new WaitForSeconds(1f);
        delayBool = true;
    }
}
