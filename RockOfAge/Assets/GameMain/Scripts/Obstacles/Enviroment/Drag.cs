using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    [SerializeField]
    Transform dist;

    [SerializeField]
    float dragPower = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            Vector3 point = other.ClosestPoint(other.transform.position);
            other.GetComponent<Rigidbody>().velocity += (dist.position - point) * dragPower;
        }
    }
}
