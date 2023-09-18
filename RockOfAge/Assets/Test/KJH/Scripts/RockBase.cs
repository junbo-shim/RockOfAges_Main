using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBase : MonoBehaviour, IHitObjectHandler
{
    [SerializeField]
    protected Transform rockObject;
    [SerializeField]
    protected Transform checkPoint;
    [SerializeField]
    public RockStatus rockStatus;
    [SerializeField]
    protected List<Mesh> forms;

    protected Vector2 playerInput;

    protected Camera mainCamera;
    protected Rigidbody rockRigidbody;
    protected MeshRenderer rockRenderer;
    protected MeshFilter rockMesh;
    protected MeshCollider rockCollider;

    //protected Vector3 direction;
    protected float currHp;
    protected bool isGround = false;
    protected bool isSlope = false;

    protected const float DAMAGE_LIMIT_MIN = 50f;
    protected const float SLOPE_LIMIT_MAX = 60f;
    protected const float COLLISION_ALLOW_ANGLE = 45f;
    protected const float DEFAULT_JUMP_FORCE = 5f;
    protected const float DEFAULT_GROUND_DRAG = .1f;
    protected const float DEFAULT_AIR_MULTIPLE = .3f;
    protected const float DEFAULT_SLOPE_MULTIPLE = 1.5f;

    private void OnDrawGizmos()
    {
        if (rockObject != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(rockObject.position - Vector3.up * rockObject.gameObject.GetHeight(.5f), .05f);
        }
    }

    public  virtual void Init()
    {
        //추후 시네머신 카메라로 바꿀것
        mainCamera = Camera.main;

        rockObject = transform.Find("RockObject");
        checkPoint = transform.Find("CheckPoint");

        rockRigidbody = rockObject.GetComponent<Rigidbody>();
        rockMesh = rockObject.GetComponent<MeshFilter>();
        rockCollider = rockObject.GetComponent<MeshCollider>();
        rockRenderer = rockObject.GetComponent<MeshRenderer>();

        rockStatus = new RockStatus(rockStatus);

        rockRigidbody.mass = rockStatus.Weight;
        currHp = rockStatus.Health;
    }

    protected virtual void Move()
    {
        playerInput = GetInput();
        Move(playerInput);
    }


    protected virtual void Jump()
    {
        Jump(DEFAULT_JUMP_FORCE);
    }


    protected virtual void Move(Vector2 input)
    {


        float accel = rockStatus.Acceleration;
        float maxSpeed = rockStatus.Speed;

        Vector3 inputDirection = (Camera.main.transform.forward * input.y + Camera.main.transform.right * input.x).normalized;
        Vector3 groundValocity = new Vector3(rockRigidbody.velocity.x, 0f, rockRigidbody.velocity.z);
        inputDirection *= accel * Time.deltaTime;
        groundValocity += inputDirection;

        Vector3 newVelocity = (new Vector3(inputDirection.x, 0, inputDirection.z));
        if (!isGround)
        {
            rockRigidbody.velocity += newVelocity * DEFAULT_AIR_MULTIPLE;
        }
        else if (CheckSlope())
        {
            rockRigidbody.velocity += GetSlopeDirection(newVelocity)* DEFAULT_SLOPE_MULTIPLE;
        }
        else
        {
            rockRigidbody.velocity += newVelocity;
        }

        if (groundValocity.magnitude > maxSpeed)
        {
            rockRigidbody.velocity = groundValocity.normalized * maxSpeed + Vector3.up * rockRigidbody.velocity.y;
        }
        else if(isSlope && rockRigidbody.velocity.magnitude>maxSpeed)
        {
            rockRigidbody.velocity = rockRigidbody.velocity.normalized * maxSpeed;
        }

    }
    protected virtual void Jump(float power)
    {
        rockRigidbody.velocity += Vector3.up * power;
    }

    [Obsolete]
    protected virtual bool IsGround()
    {
        float rockHeightHalf = rockObject.gameObject.GetHeight(.5f);
        Collider[] colliders = Physics.OverlapSphere(rockObject.position - Vector3.up * rockHeightHalf, .05f, Global_PSC.FindLayerToName("Terrains"));

        if (colliders.Length > 0)
        {
            return true;
        }
        return false;
    }

    protected virtual bool CheckGround()
    {
        isGround = false;
        float rockHeightHalf = rockObject.gameObject.GetHeight(.5f);
        
        Collider[] colliders = Physics.OverlapSphere(rockObject.position - Vector3.up * rockHeightHalf, .05f, Global_PSC.FindLayerToName("Terrains"));
        if (colliders.Length > 0)
        {
            isGround = true;
        }

        return isGround;
    }
    protected virtual bool CheckGroundRay()
    {
        isGround = false;
        float rockHeightHalf = rockObject.gameObject.GetHeight(.5f);

        if (Physics.Raycast(rockObject.position, Vector3.down, out slopeHit, rockHeightHalf + rockHeightHalf * .75f, Global_PSC.FindLayerToName("Terrains")))
        {
            isGround = true;
        }

        return isSlope;
    }

    RaycastHit slopeHit;
    protected virtual bool CheckSlope()
    {
        isSlope = false;
        float rockHeightHalf = rockObject.gameObject.GetHeight(.5f);

        if (Physics.Raycast(rockObject.position, Vector3.down, out slopeHit, rockHeightHalf + rockHeightHalf * .75f, Global_PSC.FindLayerToName("Terrains")))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            if (angle != 0 && angle <= SLOPE_LIMIT_MAX)
            {
                isSlope = true;
            }
        }

        return isSlope;
    }

    protected Vector3 GetSlopeDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal);
    }

    protected virtual void Attack(Collision collision)
    {
        IHitObjectHandler hitObject = collision.gameObject.GetComponentInParent<IHitObjectHandler>();
        if (hitObject != null)
        {
            return;
        }

        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.otherCollider.gameObject == gameObject)
            {
                // 충돌 지점의 법선 벡터와 gameobject의 진행 방향을 계산합니다.
                Vector3 collisionNormal = contact.normal;
                Vector3 forwardDirection = rockRigidbody.velocity.normalized;

                // 두 벡터의 각도를 계산합니다.
                float angle = Vector3.Angle(collisionNormal, forwardDirection);

                // 일정 각도 이내의 충돌을 확인합니다.
                float maxCollisionAngle = COLLISION_ALLOW_ANGLE; // 예시: 45도 이내의 충돌을 확인
                if (angle <= maxCollisionAngle)
                {
                    hitObject.Hit((int)GetDamageValue());
                    break;
                }
                else
                {
                    continue;
                }
            }

        }
    }


    protected virtual float Attack()
    {
        return 0;
    }

    protected virtual void Fall()
    {

    }
    protected virtual void BackCheckPoint()
    {

    }

    public void Hit(int damage)
    {
        HitReaction();
        currHp -= damage;
        if (currHp < 0)
        {
            Die();
        }
        else
        {
            ChangeForm(currHp);
        }
    
    }

    

    public void SetCheckPoint()
    {

    }

    protected virtual void ChangeDrag()
    {

        if (isGround)
        {
            rockRigidbody.drag = DEFAULT_GROUND_DRAG;
        }
        else
        {
            rockRigidbody.drag = 0;
        }
    }

    protected virtual void ChangeForm(float hp)
    {
        if (forms==null || forms.Count<1)
        {
            return;
            
        }
        float changeRate = rockStatus.Health / forms.Count;
        int formIndex = (int)(currHp / changeRate);
        
    }

    public void HitReaction(){}

    protected virtual void Die(){}

    protected Vector2 GetInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        playerInput = new Vector2(horizontalInput, verticalInput);
        return playerInput;
    }

    protected bool IsMove()
    {
        return rockRigidbody.velocity.magnitude > 0;
    }
    protected bool IsMove(float speed)
    {
        return rockRigidbody.velocity.magnitude >= speed;
    }

    //공격력 * 0.5 * 최대체력 대비 현재체력 +공격력 * 0.5 * 최대속도 대비 현재속도
    //예외 공격력 50이하 일 경우 50
    protected float GetDamageValue()
    {
        float maxDamage = rockStatus.Damage;
        float resultDamage = 0;

        float healthRate = currHp / rockStatus.Health;
        float speedRate = rockRigidbody.velocity.magnitude / rockStatus.Speed;

        resultDamage += maxDamage * .5f * healthRate;
        resultDamage += maxDamage * .5f * speedRate;

        if (resultDamage < DAMAGE_LIMIT_MIN)
        {
            resultDamage = DAMAGE_LIMIT_MIN;
        }

        return resultDamage;
    }

    protected RockState rockState;
    public enum RockState
    {
        MOVE,
        JUMP
    }

    protected void ChangeRockState()
    {
        if (isGround)
        {
            rockState = RockState.MOVE;
        }

        else if (!isGround)
        {
            rockState = RockState.JUMP;
        }
    }
}

