using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBase : MonoBehaviour
{
    protected float jumpForce = 3000f;
    protected float attackPowerBase = 10f;
    
    public RockStatus rockStatus;
    protected Rigidbody rb;
    protected Camera mainCamera;

    virtual public void Init()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }
    virtual public void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f)
        {
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 cameraRight = mainCamera.transform.right;

            Vector3 forceDirection = (cameraForward * verticalInput + cameraRight * horizontalInput);
            forceDirection.y = 0;

            rb.AddForce(forceDirection * rockStatus.Acceleration, ForceMode.Acceleration);
        }
    }

    virtual public void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    virtual public void Hit()
    {

    }

    virtual public void Attack(GameObject target)
    {

    }

    virtual public void Fall()
    {

    }

    virtual public bool IsGround()
    {

        return true;
    }
}
