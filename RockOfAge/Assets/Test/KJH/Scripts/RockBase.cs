using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBase : MonoBehaviour
{
    protected float jumpForce = 3000f;
    protected float attackPowerBase = 10f;

    public float maxSpeed = 50f;
    public RockStatus rockStatus;
    public LayerMask terrainLayer; // Inspector에서 "Terrains" 레이어를 할당할 수 있는 변수 추가
    protected Rigidbody Rrb;
    protected Camera mainCamera;

    public  virtual void Init()
    {
        Rrb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, new Vector3(1, 8, 1));
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
        if (IsGround())
        {
            Debug.Log("IsGround() is true");
            Rrb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else
        {
            Debug.Log("IsGround() is false");
        }
    }
    public virtual bool IsGround()
    {
        
        float distance = 4f; // 레이 캐스트 거리
        RaycastHit hit;

        // 레이 캐스트를 사용하여 지면과의 거리를 확인합니다.
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distance, terrainLayer))
        {
            return true;
        }

        return false;
    }


    public virtual float Attack()
    {
        return 0;
    }

    public virtual void Fall()
    {

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
