using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WOPObstacle : HoldObstacleBase
{
    // 공
    private Transform rockTransform;

    private void Awake()
    {
        Init();
    }

    //초기화
    protected override void Init()
    {
        status = new ObstacleStatus(status);// Instantiate(status);
        obstacleMeshFilter = GetComponent<MeshFilter>();
        obstacleRigidBody = GetComponent<Rigidbody>();
        obstacleRenderer = GetComponent<Renderer>();
        currHealth = status.Health;
    }

    private void Start()
    {
        MeshFilter meshFilter = obstacleMeshFilter.GetComponentInChildren<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        // 삼각형을 제거할 인덱스 리스트를 만듭니다.
        List<int> trianglesList = new List<int>(mesh.triangles);
        trianglesList.RemoveAt(9); // indexToRemove는 제거할 삼각형의 인덱스입니다.

        // triangles 배열을 업데이트합니다.
        mesh.triangles = trianglesList.ToArray();

        // 메쉬를 재계산합니다.
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }


    private void Update()
    {
        TurnAroundObstacle();
    }

    //죽음
    protected override void Dead() 
    { 
    
    }

    private void TurnAroundObstacle()
    { 
    
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        { 
        
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
