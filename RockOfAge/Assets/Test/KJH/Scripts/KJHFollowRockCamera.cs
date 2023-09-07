using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KJHFollowRockCamera : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 1.0f;
    public Vector3 cameraOffset;

    private float mouseX;
    private float mouseY;

    void Start()
    {
        cameraOffset = transform.position - target.position;
    }

    void Update()
    {
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -45, 45);
            
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
        transform.position = target.position + rotation * cameraOffset;
        transform.LookAt(target);
    }
}