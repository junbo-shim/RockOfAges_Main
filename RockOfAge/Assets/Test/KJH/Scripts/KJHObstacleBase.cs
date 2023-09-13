using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KJHObstacleBase : MonoBehaviour
{
    public ObstacleStatus obstacleStatus;
    protected Rigidbody oRb;
    protected Animator animator;
    virtual public void Init()
    {
        oRb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    virtual public void Build()
    {

    }
}
