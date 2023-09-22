using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{

    [SerializeField]
    float dashPower = 1;
    [SerializeField]
    Vector3 forward = Vector3.forward;
    [SerializeField]
    bool mustZero = false;

    private void Awake()
    {
        transform.forward = forward;
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            Rigidbody rigidbody = other.GetComponent<Rigidbody>();
            if (mustZero)
            {
                rigidbody.velocity = Vector3.zero;
            }
            rigidbody.AddForce(transform.forward * dashPower, ForceMode.VelocityChange);
        }
    }
}
