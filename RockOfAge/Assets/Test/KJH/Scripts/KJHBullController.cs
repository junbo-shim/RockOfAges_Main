using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KJHBullController : MonoBehaviour
{
    public float detectionRange = 5.0f;
    public float chargeSpeed = 10.0f;
    public LayerMask Rock;

    private bool isCharging = false;
    private Transform targetRock;

    private void Update()
    {
        if (!isCharging)
        {
            DetectRock();
        }
        else
        {
            ChargeTowardsRock();
        }
    }

    private void DetectRock()
    {
        Collider[] rocks = Physics.OverlapSphere(transform.position, detectionRange, Rock);
        if (rocks.Length > 0)
        {
            Debug.LogFormat("감지했나?");
            targetRock = rocks[0].transform;
            isCharging = true;
        }
    }

    private void ChargeTowardsRock()
    {
        if (targetRock != null)
        {
            Vector3 direction = (targetRock.position - transform.position).normalized;
            transform.position += direction * chargeSpeed * Time.deltaTime;
        }
        else
        {
            isCharging = false;
        }
    }
}