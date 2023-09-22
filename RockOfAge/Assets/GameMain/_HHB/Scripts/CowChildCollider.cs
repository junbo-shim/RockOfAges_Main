using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowChildCollider : MonoBehaviour
{
    private Transform motherCow;
    private CapsuleCollider groundCollider;
    private bool delayBool;
    public int groundCount;
    public bool motherSticked;

    private void Awake()
    {
        groundCollider = GetComponent<CapsuleCollider>();
        motherCow = GetComponentInParent<Transform>();
        groundCount = 0;
        delayBool = false;
    }

    private void Update()
    {
        motherSticked = motherCow.GetComponentInParent<CowObstacle>().isSticked;
    }


    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("!");
    //    Debug.Log("motherStick :" + motherSticked);
    //    Debug.Log("DelayBool :" + delayBool);
    //    Debug.Log("layer :"+ collision.gameObject.layer);
    //    if (motherSticked == true && !delayBool && collision.gameObject.layer == LayerMask.NameToLayer("Terrains"))
    //    {
    //        Debug.Log("자식 충돌 검출됨?");
    //        delayBool = true;
    //        StartCoroutine(WaitCowCount());
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (motherSticked == true && !delayBool && other.gameObject.layer == LayerMask.NameToLayer("Terrains"))
        {
            Debug.Log("자식 충돌 검출됨?");
            delayBool = true;
            StartCoroutine(WaitCowCount());
        }
    }

    IEnumerator WaitCowCount()
    {
        yield return new WaitForSeconds(1f);
        groundCount++;
        groundCount = motherCow.GetComponentInParent<CowObstacle>().groundCount;
        Debug.Log(groundCount);

        if (delayBool == true)
        {
            delayBool = false;
        }
    }
}
