using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KJHJumpad : MonoBehaviour
{
    public string playerLayerName = "Rock";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            Rigidbody rRb = collision.gameObject.GetComponent<Rigidbody>();
            rRb.AddForce(Vector3.up * 5000f, ForceMode.Acceleration); // 점프대 속도를 적용합니다.
        }
    }
}
