using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KJHRockMove : MonoBehaviour
{
    #region [[SerializeField]
    [SerializeField]
    private float forceAmount = 2f;
    [SerializeField]
    private float jumpForce = 3f;
    #endregion

    private Rigidbody rb;
    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    void Update()
    {
       
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f)
        {
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 cameraRight = mainCamera.transform.right;

            Vector3 forceDirection = (cameraForward * verticalInput + cameraRight * horizontalInput);
            forceDirection.y = 0;

            rb.AddForce(forceDirection * forceAmount, ForceMode.Acceleration);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

    }
    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}