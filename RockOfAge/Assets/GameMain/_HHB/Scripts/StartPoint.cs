using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    [HideInInspector]
    public Transform startTransform;

    public void Awake()
    {
        startTransform = transform;
    }
}
