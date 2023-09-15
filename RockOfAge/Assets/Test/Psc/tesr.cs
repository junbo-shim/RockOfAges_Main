using UnityEngine;

public class tesr : MonoBehaviour
{
    Vector3 direction;

    private Rigidbody rb;

    [SerializeField]
    private float jumpForce = default;
    [SerializeField]
    private float accel = default;
    [SerializeField]
    private float maxSpeed = default;
    [SerializeField]
    private float mass = default;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = mass;
    }

    private void Update()
    {
        Vector2 input = IsInput();
        if (input.magnitude > 0)
        {
            Move(input);
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGround())
        {
            Jump(jumpForce);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position - Vector3.up * gameObject.GetHeight(.5f), .05f);
    }

    void Move(Vector2 input)
    {
        Vector3 inputDirection = (Camera.main.transform.forward * input.y + Camera.main.transform.right * input.x).normalized;
        direction = (direction.normalized + inputDirection).normalized;
        direction *= accel * Time.deltaTime;

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        else if (rb.velocity.magnitude < 1)
        {
            rb.velocity += direction.normalized;

        }
        else
        {
            rb.velocity += new Vector3(direction.x, 0, direction.z);
        }

    }
    void Jump(float power)
    {
        rb.velocity += Vector3.up * power;
    }

    bool IsGround()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position - Vector3.up * gameObject.GetHeight(.5f), .05f, Global_PSC.FindLayerToName("Terrains"));

        if (colliders.Length > 0)
        {
            return true;
        }
        return false;
    }

    Vector2 IsInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        return new Vector2(horizontalInput, verticalInput);
    }

    bool IsMove()
    {
        return  rb.velocity.magnitude > 0;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Obstacles")) 
        {
            if (IsMove())
            {
                foreach (ContactPoint contact in collision.contacts)
                {

                    if (contact.thisCollider)
                    {
                        // 충돌 지점의 법선 벡터와 gameobject의 진행 방향을 계산합니다.
                        Vector3 collisionNormal = contact.normal;
                        Vector3 forwardDirection = rb.velocity.normalized;

                        // 두 벡터의 각도를 계산합니다.
                        float angle = Vector3.Angle(collisionNormal, forwardDirection);

                        // 일정 각도 이내의 충돌을 확인합니다.
                        float maxCollisionAngle = 45f; // 예시: 45도 이내의 충돌을 확인
                        if (angle <= maxCollisionAngle)
                        {
                            // 일정 각도 이내의 충돌이 발생한 경우
                            Debug.Log("일정 각도 이내의 충돌 발생!");
                        }
                    }
                  
                }

            }
        }
    }
}