using Unity.VisualScripting;
using UnityEngine;

public class KJHBullController : MoveObstacleBase
{
    public float detectionRange = 10.0f; // 감지 범위
    public float detectionAngle = 45.0f; // 감지 각도
    public LayerMask Rock; // 바위 레이어

    public int numSegments = 50; // 부채꼴 시각화 세그먼트 개수
    public float yOffset = 1.0f; // 부채꼴 시각화 y 오프셋

   

  
    private void Start()
    {
       
    }

    void Update()
    {
        FindAndRotateToClosestRock();
    }
    private void FindAndRotateToClosestRock()
    {
        // 가장 가까운 바위를 찾기 위한 변수 초기화
        GameObject closestRock = null;
        float minDistance = Mathf.Infinity;

        // 감지 범위 내의 모든 바위를 찾음
        Collider[] rocksInRange = Physics.OverlapSphere(transform.position, detectionRange, Rock);
        foreach (Collider rockCollider in rocksInRange)
        {
            // 바위를 향한 방향 벡터 계산
            Vector3 directionToRock = (rockCollider.transform.position - transform.position).normalized;
            // 현재 바위와의 각도 계산
            float angle = Vector3.Angle(transform.forward, directionToRock);

            // 각도가 감지 각도 내에 있는지 확인
            if (angle <= detectionAngle)
            {
                // 현재 바위와의 거리 계산
                float distance = Vector3.Distance(transform.position, rockCollider.transform.position);
                // 가장 가까운 바위를 찾음
                if (distance < minDistance)
                {
                    closestRock = rockCollider.gameObject;
                    minDistance = distance;
                }
            }
        }

        // 가장 가까운 바위가 있으면 회전
        if (closestRock != null)
        {
            // 바위를 향한 방향 벡터 계산 (y축 제외)
            Vector3 direction = (closestRock.transform.position - transform.position).normalized;
            direction.y = 0;
            // 목표 회전값 계산
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // 현재 x축 회전값 고정
            float fixedXRotation = transform.rotation.eulerAngles.x;
            // 회전값을 부드럽게 변경
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
            // x축 회전값 적용
            transform.rotation = Quaternion.Euler(fixedXRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }

    // 부채꼴 시각화
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 origin = transform.position + new Vector3(0, yOffset, 0); // y값을 올림
        DrawSector(origin, transform.forward, detectionRange, detectionAngle, numSegments);
    }

    // 부채꼴 그리기 함수
    void DrawSector(Vector3 origin, Vector3 forward, float radius, float angle, int segments)
    {
        float stepAngle = angle * 2 / segments;
        Vector3 previousPoint = origin + Quaternion.Euler(0, -angle, 0) * forward * radius;

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = -angle + stepAngle * i;
            Vector3 currentPoint = origin + Quaternion.Euler(0, currentAngle, 0) * forward * radius;
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }

        Gizmos.DrawLine(origin, origin + Quaternion.Euler(0, -angle, 0) * forward * radius);
        Gizmos.DrawLine(origin, origin + Quaternion.Euler(0, angle, 0) * forward * radius);
    }
}