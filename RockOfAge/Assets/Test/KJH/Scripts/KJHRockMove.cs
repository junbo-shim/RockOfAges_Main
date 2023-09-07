using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KJHRockMove : MonoBehaviour
{
    #region [[SerializeField]
    [SerializeField]
    private float forceAmount = 1f;
    [SerializeField]
    private float jumpForce = 3000f;
    [SerializeField]
    private float health = 100f;
    [SerializeField]
    private float attackPowerBase = 1f;
    #endregion

    private float attackPower;
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

        // 공격력을 현재 속도에 비례하게 계산합니다.
        float currentSpeed = rb.velocity.magnitude;
        attackPower = attackPowerBase * (health + currentSpeed);
    }
    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    public void Attack(GameObject target)
    {
        // 타겟에게 공격력만큼의 데미지를 입힙니다.
        // 타겟의 데미지 처리 메서드를 호출하고 attackPower 값을 전달합니다.
        target.GetComponent<Target>().TakeDamage(attackPower);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            Attack(collision.gameObject);
        }
    }
}