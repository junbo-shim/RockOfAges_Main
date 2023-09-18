using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KJHBooster : MonoBehaviour
{
    public float duration = default;
    public float boostForce = default;
    public float upForce = default;

    private void OnTriggerEnter(Collider other)
    {
        RockBase rockBase = other.GetComponent<RockBase>();
        if (rockBase != null)
        {
            //rockBase.ApplyBoosterEffect(duration, boostForce, upForce, transform.forward.normalized);
        }
    }
}