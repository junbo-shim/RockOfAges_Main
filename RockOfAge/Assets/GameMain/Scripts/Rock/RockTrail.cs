using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockTrail : MonoBehaviour
{
    [SerializeField]
    Transform rockMesh;
    [SerializeField]
    RockBase rockMainScript;
    [SerializeField]
    TrailRenderer trailRenderer;

    bool isStart;

    // Start is called before the first frame update
    void Awake()
    {
        rockMesh = transform.parent.Find("RockObject");
        rockMainScript = transform.GetComponentInParent<RockBase>();
        trailRenderer = GetComponent<TrailRenderer>();

        isStart = false;
        transform.position = rockMesh.position - Vector3.up * rockMesh.gameObject.GetHeight(.49f);
    }

    // Update is called once per frame
    void Update()
    {
        if (rockMesh == null)
        {
            Destroy(gameObject);
            return;
        }

        if(!isStart && rockMainScript.isGround)
        {
            isStart = true;
        }

        if (isStart)
        {
            transform.position = rockMesh.position - Vector3.up * rockMesh.gameObject.GetHeight(.49f);
            trailRenderer.emitting = true;
        }

        if (isStart && !rockMainScript.isGround)
        {
            trailRenderer.emitting = false; 
        }
    }
}
