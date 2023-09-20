using Unity.VisualScripting;
using UnityEngine;

public class KJHBullController : MoveObstacleBase, IHitObjectHandler
{
    public float detectionRadius = 5f;
    public float detectionAngle = 90f;
    public LayerMask rockLayer;
    public float chargeSpeed = 5f;
    public float returnSpeed = 3f;

    private Vector3 originalPosition;
    private Vector3 lastDetectedRockPosition;
    private bool isCharging = false;
    private bool isReturning = false;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (!isCharging && !isReturning)
        {
            Collider[] detectedRocks = Physics.OverlapSphere(transform.position, detectionRadius, rockLayer);

            foreach (Collider rock in detectedRocks)
            {
                Vector3 directionToRock = (rock.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToRock);

                if (angle <= detectionAngle * 0.5f)
                {
                    Debug.Log("Rock detected: " + rock.name);
                    lastDetectedRockPosition = rock.transform.position;
                    isCharging = true;
                    break;
                }
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
        Vector3 direction = (lastDetectedRockPosition - transform.position).normalized;
        direction.y = 0; // y값을 0으로 고정합니다.
        transform.position += direction * chargeSpeed * Time.deltaTime;

        // 모루황소가 돌을 바라보도록 회전
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * chargeSpeed);

        // 도착 여부 확인
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(lastDetectedRockPosition.x, 0, lastDetectedRockPosition.z)) < 0.1f)
        {
            Debug.Log("Arrived at the last detected rock position");
            isCharging = false;
            isReturning = true;
        }
    }

    void ReturnToOriginalPosition()
    {
        Vector3 direction = (originalPosition - transform.position).normalized;
        direction.y = 0; // y값을 0으로 고정합니다.
        transform.position += direction * returnSpeed * Time.deltaTime;

        // 원래 위치로 돌아왔는지 확인
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(originalPosition.x, 0, originalPosition.z)) < 0.1f)
        {
            Debug.Log("Returned to the original position");
            isReturning = false;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (isCharging && collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            Debug.Log("Hit a rock: " + collision.gameObject.name);
            // 충돌한 바위에 힘을 가합니다.
            Rigidbody rockRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (rockRigidbody != null)
            {
                Vector3 forceDirection = (collision.transform.position - transform.position).normalized;
                float forceMagnitude = GetComponent<Rigidbody>().mass * chargeSpeed;
                rockRigidbody.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
            }
        }
    }
    void OnDrawGizmos()
    {
        // 부채꼴 감지 범위를 그립니다.
        Gizmos.color = Color.green;
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


    public void Hit(int damage)
    {
        throw new System.NotImplementedException();
    }

    public void HitReaction()
    {
        throw new System.NotImplementedException();
    }
}

