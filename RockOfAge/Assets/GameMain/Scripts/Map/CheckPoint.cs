using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            RockBase rock = other.GetComponentInParent<RockBase>();
            rock.SetCheckPoint(transform.position+Vector3.up*5);
        }
    }
}
