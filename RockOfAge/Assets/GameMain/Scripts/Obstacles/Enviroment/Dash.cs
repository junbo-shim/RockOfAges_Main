using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{

    [SerializeField]
    float dashPower = default;
    [SerializeField]
    bool useParent = false;
    [SerializeField]
    Vector3 forward = default;
    [SerializeField]
    bool beforeVelocityZero = false;
    [SerializeField]
    ForceMode forceMode = ForceMode.Impulse;

    public AudioSource audioSource;

    [SerializeField]
    bool pushShaking = false;
    [SerializeField]
    float shakePower = 2f;

    
    const float SHAKE_TIME = 0.1F;

    private void Awake()
    {
        if(useParent)
        {
            transform.forward = transform.parent.forward;
        }
        else
        {
            transform.forward = forward;
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

    }


    private void OnTriggerEnter(Collider other)
    {
        audioSource.Play();

        if (other.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            Rigidbody rigidbody = other.GetComponent<Rigidbody>();
            if (beforeVelocityZero)
            {
                rigidbody.velocity = Vector3.zero;
            }
            if (pushShaking)
            {
                RockBase rockOrigin = other.GetComponentInParent<RockBase>();
                StartCoroutine(rockOrigin.CameraShakeRoutine(SHAKE_TIME, shakePower, 3));
            }
            rigidbody.AddForce(transform.forward * dashPower, forceMode);
        }
    }
}
