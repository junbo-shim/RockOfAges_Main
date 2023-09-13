using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBase : MonoBehaviour
{
    protected float jumpForce = 3000f;
    protected float attackPowerBase = 10f;

    public float maxSpeed = 50f;
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
        //최대속도 제한
        float currentSpeed = Rrb.velocity.magnitude;
        if(currentSpeed > maxSpeed)
        {
            Rrb.velocity = Rrb.velocity.normalized * maxSpeed;
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
            Rrb.AddForce(forceDirection * rockStatus.Acceleration * 300.0f, ForceMode.Impulse);
            Rrb.AddForce(Vector3.up * 5000f, ForceMode.Impulse);
            StartCoroutine(ApplyBooster(0.5f, 300f));
        }
    }
}
