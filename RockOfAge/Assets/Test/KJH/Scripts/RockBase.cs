using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBase : MonoBehaviour
{
    protected float jumpForce = 3000f;
    protected float attackPowerBase = 10f;

    public Vector3 boostDirection;
    public RockStatus rockStatus;
    protected Rigidbody Rrb;
    protected Camera mainCamera;

    public  virtual void Init()
    {
        Rrb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }
    public virtual void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f)
        {
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 cameraRight = mainCamera.transform.right;

            Vector3 forceDirection = (cameraForward * verticalInput + cameraRight * horizontalInput);
            forceDirection.y = 0;

            // 이동 방향에 따라 가속도를 조절합니다.
            float acceleration = (Mathf.Abs(horizontalInput) > 0.1f && Mathf.Abs(verticalInput) > 0.1f) ? rockStatus.Acceleration * 2 : rockStatus.Acceleration;
            Rrb.AddForce(forceDirection * acceleration, ForceMode.Acceleration);
        }
    }

    public virtual void Jump()
    {
        Rrb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

   

    public virtual float Attack()
    {
        return 0;
    }

    public virtual void Fall()
    {

    }

    public virtual bool IsGround()
    {

        return true;
    }
    protected IEnumerator ApplyBooster(float duration, float boosterMultiplier)
    {
        rockStatus.Acceleration *= boosterMultiplier;
        yield return new WaitForSeconds(duration);
        rockStatus.Acceleration /= boosterMultiplier;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Booster"))
        {
            Vector3 forceDirection = transform.forward.normalized; // 힘 방향 앞쪽으로 고정
            Rrb.AddForce(Vector3.up * 30f, ForceMode.VelocityChange);
            Rrb.AddForce(forceDirection * rockStatus.Acceleration * 100.0f, ForceMode.Acceleration);
            StartCoroutine(ApplyBooster(0.01f, 100f));
        }
    }
}
