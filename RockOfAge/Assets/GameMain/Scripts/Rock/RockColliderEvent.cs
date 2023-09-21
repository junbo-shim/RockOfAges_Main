using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockColliderEvent : MonoBehaviour
{
    RockBase parent;

    private void Awake()
    {
        parent = GetComponentInParent<RockBase>();
    }

    private void OnCollisionEnter(Collision collision)
    {


        if (parent.IsMove(1))
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
            {
                parent.Attack(collision);
            }
            if (collision.gameObject.layer != LayerMask.NameToLayer("Terrains"))
            {
                Debug.Log(collision.gameObject.layer);
                Debug.Log(collision.gameObject.name);
                parent.Hit(100);
            }
        }
    }
}
