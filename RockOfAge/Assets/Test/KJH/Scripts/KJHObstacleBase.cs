using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KJHObstacleBase : MonoBehaviour
{
    public ObstacleStatus obstacleStatus;
    protected Rigidbody Orb;
    protected Animator animator;
    virtual public void Init()
    {
        Orb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    virtual public void Build()
    {

    }
}
