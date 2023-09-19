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

        Debug.Log("공격 인식");

        Debug.Log(parent.NowSpeed());

        if (parent.IsMove(1))
        {
            Debug.Log("공격 시작");
            Debug.Log(collision.gameObject.layer+"/"+ LayerMask.NameToLayer("Obstacles"));
            if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
            {
                Debug.Log("장해물 공격");
                parent.Attack(collision);
            }
            if (parent.IsMove(2.5f))
            {
                Debug.Log("아무데나 공격");
                parent.Hit(10);
            }
        }
    }
}
