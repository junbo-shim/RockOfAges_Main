//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Catapult : MonoBehaviour
//{
//    public float detectionRadius = 5f; // 원형 감지 범위 반지름
//    public LayerMask Rock; // 감지할 레이어 설정
//    private Quaternion initialRotation; // 투석기의 초기 로테이션

//    void Start()
//    {
//        // 투석기의 초기 로테이션을 저장합니다.
//        initialRotation = transform.rotation;
//    }

//    void Update()
//    {
//        // 투석기의 현재 위치
//        Vector3 catapultPosition = transform.position;

//        // 원형 감지 범위 내의 모든 Collider 가져오기
//        Collider[] colliders = Physics.OverlapSphere(catapultPosition, detectionRadius, Rock);

//        foreach (Collider collider in colliders)
//        {
//            // 감지한 객체의 위치
//            Vector3 targetPosition = collider.transform.position;

//            // 투석기를 감지한 객체(Rock)를 향하도록 회전시키기
//            Vector3 directionToTarget = (targetPosition - catapultPosition).normalized;
//            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

//            // 투석기의 초기 로테이션을 기준으로 회전
//            targetRotation *= initialRotation;

//            // 투석기의 회전을 부드럽게 변경하려면 Quaternion.Slerp 또는 Quaternion.RotateTowards를 사용할 수 있습니다.

//            // Y와 X 로테이션을 초기 로테이션으로 고정
//            targetRotation.eulerAngles = new Vector3(initialRotation.eulerAngles.x, targetRotation.eulerAngles.y, initialRotation.eulerAngles.z);

//            // 투석기의 회전 적용
//            transform.rotation = targetRotation;

//            // 여기에서 필요한 추가 동작을 수행하세요.
//        }
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : MonoBehaviour
{
    public float detectionRadius = 5f; // 원형 감지 범위 반지름
    public LayerMask Rock; // 감지할 레이어 설정
    public float rotationSpeed = 1.0f; // 회전 속도 조절 변수
    public float throwAngleThreshold = 10f; // 돌 던짐 각도 임계값
    public Transform throwPoint; // 돌을 던질 위치
    public GameObject rockPrefab; // 던질 돌의 프리팹

    private Quaternion initialRotation; // 투석기의 초기 로테이션

    void Start()
    {
        // 투석기의 초기 로테이션을 저장합니다.
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // 투석기의 현재 위치
        Vector3 catapultPosition = transform.position;

        // 원형 감지 범위 내의 모든 Collider 가져오기
        Collider[] colliders = Physics.OverlapSphere(catapultPosition, detectionRadius, Rock);

        foreach (Collider collider in colliders)
        {
            // 감지한 객체의 위치
            Vector3 targetPosition = collider.transform.position;

            // 투석기를 감지한 객체(Rock)를 향하도록 회전시키기
            Vector3 directionToTarget = (targetPosition - catapultPosition).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

            // 투석기의 초기 로테이션을 기준으로 회전
            targetRotation *= initialRotation;

            //x와 z 로테이션을 초기 로테이션으로 고정
            targetRotation.eulerAngles = new Vector3(initialRotation.eulerAngles.x, targetRotation.eulerAngles.y, initialRotation.eulerAngles.z);

            // 회전 속도를 적용하여 부드럽게 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 투석기와 돌 사이의 각도 계산
            float angleToRock = Vector3.Angle(transform.up, directionToTarget);

            if (angleToRock <= throwAngleThreshold)
            {
                ThrowRock();
            }
        }
    }
    void ThrowRock()
    {
        if (rockPrefab != null && throwPoint != null)
        {
            // 돌을 throwPoint 위치에서 생성하고 발사합니다.
            GameObject rock = Instantiate(rockPrefab, throwPoint.position+Vector3.up * 2, throwPoint.rotation);
            Debug.Log(rock);
            // 돌을 앞으로 발사합니다. 이동 방향은 throwPoint의 up 방향입니다.
            rock.GetComponent<Rigidbody>().AddForce(throwPoint.up * 1000f);
        }
    }
}