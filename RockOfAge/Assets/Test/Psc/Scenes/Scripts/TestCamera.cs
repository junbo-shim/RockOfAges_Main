using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamera : MonoBehaviour
{
    public GameObject target;
    public Vector3 v;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {

            transform.position = target.transform.position + v;
            transform.LookAt(target.transform.position);
        }
    }
}
