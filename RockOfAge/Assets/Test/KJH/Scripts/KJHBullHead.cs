using UnityEngine;
using System.Collections;
public class KJHBullHead : MoveObstacleBase, IHitObjectHandler
{
    public float detectionRadius = 5f;     // 바위 감지 범위
    public float detectionAngle = 90f;     // 바위 감지 각도
    public LayerMask rockLayer;           // 바위 레이어
    public float chargeSpeed = 5f;        // 돌진 속도
    public float returnSpeed = 3f;        // 원래 위치로 돌아가는 속도

    private Vector3 originalPosition;      // 원래 위치
    private Vector3 lastDetectedRockPosition; // 마지막으로 감지된 바위 위치
    private bool isCharging = false;       // 돌진 중 여부
    private bool isReturning = false;      // 원래 위치로 복귀 중 여부
    private Animator animator; // Animator 컴포넌트 참조

    private float stareTime = 0.8f; // 바위를 바라보는 시간 
    private bool isStaring = false; // 바위를 바라보고 있는지 여부
    private float stareTimer = 0f; // 바위를 바라보기 시작한 시간을 추적하는 타이머


    void Start()
    {
        originalPosition = transform.position; // 시작 시 원래 위치 저장

        animator = GetComponent<Animator>(); // Animator 컴포넌트 가져오기
    }

    void Update()
    {
        if (!isCharging && !isReturning && !isStaring)
        {
            // 감지 범위 내에서 바위 감지
            Collider[] detectedRocks = Physics.OverlapSphere(transform.position, detectionRadius, rockLayer);

            foreach (Collider rock in detectedRocks)
            {
                this.target = rock.gameObject;
                Vector3 directionToRock = (rock.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToRock);

                if (angle <= detectionAngle * 0.5f)
                {
                    Debug.Log("바위 감지: " + rock.name);
                    lastDetectedRockPosition = rock.transform.position;
                    isStaring = true;
                    stareTimer = Time.time;

                    // Attack 애니메이션 트리거 리셋
                    animator.ResetTrigger("Attack");

                    break;
                }
            }
        }
        else if (isStaring)
        {
            // 바위를 바라보는 동안 회전
            Vector3 direction = (target.transform.position - transform.position).normalized;
            direction.y = 0; // Y 축 이동 금지
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * chargeSpeed);

            // 2초가 지났는지 확인
            if (Time.time - stareTimer >= stareTime)
            {
                lastDetectedRockPosition = target.transform.position; 
                isStaring = false;
                isCharging = true;
            }
        }
       else if (isCharging)
    {
        ChargeToLastDetectedRock();
    }
        else if (isReturning)
        {
            ReturnToOriginalPosition();
        }
    }

    void ChargeToLastDetectedRock()
    {
        animator.SetBool("isCharging", true);

        Vector3 direction = (lastDetectedRockPosition - transform.position).normalized;
        direction.y = 0; // Y 축 이동 금지

        // 돌진 중인 모루 황소를 이동 및 회전
        transform.position += direction * chargeSpeed * Time.deltaTime;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * chargeSpeed);

        // 도착 여부 확인
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(lastDetectedRockPosition.x, 0, lastDetectedRockPosition.z)) < 0.1f)
        {
            // 돌진 애니메이션 종료
            animator.SetBool("isCharging", false);
            
            Debug.Log("바위의 마지막 위치로 돌진 완료");
            Invoke("Wait", 1f);
        }
    }
   
    void ReturnToOriginalPosition()
    {
        Vector3 direction = (originalPosition - transform.position).normalized;
        direction.y = 0; // Y 축 이동 금지

        // 원래 위치로 복귀
        transform.position += direction * returnSpeed * Time.deltaTime;
        animator.SetBool("isReturning", true);

        // 복귀 완료 여부 확인
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(originalPosition.x, 0, originalPosition.z)) < 0.1f)
        {
            // 돌진 애니메이션 종료

            Debug.Log("원래 위치로 복귀 완료");
            isReturning = false;
            animator.SetBool("isReturning", false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isCharging && collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            Debug.Log("바위와 충돌: " + collision.gameObject.name);


            // 공격 애니메이션 시작
            // 충돌한 바위에 힘을 전달
            Rigidbody rockRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (rockRigidbody != null)
            {
                Vector3 forceDirection = (collision.transform.position - transform.position).normalized;
                float forceMagnitude = GetComponent<Rigidbody>().mass * chargeSpeed;
                rockRigidbody.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
            }
            Debug.Log(collision.contacts[0].thisCollider);
           
                // 몸통이 아닌 다른 부분에 충돌했을 때의 처리
                Debug.Log("어딘가에 충돌");
                animator.SetTrigger("Attack");
            
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            Debug.Log("바위와 충돌: " + collision.gameObject.name);


          
            Debug.Log(collision.contacts[0].thisCollider);
            // 충돌한 콜라이더가 몸통인지 확인
            if (collision.contacts[0].thisCollider.CompareTag("BullBody"))
            {
                // 몸통에 충돌했을 때 모루 황소를 죽이는 코드를 여기에 추가하세요.
                Debug.Log("몸통에 충돌");
                Die();
            }
            else
            {
                // 몸통이 아닌 다른 부분에 충돌했을 때의 처리
                Debug.Log("어딘가에 충돌");
                animator.SetTrigger("Attack");
            }
        }
    }

    void OnDrawGizmos()
    {
        // 부채꼴 감지 범위를 그립니다.
        Gizmos.color = Color.blue;
        float step = detectionAngle / 20; // 부채꼴을 20개의 선분으로 나눕니다.
        for (float angle = -detectionAngle * 0.5f; angle <= detectionAngle * 0.5f; angle += step)
        {
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            Gizmos.DrawLine(transform.position, transform.position + direction * detectionRadius);
        }

        // 원형 감지 범위를 그립니다.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    public void Wait()
    {
        isCharging = false;
        isReturning = true;

    }
    private void Die()
    {
        // Die 애니메이션 재생
        animator.SetTrigger("Die");

        // 1초 후에 사라지는 로직을 실행
        Invoke("Disappear", 2f);
    }

    private void Disappear()
    {
        // 게임 오브젝트를 비활성화하거나 파괴
        gameObject.SetActive(false);
        // 또는 Destroy(gameObject);
    }
    public void Hit(int damage)
    {
        // 아직 구현되지 않음
    }

    public void HitReaction()
    {
        // 아직 구현되지 않음
    }
}