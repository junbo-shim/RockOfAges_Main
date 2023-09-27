using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodHand : MonoBehaviour
{
    private Animator animator;


    private void Start()
    {
        animator = GetComponent<Animator>();
        this.gameObject.transform.localScale = Vector3.one * 0.001f;
    }

    public void StandBy(GameObject rock)
    {
        animator.SetBool("Grab", false);
        this.gameObject.transform.localScale = Vector3.one * 0.2f;
        Vector3 pos = Camera.main.transform.position - rock.transform.GetChild(1).transform.position;
        transform.position = rock.transform.GetChild(1).transform.position; /*+ Vector3.up * 2f - Vector3.forward * 2f;*/
    }

    public void FollowRock(GameObject rock)
    {
        if (rock != null)
        {
            StartCoroutine(WaitForHand(rock));
        }
    }

    IEnumerator WaitForHand(GameObject rock)
    {
        this.gameObject.transform.localScale = Vector3.one * 0.2f;
        float timer = 0f;
        animator.SetBool("Grab", true);
        GameObject godHand = this.gameObject;
        while (timer <= 3f)
        {
            if (rock == null) { yield break; }
            timer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
            Vector3 pos = Camera.main.transform.position - rock.transform.GetChild(1).transform.position;
            godHand.transform.position = rock.transform.GetChild(1).transform.position - Vector3.forward * .5f;
            pos.y = 0;
            godHand.transform.rotation = Quaternion.LookRotation(pos.normalized, Vector3.up);
        }
        animator.SetBool("Grab", false);
        this.gameObject.transform.localScale = Vector3.zero;
    }
}
