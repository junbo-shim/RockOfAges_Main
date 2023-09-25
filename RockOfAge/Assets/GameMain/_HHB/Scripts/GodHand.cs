using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodHand : MonoBehaviour
{
    private Transform rockPosition;
    private Animator animator;


    private void Start()
    {
        rockPosition = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        this.gameObject.transform.localScale = Vector3.one * 0.001f;
    }


    public void FollowRock(GameObject rock)
    {
        this.gameObject.transform.localScale = Vector3.one;
        if (rock != null)
        {
            StartCoroutine(WaitForHand(rock));
        }
    }

    IEnumerator WaitForHand(GameObject rock)
    {
        float timer = 0f;
        Transform rockPosition = transform.GetChild(1).GetChild(0).GetChild(0).Find("RockPosition");
        animator.SetBool("Grab", true);
        GameObject godHand = this.gameObject;
        while (timer <= 3f)
        {
            timer += Time.deltaTime;
            yield return null;
            //godHand.transform.position = rockPosition.transform.position;
            //godHand.transform.position = new Vector3(-2.4f, 11.8f, -109.3f);
            godHand.transform.position = rock.transform.GetChild(0).transform.position;
        }
        animator.SetBool("Grab", false);
        this.gameObject.transform.localScale = Vector3.one * 0.001f;
    }
}
