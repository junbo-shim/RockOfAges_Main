using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WOPObstacle : HoldObstacleBase
{
    // 공
    private new Transform transform;
    Renderer[] renderers;
    Queue<Material[]> originMaterials;

    //초기화
    protected override void Init()
    {
        base.Init();
        originMaterials = new Queue<Material[]>();
        obstacleRigidBody = GetComponent<Rigidbody>();
        obstacleRigidBody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        obstacleRigidBody.isKinematic = true;
        transform = GetComponent<Transform>();
        currHealth = status.Health;
        renderers = GetComponentsInChildren<Renderer>();
    }


    //제일 하단 스크립트에서 해당 함수를 불러온다(ONENABLE)
    protected override void StartBuild(float time)
    {
        foreach(var renderer in renderers)
        {
            originMaterial = renderer.materials;
            //마테리얼 교체
            originMaterials.Enqueue(renderer.materials);
            Material[] subMaterial = new Material[originMaterial.Length];
            for (int i = 0; i < subMaterial.Length; i++)
            {
                subMaterial[i] = BuildManager.instance.white;
            }
            renderer.materials = subMaterial;

           

        }
         if (obstacleCollider != null)
            {
                if (obstacleCollider is MeshCollider)
                {
                    (obstacleCollider as MeshCollider).convex = true;
                }
                obstacleCollider.isTrigger = true;
            }

        StartCoroutine(BuildRoutine(time));
    }

    //일정 시간동안 대기하는 COROUTINE
    protected override IEnumerator BuildRoutine(float buildTime)
    {
        float currTime = 0;
        while (currTime < buildTime)
        {
            yield return Time.deltaTime;
            currTime += Time.deltaTime;
        }

        if (gameObject == null)
            yield break;

        isBuildComplete = true;


        foreach (var renderer in renderers)
        {
            //마테리얼 교체
            renderer.materials = originMaterials.Dequeue();
        }


        if (obstacleCollider != null)
        {
            obstacleCollider.isTrigger = false;
            if (obstacleCollider is MeshCollider)
            {
                (obstacleCollider as MeshCollider).convex = false;
            }
        }
        MakePeople();
    }

    private void Update()
    {
        TurnAroundObstacle();
    }

    //죽음
    protected override void Dead() 
    {
        PhotonNetwork.Destroy(this.gameObject);
    }

    private void TurnAroundObstacle()
    {
        transform.rotation *= Quaternion.Euler(0f, 2f, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            Dead();
        }
    }
}
