using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KJHObject : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
        Vector3 vector3 = Camera.main.transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler (new Vector3(0, vector3.y, 0));
    }
}
