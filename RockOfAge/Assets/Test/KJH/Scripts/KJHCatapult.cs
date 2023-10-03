
using System.Collections;
using UnityEngine;
// ! Photon
using Photon.Pun;

public class KJHCatapult : HoldObstacleBase, IHitObjectHandler
{
    public float detectionRadius = 5f; // 원형 감지 범위 반지름
    public LayerMask Rock; // 감지할 레이어 설정
    public float rotationSpeed = 1.0f; // 회전 속도 조절 변수

    public Transform throwPoint; // 돌을 던질 위치
    public GameObject rockPrefab; // 던질 돌의 프리팹
    public Animator animator; // 애니메이터 컴포넌트에 대한 참조
    public AudioClip rotateSound;
    public AudioClip throwingSound;
    public AudioClip relodingSound;
    public AudioClip DeadSound;

    private Quaternion initialRotation; // 투석기의 초기 로테이션
    private bool canThrowRock = true; // 돌을 던질 수 있는 상태인지 여부를 나타내는 변수
    private Vector3 targetPosition; // 감지한 객체의 위치
    private bool hasThrownRock = false; // 돌을 이미 발사했는지 여부를 나타내는 변수
    private Vector3 currentRockPosition;

    protected override void Init()
    {
        base.Init();
        animator = GetComponent<Animator>();
        // 투석기의 초기 로테이션을 저장합니다.
        initialRotation = Quaternion.Euler(Vector3.zero);
    }

    void Update()
    {
        if (!isBuildComplete)
        {
            return;

        }
        // 투석기의 현재 위치
        Vector3 catapultPosition = transform.position;

        // 원형 감지 범위 내의 모든 Collider 가져오기
        Collider[] colliders = Physics.OverlapSphere(catapultPosition, detectionRadius, Rock);
        if(colliders.Length > 0 )
        {
            foreach (Collider collider in colliders)
            {
                // 감지한 객체의 위치를 전역 변수로 변경
                targetPosition = collider.transform.position;
                if (collider.GetComponentInParent<RockBase>()==null)
                {
                    continue;
                }

                // 투석기를 감지한 객체(Rock)를 향하도록 회전시키기
                Vector3 directionToTarget = (targetPosition - catapultPosition).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

                //animator.SetTrigger("Attack");

                // 투석기의 초기 로테이션을 기준으로 회전
                targetRotation *= initialRotation;
                //x와 z 로테이션을 초기 로테이션으로 고정
                targetRotation.eulerAngles = new Vector3(initialRotation.eulerAngles.x, targetRotation.eulerAngles.y, initialRotation.eulerAngles.z);

                // 회전 속도를 적용하여 부드럽게 회전
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                if(!audioSource.isPlaying)
                {
                    audioSource.clip = rotateSound;
                    audioSource.Play();
                }
                // 투석기와 돌 사이의 각도 계산
                float angleToRock = Vector3.Angle(transform.forward, directionToTarget);

                // 일직선 상에 있는지 확인하고 애니메이션을 실행하거나 종료합니다.
                float angleThreshold = 10f; // 각도 임계값을 설정합니다. 필요한 경우 이 값을 조절할 수 있습니다.
                if (angleToRock <= angleThreshold)
                {
                    if (collider.GetComponentInParent<RockBase>().photonView.IsMine)
                    {
                        photonView.RPC("PlayAttackAnimation", RpcTarget.All);
                        break;
                    }
                }
                else
                {
                    animator.SetBool("Attack", false);
                }
            }
            // 돌을 이미 발사했고 바위가 투석기 감지 범위를 벗어난 경우에만
            // 돌을 다시 던질 수 있게 설정
            if (hasThrownRock && Vector3.Distance(catapultPosition, currentRockPosition) > detectionRadius)
            {
                hasThrownRock = false;
            }
        }
        else
        {
            // 감지 범위 내에 바위가 없을 때는 애니메이션을 종료
            animator.SetBool("Attack", false);
        }
    }


    void OnDrawGizmos()
    {
        // 원형 감지 범위의 색상을 설정합니다.
        Gizmos.color = Color.red;

        // 원형 감지 범위를 그립니다.
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    [PunRPC]
    public void PlayAttackAnimation()
    {
        if (!animator.GetBool("Attack"))
        {
            animator.SetBool("Attack", true);
        }
    }


    void ThrowRock()
    {
        if (rockPrefab != null && throwPoint != null && canThrowRock)
        {
            // 돌을 throwPoint 위치에서 생성하고 발사합니다.
            GameObject rock = Instantiate(rockPrefab, throwPoint.position, throwPoint.rotation);
            Debug.Log(rock);

            // 투석기와 돌 사이의 방향 및 거리 계산
            Vector3 directionToTarget = (targetPosition - throwPoint.position).normalized;
            float distanceToTarget = (targetPosition - throwPoint.position).magnitude;

            // 발사 속도를 거리에 비례하게 설정합니다.
            float forceMultiplier = 15f;
            float force = Mathf.Sqrt(distanceToTarget * Physics.gravity.magnitude * forceMultiplier);

            // 돌을 해당 방향으로 발사합니다.
            Vector3 launchVelocity = (directionToTarget * force);
            rock.GetComponent<Rigidbody>().velocity = launchVelocity;
            audioSource.clip = throwingSound;
            audioSource.Play();

            // 일정 시간(rockDestroyDelay)이 지난 후에 돌을 제거
            StartCoroutine(DestroyRock(rock));
        }
    }
    IEnumerator DestroyRock(GameObject rock)
    {
        yield return new WaitForSeconds(1f);
        // ! Photon
        Destroy(rock);
    }
    protected override void Dead() 
    {
        audioSource.clip = DeadSound;
        audioSource.Play();

        // 1.0초 후에 사라지는 로직을 실행
        Invoke("Disappear", 1.0f);
    }
    private void Disappear()
    {
        // 게임 오브젝트를 비활성화하거나 파괴
        PhotonNetwork.Destroy(gameObject);

        // 또는 Destroy(gameObject);
    }

    public void Hit(int damage)
    {
        Debug.Log("맞았는가");
        currHealth -= damage;
        if (currHealth <= 0)
        {
            Dead();
        }
    }
public void HitReaction()
    {
        throw new System.NotImplementedException();
    }
}