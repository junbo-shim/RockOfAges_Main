using UnityEngine;
using System.Collections;
using Photon.Pun;

public class KJHBullHead : MoveObstacleBase, IHitObjectHandler
{
    public float detectionRadius = 5f;     // 바위 감지 범위
    public float detectionAngle = 90f;     // 바위 감지 각도
    public LayerMask rockLayer;           // 바위 레이어
    public float chargeSpeed = 3f;        // 돌진 속도
    public float returnSpeed = 1f;        // 원래 위치로 돌아가는 속도
    public AudioClip attackSound;
    public AudioClip dieSound;

    private Vector3 originalPosition;      // 원래 위치
    private Vector3 lastDetectedRockPosition; // 마지막으로 감지된 바위 위치
    private bool isCharging = false;       // 돌진 중 여부
    private bool isReturning = false;      // 원래 위치로 복귀 중 여부
    private bool isWait = false;      // 아무것도 안하는 대기상태(탐색조차 하지않는 상태)

    private float stareTime = 0.8f; // 바위를 바라보는 시간 
    private bool isStaring = false; // 바위를 바라보고 있는지 여부
    private float stareTimer = 0f; // 바위를 바라보기 시작한 시간을 추적하는 타이머

    private float idleSoundDelay = 2.0f; // 재생 딜레이 설정 (예: 5초)



    protected override void Init()
    {
        base.Init();
        originalPosition = transform.position; // 시작 시 원래 위치 저장

        obstacleAnimator = GetComponent<Animator>(); // Animator 컴포넌트 가져오기

        
    }

    Vector3 GetProjectionVector(Vector3 vector)
    {
        
        return Vector3.ProjectOnPlane(vector, GetPlaneNormal());

    }

    Vector3 GetPlaneNormal()
    {
        Vector3 plane;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f, Global_PSC.FindLayerToName("Terrains")))
        {
            plane = hit.normal;
        }
        else
        {
            plane = Vector3.up;
        }

        return plane;

    }

    void Update()
    {
        if (!isBuildComplete || currHealth <= 0)
        {
            return;
        }

        //돌진, 복귀, 회전 상태가 아니면 rock 탐지
        if (!isCharging && !isReturning && !isStaring && !isWait)
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
                    //Debug.Log("바위 감지: " + rock.name);
                    lastDetectedRockPosition = rock.transform.position;
                    isStaring = true;
                    stareTimer = Time.time;
                    // Attack 애니메이션 트리거 리셋
                    obstacleAnimator.ResetTrigger("Attack");

                    break;
                }
            }
        }
        else if (isWait)
        {
            //empty
        }
        //회전
        else if (isStaring)
        {
            // 바위를 바라보는 동안 회전
            Vector3 direction = (target.transform.position - transform.position).normalized;
            direction.y = 0; // Y 축 이동 금지
            Quaternion targetRotation =  Quaternion.LookRotation(GetProjectionVector(direction));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * chargeSpeed);

            // 2초가 지났는지 확인
            if (Time.time - stareTimer >= stareTime)
            {
                lastDetectedRockPosition = target.transform.position;
                isStaring = false;
                isCharging = true;
            }
        }
        //돌진
        else if (isCharging)
        {
            ChargeToLastDetectedRock();
        }
        //복귀
        else if (isReturning)
        {
            ReturnToOriginalPosition();
        }
    }
    void ChargeToLastDetectedRock()
    {

        obstacleAnimator.SetBool("isCharging", true);

        Vector3 direction = (lastDetectedRockPosition - transform.position).normalized;
        direction.y = 0; // Y 축 이동 금지

        // 돌진 중인 모루 황소를 이동 및 회전
        Quaternion targetRotation = Quaternion.LookRotation(GetProjectionVector(direction));
        obstacleRigidBody.velocity = GetProjectionVector(direction).normalized * chargeSpeed;

        // 도착 여부 확인
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(lastDetectedRockPosition.x, 0, lastDetectedRockPosition.z)) < 0.1f)
        {
            obstacleAnimator.SetBool("isCharging", false);
            obstacleRigidBody.velocity = Vector3.zero;
            isWait = true;

            isCharging = false;
            isReturning = true;

            Invoke("Wait", 2f);
        }
    }
    void ReturnToOriginalPosition()
    {
        Vector3 direction = (originalPosition - transform.position).normalized;
        direction.y = 0; // Y 축 이동 금지

        // 원래 위치로 복귀
        obstacleRigidBody.velocity = GetProjectionVector(direction).normalized * returnSpeed;
        obstacleAnimator.SetBool("isReturning", true);

        // 복귀 완료 여부 확인
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(originalPosition.x, 0, originalPosition.z)) < 0.1f)
        {

            obstacleRigidBody.velocity = Vector3.zero;
            isWait = true;

            // 돌진 애니메이션 종료
            isReturning = false;

            Invoke("Wait", 2f);
            obstacleAnimator.SetBool("isReturning", false);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!isBuildComplete || currHealth <= 0)
        {
            return;

        }

        if (isCharging && collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            audioSource.clip = attackSound;
            audioSource.Play();
            // 충돌한 바위와의 방향 계산
            Vector3 collisionDirection = collision.contacts[0].point - transform.position;
            collisionDirection.y = 0f; // Y 축 이동 금지

            // 방향 벡터를 정규화하여 바라봐야 할 방향으로 사용
            collisionDirection.Normalize();

            // 충돌한 바위에 힘을 전달
            Rigidbody rockRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (rockRigidbody != null)
            {
                Vector3 forceDirection = (collision.transform.position - transform.position).normalized;
                float forceMagnitude = GetComponent<Rigidbody>().mass * chargeSpeed;
                rockRigidbody.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
            }

            obstacleAnimator.SetBool("isCharging", false);

            isWait = true;

            isCharging = false;
            isReturning = true;

            Invoke("Wait", 5f);
            photonView.RPC("PlayAnimationTrigger", RpcTarget.All, "Attack");
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
        isWait = false;

    }
    protected override void Dead()
    {
        audioSource.clip = dieSound;
        audioSource.Play();
        // Die 애니메이션 재생
        photonView.RPC("PlayAnimationTrigger", RpcTarget.All, "Die");

        GetComponent<Rigidbody>().isKinematic = true; // 물리 시뮬레이션 비활성화
        GetComponent<Collider>().enabled = false; // 콜라이더 비활성화 (옵션)

        //PhotonNetwork.Destroy(gameObject);
        // 1.0초 후에 사라지는 로직을 실행
        Invoke("DestroyPhotonViewObject", 1f);
    }

    public void Hit(int damage)
    {
        currHealth -= damage;
        if (currHealth <= 0)
        {
            Dead();
        }
    }

    public void HitReaction()
    {
        // 아직 구현되지 않음
    }
}