using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KJHHeavenBull : MonoBehaviour
{
    /* 천국의 황소
    감지범위설정
    감지범위에 들어오면 돌을 바라본다
    돌을 공격*/

    public float detectionRadius = 5f;
    public LayerMask Rock;

    private void Update()
    {
        DetectRock();
    }
    public void DetectRock()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, detectionRadius, Rock);
        foreach (Collider coll in colls)
        {
            if (coll.gameObject.layer == LayerMask.NameToLayer("Rock"))
            {
                Debug.Log("감지했나");

            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
