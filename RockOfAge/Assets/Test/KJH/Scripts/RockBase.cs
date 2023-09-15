using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBase : MonoBehaviour
{
    protected float jumpForce = 10000f;
    protected float attackPowerBase = 10f;

    public float maxSpeed = 50f;
    public RockStatus rockStatus;
    public LayerMask terrainLayer; // Inspector에서 "Terrains" 레이어를 할당할 수 있는 변수 추가
    protected Rigidbody rRb;
    protected Camera mainCamera;

    public  virtual void Init()
    {
        rRb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    // 돌의 레이거리를 확인하기 위한 함수
    //private void OnDrawGizmos() 
    //{
    //    Gizmos.DrawCube(transform.position, new Vector3(1, 8, 1));
    //}
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
            rRb.AddForce(forceDirection * acceleration, ForceMode.Acceleration);
        }
        //최대속도 제한
        float currentSpeed = rRb.velocity.magnitude;
        if(currentSpeed > maxSpeed)
        {
            rRb.velocity = rRb.velocity.normalized * maxSpeed;
        }
    }

    public virtual void Jump()
    {
        if (IsGround())
        {
            rRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    public virtual bool IsGround()
    {
        
        float distance = 0.5f; // 레이 캐스트 거리
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
    public void ApplyBoosterEffect(float duration, float boostForce, float upForce, Vector3 direction)
    {
        rRb.velocity = Vector3.zero;
        rRb.AddForce(Vector3.up * upForce, ForceMode.Impulse);
        StartCoroutine(ApplyBooster(duration, boostForce, direction));
    }

    protected IEnumerator ApplyBooster(float duration, float boostForce, Vector3 direction)
    {
        float time = 0;
        while (time < duration)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            rRb.velocity = direction * boostForce - Vector3.Cross(direction, Vector3.up).normalized * horizontalInput * 100;
            yield return new WaitForSeconds(Time.deltaTime);
            time += Time.deltaTime;
        }
    }

    //else if (other.CompareTag("JumpPad")) // 점프대 태그를 확인합니다.
    //{

    //    rRb.AddForce(Vector3.up * 50000f, ForceMode.Acceleration); // 점프대 속도를 적용합니다.
    //}
}
