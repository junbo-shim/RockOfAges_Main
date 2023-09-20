//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class KJHRockID1 : RockBase, IHitObjectHandler
//{
//    public float jumpForce = 50;
//    public float maxSpeed = 100f;
//    public float gravityStrength = 100f; // 중력 세기 (기본값: 지구 중력)
//    public float attackPowerBase = 10;
//    protected Rigidbody rRb;
//    public LayerMask terrainLayer; // Inspector에서 "Terrains" 레이어를 할당할 수 있는 변수 추가
//    void Awake()
//    {
//        rRb = GetComponent<Rigidbody>();
//        rockStatus = new RockStatus(rockStatus);
//    }
//    void Start()
//    {
//        Init(); // RockBase 클래스의 Init 메서드를 호출합니다.
//    }
//    void FixedUpdate()
//    {
//        Move(); // RockBase 클래스의 Move 메서드를 호출합니다.
//        if (!IsGround())
//        {
//            // 중력 힘을 직접 적용
//            Vector3 gravityForce = Vector3.down * gravityStrength;
//            rRb.AddForce(gravityForce, ForceMode.Acceleration);
//        }
//    }
//    void Update()
//    {

//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            Jump(); // RockBase 클래스의 Jump 메서드를 호출합니다.
//        }
//    }


//    public virtual void Jump()
//    {
//        if (IsGround())
//        {
//            rRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
//        }
//    }

//    public virtual void Move()
//    {
//        float horizontalInput = Input.GetAxis("Horizontal");
//        float verticalInput = Input.GetAxis("Vertical");

//        if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f)
//        {
//            Vector3 cameraForward = mainCamera.transform.forward;
//            Vector3 cameraRight = mainCamera.transform.right;

//            Vector3 forceDirection = (cameraForward * verticalInput + cameraRight * horizontalInput);
//            forceDirection.y = 0;

//            // 이전 프레임에서의 벨로시티 방향
//            Vector3 previousVelocityDirection = rockRigidbody.velocity.normalized;

//            // 새로운 이동 방향과 이전 벨로시티 방향 사이의 각도 계산
//            float angle = Vector3.Angle(previousVelocityDirection, forceDirection);

//            // 각도에 따라 벨로시티 값을 조절합니다.
//            // 여기서 45도 이상인 경우를 빠른 반응으로 설정합니다.
//            if (angle > 45f)
//            {
//                // 빠른 반응을 위해 벨로시티 값을 새 방향으로 바로 변경합니다.
//                rRb.velocity = forceDirection.normalized * maxSpeed + rRb.velocity * 0.5f;
//                rRb.angularVelocity = forceDirection.normalized * maxSpeed + rRb.angularVelocity * 0.5f;
//            }
//            else
//            {
//                // 그렇지 않으면 가속도를 사용하여 천천히 방향을 변경합니다.
//                float acceleration = (Mathf.Abs(horizontalInput) > 0.1f && Mathf.Abs(verticalInput) > 0.1f) ? rockStatus.Acceleration : rockStatus.Acceleration * 2;

//                // 가속도를 사용하여 velocity를 계산하고 적용합니다.
//                Vector3 newVelocity = rRb.velocity + forceDirection * acceleration * Time.deltaTime;

//                // 최대 속도 제한
//                if (newVelocity.magnitude > maxSpeed)
//                {
//                    newVelocity = newVelocity.normalized * maxSpeed;
//                }

//                rRb.velocity = newVelocity;
//            }
//        }
//    }
//    protected override bool IsGround()
//    {

//        float distance = 3f; // 레이 캐스트 거리
//        RaycastHit hit;

//        // 레이 캐스트를 사용하여 지면과의 거리를 확인합니다.
//        if (Physics.Raycast(transform.position, Vector3.down, out hit, distance, terrainLayer))
//        {
//            return true;
//        }

//        return false;
//    }


//    protected override float Attack()
//    {
//        float attackPower;
//        // 공격력을 현재 속도에 비례하게 계산합니다.
//        float currentSpeed = rRb.velocity.magnitude;
//        attackPower = attackPowerBase * (rockStatus.Health + currentSpeed);
//        return attackPower;
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.CompareTag("Target"))
//        {
//            IHitObjectHandler hitObj = collision.gameObject.GetComponent<IHitObjectHandler>();
//            if (hitObj != null)
//            {
//                Attack();
//                hitObj.Hit((int)Attack());
//            }
//        }
//    }

//    public void Hit(int damage)
//    {
//        rockStatus.Health -= damage;
//    }

//    public void HitReaction()
//    {
//        throw new System.NotImplementedException();
//    }

//    public void ApplyBoosterEffect(float duration, float boostForce, float upForce, Vector3 direction)
//    {
//        rRb.velocity = Vector3.zero;
//        rRb.AddForce(Vector3.up * upForce, ForceMode.Impulse);
//        StartCoroutine(ApplyBooster(duration, boostForce, direction));
//    }

//    protected IEnumerator ApplyBooster(float duration, float boostForce, Vector3 direction)
//    {
//        float time = 0;
//        while (time < duration)
//        {
//            float horizontalInput = Input.GetAxis("Horizontal");
//            rRb.velocity = direction * boostForce - Vector3.Cross(direction, Vector3.up).normalized * horizontalInput * 100;
//            yield return new WaitForSeconds(Time.deltaTime);
//            time += Time.deltaTime;
//        }
//    }
//}