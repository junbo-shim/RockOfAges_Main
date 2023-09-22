using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : MonoBehaviour
{

    [SerializeField]
    int damage;
    [SerializeField]
    float delayTime = .1f;
    [SerializeField]
    [Range(0,1)]
    float slowPower;

    float time = 0;

    private void OnTriggerEnter(Collider other)
    {
        time = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        
        time += Time.deltaTime;
        if (time>= delayTime && other.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            time = 0;
            other.GetComponentInParent<RockBase>().Hit(damage);
            Rigidbody rigidbody = other.GetComponent<Rigidbody>();
            rigidbody.velocity *= slowPower;
            rigidbody.angularVelocity *= slowPower;
        }
    }

}