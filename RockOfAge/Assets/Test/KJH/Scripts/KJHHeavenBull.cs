using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class KJHHeavenBull : MoveObstacleBase, IHitObjectHandler
{
    /*감지범위내에 있는지 확인
     감지범위 안에 있다면 3초후 공격
    -공격타이밍에 땅에있다면 머드에 묵임
    -공격타이밍에 땅에 없으면 공격 안당함*/



    // 감지 범위를 설정하는 변수
    public float detectionRadius = 5f;
    // 감지할 대상의 레이어를 지정하는 변수
    public LayerMask Rock;
    // 머드 프리팹
    public GameObject mudPrefab;

    public float mudDuration = 1f;
    public AudioClip attackSound;
    public AudioClip attackCharge;
    public AudioClip idleSound;
    public AudioClip idleSound2;

    // 가장 가까운 돌의 위치
    private Transform nearRock;
    // 공격 목표물 위치
    private Vector3 targetPosition;
    // 공격 여부 확인
    private bool isAttacking = false;
    private bool isInsideRange = false;
    private GameObject mudObject;

    protected override void Init()
    {
        base.Init();
        obstacleAnimator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
    }


    private void FixedUpdate()
    {
        DetectRock();
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, Rock);
        bool detectedRock = colliders.Length > 0;

        if (detectedRock && !isInsideRange && !isAttacking)
        {
            // 범위 안에 들어왔을 때 소리 활성화
            audioSource.enabled = true;
            isInsideRange = true;

            // 감지 범위 내에 돌이 있을 때 애니메이션을 실행
//            obstacleAnimator.SetTrigger("Attack");
            photonView.RPC("PlayAnimationTrigger",RpcTarget.All, "Attack");
            StartCoroutine(WaitAni());
        }
        else if (!detectedRock && isInsideRange)
        {
            // 범위를 벗어났을 때 소리 비활성화
            audioSource.enabled = false;
            isInsideRange = false;
        }

        //if (colliders.Length > 0 && !isAttacking)
        //{
        //    // 감지 범위 내에 돌이 있을 때 애니메이션을 실행
        //    animator.SetTrigger("Attack");
            
        //    StartCoroutine(WaitAni());
        //}
    }

    private IEnumerator WaitAni()
    {
        isAttacking = true;

        yield return new WaitForSeconds(5f);

        isAttacking = false;
    }
    public void DetectRock()
    {
        // 주변에 있는 돌을 감지하고 배열에 저장
        Collider[] colls = Physics.OverlapSphere(transform.position, detectionRadius, Rock);
        // nearDistance 변수 초기화 (가장 까가운 돌을 감지하기 위한 변수)
        float nearDistance = Mathf.Infinity;

        bool isRockDetected = false;

        foreach (Collider coll in colls)
        {
            targetPosition = coll.transform.position;
            // 두 Vector3 위치 사이의 거리를 계산하는 Unity 엔진의 함수 coll = Rcok
            float distance = Vector3.Distance(transform.position, coll.transform.position);
            if (coll.gameObject.layer == LayerMask.NameToLayer("Rock"))
            {
                if (distance < nearDistance)
                {
                    nearDistance = distance;
                    nearRock = coll.transform;
                    LookRock(coll.transform);
                    isRockDetected = true;
                }
            }
        }
        if(!isRockDetected)
        {
            isAttacking=false;
        }
    }

    public void LookRock(Transform target)
    {
        if (nearRock != null)
        {
            // 현재 객체와 돌 사이의 방향 벡터 계산
            Vector3 direction = target.position - transform.position;
            // y 축 회전을 고려하지 않도록 설정
            direction.y = 0;
            // 현재 객체를 계산된 방향으로 회전시킴
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // 부드럽게 회전하기위해 보간 사용
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
        }
    }

    public void AttackRocks()
    {
        audioSource.clip = attackSound;
        audioSource.Play();

        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, Rock);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Rock"))
            {
                RockBase rock = collider.GetComponentInParent<RockBase>();

                if (rock != null && rock.isGround)
                {
                    Vector3 mudDirection = new Vector3(0, 1f, 0);
                    Vector3 mudPosition = new Vector3(collider.transform.position.x, collider.transform.position.y - 0.3f, collider.transform.position.z);
                    Quaternion mudRotation = Quaternion.LookRotation(mudDirection);
                    GameObject mudObject = Instantiate(mudPrefab, mudPosition, mudRotation);

                    Rigidbody mudRb = mudObject.GetComponent<Rigidbody>();
                    if (mudRb != null)
                    {
                        mudRb.isKinematic = true;
                    }

                    StartCoroutine(DestroyMudObject(mudObject, mudDuration));

                    Rigidbody rockRb = collider.GetComponent<Rigidbody>();
                    if (rockRb != null)
                    {
                        rockRb.isKinematic = true;
                    }
                }
            }
        }
    }

    IEnumerator DestroyMudObject(GameObject mudObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(mudObject);

        // 머드 프리팹이 삭제된 후에 바위의 Rigidbody를 다시 움직이게 설정
        Rigidbody rockRb = nearRock.GetComponent<Rigidbody>();
        if (rockRb != null)
        {
            rockRb.isKinematic = false;
        }
    }

    public void AttackChargeSound()
    {
        audioSource.clip = attackCharge;
        audioSource.Play();
    }
    public void IdleOne()
    {
        if(!isAttacking)
        {
        audioSource.clip = idleSound;
        audioSource.Play();

        }
    }
    public void IdleTwo()
    {
        if (isAttacking)
        {
        audioSource.clip = idleSound2;
        audioSource.Play(); 
            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    protected override void Dead()
    {
        // Die 애니메이션 재생
        photonView.RPC("PlayAnimationTrigger", RpcTarget.All, "Die");
        GetComponent<Rigidbody>().isKinematic = true; // 물리 시뮬레이션 비활성화
        GetComponent<Collider>().enabled = false; // 콜라이더 비활성화 (옵션)

        // 1.0초 후에 사라지는 로직을 실행
        Invoke("DestroyPhotonViewObject", 1f);
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
