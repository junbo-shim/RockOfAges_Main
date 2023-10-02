using Cinemachine;
using Photon.Pun;
using RayFire;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public class RockBase : MonoBehaviourPun, IHitObjectHandler
{
    //돌 하위 오브젝트들
    [SerializeField]
    protected Transform rockObject;
    [SerializeField]
    protected Transform checkPoint;
    [SerializeField]
    public RockStatus rockStatus;
    [SerializeField]
    public FallText fallText;
    [SerializeField]
    protected RockTrail trail;
    [SerializeField]
    protected List<FormContainer> forms;

    //사용자 입력(Y축 제외)
    protected Vector2 playerInput;

    //기본 컴포넌트
    protected Camera mainCamera;
    //protected CinemachineFreeLook lookCamera;
    protected Rigidbody rockRigidbody;
    protected MeshRenderer rockRenderer;
    protected MeshFilter rockMesh;
    protected MeshCollider rockCollider;

    protected float currHp;
    protected float rockHeightHalf;
    protected float obstacleMultiple = 1;

    public bool isGround = false;
    protected bool isSlope = false;
    protected bool isFall = false;
    public bool isDestroy = false;
    //{ 0920 홍한범
    // 디버프 체크
    public bool isDebuffed = false;
    // 점프포스 감소
    public float debuffJumpForce;
    //} 0920 홍한범

    public int teamNumber= -1;

    RaycastHit slopeHit;
    Coroutine fallCheckCoroutine = null;
    protected Queue<RockTrail> trails;

    protected RayfireRigid rayfireRigid;

    protected AudioSource rockAudio;

    public AudioClip destroyAudio;
    public AudioClip collisionAudio;
    public AudioClip fallAudio;
    public AudioClip jumpAudio;
    public AudioClip rollAudio;

    protected int currLayer;


    protected const float DAMAGE_LIMIT_MIN = 50f;
    protected const float SLOPE_LIMIT_MAX = 60f;
    protected const float COLLISION_ALLOW_ANGLE = 45f;
    protected const float DEFAULT_JUMP_FORCE = 5f;
    protected const float DEFAULT_GROUND_DRAG = .1f;
    protected const float DEFAULT_AIR_MULTIPLE = .3f;
    protected const float DEFAULT_SLOPE_MULTIPLE = 1.5f;
    protected const float DEFAULT_OBSTACLE_MULTIPLE = 1f;

    // ! Photon
    public PhotonView dataContainerView;


    private void OnDrawGizmos()
    {
        if (rockObject != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(rockObject.position - Vector3.up * rockObject.gameObject.GetHeight(.05f), .05f);
        }
    } //사랑해요 성철이형 -재현-
      //GOOD

    #region if thing goes wrong unlock this
    // ! Photon
    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) 
    //{
    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext(currHp);
    //    }
    //    else
    //    {
    //        currHp = (float)stream.ReceiveNext();
    //    }
    //}
    #endregion

    //{ 0920 홍한범
    // 속도에 대한 저항값, 질량에 대한 저항값을 적용할 수 있음
    // 감소수치를 넣고 SetObstacleMultiple를 부르고 ResetDebuff를 부르면 풀림
    // 모든값은 퍼센트로 줄임 Ex) 2배 2f, 0.5배 0.5f;
    public void SetObstacleMultiple(float velocityValue, float massValue, float jumpValue)
    {
        if (isDebuffed == false)
        {
            obstacleMultiple *= velocityValue;
            rockRigidbody.mass = rockStatus.Weight * massValue;
            debuffJumpForce = jumpValue;
            isDebuffed = true;
        }
    }

    public void ResetDebuff()
    {
        if (isDebuffed == true)
        {
            obstacleMultiple = 1f;
            rockRigidbody.mass = rockStatus.Weight;
            debuffJumpForce = 1f;
            isDebuffed = false;
        }
    }
    //} 0920 홍한범

    public virtual void Init()
    {
        // ! PSC Editted
        dataContainerView = NetworkManager.Instance.myDataContainer.GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            //추후 시네머신 카메라로 바꿀것
            mainCamera = Camera.main;
        }
        else
        {
            //카메라 큐 추가할 공간
        }

        rockObject = transform.Find("RockObject");

        rockRigidbody = rockObject.GetComponent<Rigidbody>();
        rockMesh = rockObject.GetComponent<MeshFilter>();
        rockCollider = rockObject.GetComponent<MeshCollider>();
        rockRenderer = rockObject.GetComponent<MeshRenderer>();
        rockAudio = rockObject.GetComponent<AudioSource>();
        fallText = GetComponentInChildren<FallText>();

        rockStatus = Instantiate(rockStatus, rockObject);

        rockHeightHalf = rockObject.gameObject.GetHeight(.1f)*.5f;
        rockRigidbody.mass = rockStatus.Weight *0.2f;
        currHp = rockStatus.Health;
        //{ 0920 홍한범
        debuffJumpForce = 1f;
        //} 0920 홍한범



        rayfireRigid = rockObject.GetComponent<RayfireRigid>();
        trails = new Queue<RockTrail>();

        CreateTrail();
        if (UIManager.uiManager != null)
        {
            UIManager.uiManager.PrintFillAmountRockHp(currHp, rockStatus.Health);
        }

        // PSC Editted
        Vector3 startPos = Vector3.zero;
        if(teamNumber == 1)
        {
            startPos = ResourceManager.Instance.team1StartPoint;
        }
        else if(teamNumber == 2)
        {
            startPos = ResourceManager.Instance.team2StartPoint;
        }

        transform.localScale = Vector3.one * 0.1f;
        gameObject.SetChildPosition(startPos, "RockObject");
        SetCheckPoint(startPos);

        // ! Photon
        if (photonView.IsMine == false)
        {
            CycleManager.cycleManager.CheckTeamAndSaveQueue(dataContainerView.ViewID.ToString(), gameObject);
            //UIManager.uiManager.RotateMirror();
        }
    }

    //혹시 모를 오버로딩
    protected virtual void Move()
    {
        playerInput = GetInput();
        Move(playerInput);
    }
    protected virtual void Jump()
    {
        Jump(DEFAULT_JUMP_FORCE);
    }


    //이동
    protected virtual void Move(Vector2 input)
    {
        float accel = rockStatus.Acceleration * obstacleMultiple*0.5f;
        float maxSpeed = rockStatus.Speed * obstacleMultiple;

        //카메라를 기준으로 좌표 변경
        Vector3 inputDirection = (Camera.main.transform.forward * input.y + Camera.main.transform.right * input.x).normalized;
        //y축 고려 x
        Vector3 groundValocity = new Vector3(rockRigidbody.velocity.x, 0f, rockRigidbody.velocity.z);

        //이동방향으로 
        inputDirection *= accel * Time.deltaTime;
        groundValocity += inputDirection;

        //속도 변화
        Vector3 newVelocity = (new Vector3(inputDirection.x, 0, inputDirection.z));
        //점프
        if (!isGround)
        {
            rockRigidbody.velocity += newVelocity * DEFAULT_AIR_MULTIPLE;
        }
        //경사
        else if (CheckSlope())
        {
            rockRigidbody.velocity += GetSlopeDirection(newVelocity)* DEFAULT_SLOPE_MULTIPLE;
        }
        //땅
        else
        {
            rockRigidbody.velocity += newVelocity;
        }


        //속도 제한
        //땅
        if (groundValocity.magnitude > maxSpeed)
        {
            rockRigidbody.velocity = groundValocity.normalized * maxSpeed + Vector3.up * rockRigidbody.velocity.y;
        }
        //경사
       /* else if(isSlope && rockRigidbody.velocity.magnitude>maxSpeed)
        {
            rockRigidbody.velocity = rockRigidbody.velocity.normalized * maxSpeed;
        }*/

        //Debug.Log(rockRigidbody.velocity.magnitude);
    }

    //점프
    protected virtual void Jump(float power)
    {
        PlayJumpSound(true);
        rockRigidbody.velocity += Vector3.up * (power*debuffJumpForce);
        CreateTrail();
    }


    //레거시 
    [Obsolete]
    protected virtual bool IsGround()
    {
        Collider[] colliders = Physics.OverlapSphere(rockObject.position - Vector3.up * rockHeightHalf, .05f, Global_PSC.FindLayerToName("Terrains")+ Global_PSC.FindLayerToName("Walls"));

        if (colliders.Length > 0)
        {
            foreach (Collider collider in colliders)
            {
                currLayer = collider.gameObject.layer;
                if (currLayer == LayerMask.NameToLayer("Terrains"))
                {
                    break;
                }
            }
            return true;
        }
        currLayer = 0;
        return false;
    }


    //오버랩을 사용하는 checkGround
    protected virtual bool CheckGround()
    {
        bool result;

        if (isSlope)
        {
            result = CheckGroundRay();
        }
        else
        {
            result = CheckGroundOverlap();
        }
        // 0925 홍한범 조건추가
        if (!isGround && result)
        {

            StartCoroutine(CameraShakeRoutine(.1f, 3, 3));
            PlayCollisionSound(true);
        }
        isGround = result;

        return result;

    }
    //오버랩을 사용하는 checkGround
    protected virtual bool CheckGroundOverlap()
    {

        Collider[] colliders = Physics.OverlapSphere(rockObject.position - Vector3.up * rockHeightHalf, .05f, Global_PSC.FindLayerToName("Terrains") + Global_PSC.FindLayerToName("Walls"));
        if (colliders.Length > 0)
        {
            foreach (Collider collider in colliders)
            {
                currLayer = collider.gameObject.layer;
                if (currLayer == LayerMask.NameToLayer("Terrains"))
                {
                    break;
                }
            }
            return true;
        }

        currLayer = 0;
        return false;
    }

    //레이를 사용하는 checkGround
    //경사 구조때문에 해당 메서드 사용 권장
    protected virtual bool CheckGroundRay()
    {
        if (Physics.Raycast(rockObject.position, Vector3.down, out slopeHit, rockHeightHalf + rockHeightHalf * .75f, Global_PSC.FindLayerToName("Terrains") + Global_PSC.FindLayerToName("Walls")))
        {
            currLayer = slopeHit.collider.gameObject.layer;
            return  true;
        }
        currLayer = 0;
        return false;
    }

    //경사로 체크
    //레이 사용
    protected virtual bool CheckSlope()
    {
        isSlope = false;
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

    //떨어짐 체크
    //레이 사용
    //바닥이 없는 상황에서 계산 시작
    protected virtual void CheckFall()
    {
        // ! PSC Editted
        if (photonView.IsMine)
        {
            if (!Physics.Raycast(rockObject.position, Vector3.down, 1000, Global_PSC.FindLayerToName("Terrains")) && fallCheckCoroutine == null)
            {
                StartFallingCheck();
            }
        }
        
    }

    
    //일정시간동안 검사하기 위해서 coroutine으로 검사 시작.
    protected virtual void StartFallingCheck()
    {
        if (fallCheckCoroutine != null)
        {
            StopCoroutine(fallCheckCoroutine);
            fallCheckCoroutine = null;
        }

        fallCheckCoroutine = StartCoroutine(FallingCheckRoutine(2));
        
    }

    //일정 시간동안 바닥이 없을경우
    // checkpoint로 back시킴
    //이때 떨어지는화면->손->실제 플레이 순으로 진행되기 때문에 coroutine으로 구현
    IEnumerator FallingCheckRoutine(float needTime)
    {
        float time = 0;
        while (time < needTime)
        {
            if (isGround && currLayer == LayerMask.NameToLayer("Terrains"))
            {
                isFall = false;
                fallCheckCoroutine = null;
                yield break;
            }
            yield return Time.deltaTime;
            time += Time.deltaTime;
        }

        if (!isFall)
        {
            isFall = true;
            StartCoroutine(ComebackCheckPointRoutine());

        }
    }


    protected IEnumerator ComebackCheckPointRoutine()
    {
        //카메라 정지
        //떨어지면서 메아리 추가

        Fall();
        yield return new WaitForSeconds(2f);

        fallText.ClearText();

        if (rockObject != null)
        {
            //되돌리기
            BackCheckPoint();
        }

        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("PscTestScene"))
        {
            // 손
            GodHand godHand = FindObjectOfType<GodHand>();
            yield return new WaitForSeconds(0.4f);
            godHand.StandBy(this.gameObject);
            yield return new WaitForSeconds(0.3f);
            godHand.FollowRock(this.gameObject);

        }

    }

    //경사에 있을 경우 힘의 방향을 해당 경사에 맞게 회전시킴
    protected Vector3 GetSlopeDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal);
    }


    //공격
    //hitobject가 있을 경우 해당 오브젝트를 공격한다.
    //속도가 있어야하고
    //징행 방향과 비슷한 방향이어야 공격이라 판단하며
    //공격때 자신의 체력도 닳게한다.
    public virtual void Attack(Collision collision)
    {

        IHitObjectHandler hitObject = collision.gameObject.GetComponentInParent<IHitObjectHandler>();
        if (hitObject == null)
        {
            hitObject = collision.gameObject.GetComponent<IHitObjectHandler>();
            if (hitObject == null)
            {
                return;
            }
        }

        Debug.Log(hitObject);
        PlayCollisionSound(true);

        foreach (ContactPoint contact in collision.contacts)
        {
            //Debug.Log(contact.thisCollider.transform.parent.gameObject + "/"+ gameObject);
            if (contact.thisCollider.transform.parent.gameObject == gameObject)
            {
                // ! Photon
                hitObject.Hit((int)GetDamageValue());
                break;
                #region Legacy
                /* // 충돌 지점의 법선 벡터와 gameobject의 진행 방향을 계산합니다.
                 Vector3 collisionNormal = contact.normal;
                 Vector3 forwardDirection = rockRigidbody.velocity.normalized;

                 // 두 벡터의 각도를 계산합니다.
                 float angle = Vector3.Angle(collisionNormal, forwardDirection);

                 // 일정 각도 이내의 충돌을 확인합니다.
                 float maxCollisionAngle = COLLISION_ALLOW_ANGLE; // 예시: 45도 이내의 충돌을 확인
                 Debug.Log(angle + "/" + maxCollisionAngle);
                */
                /* if (angle <= maxCollisionAngle)
                 {
                     hitObject.Hit((int)GetDamageValue());
                     break;
                 }
                 else
                 {
                     continue;
                 }*/
                #endregion
            }
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Castle"))
        {
            // ! Photon
            // 안전장치 if 문
            if (dataContainerView.IsMine == true)
            {
                // enemyCameraQueue 를 확인하고 Dequeue 하는 메서드를 다른 ViewID 에게 발사
                photonView.RPC("CheckTeamAndDequeue", RpcTarget.Others, 
                    dataContainerView.ViewID.ToString(), photonView.ViewID.ToString());
            }

            StartCoroutine(EndAttackRoutine());    
        }
    }

    public IEnumerator CameraShakeRoutine(float time, float AmplitudeGain, float FrequencyGain)
    {
        mainCamera.ShakeFreeLookCamera(AmplitudeGain, FrequencyGain);
        yield return new WaitForSeconds(time);
        mainCamera.ShakeFreeLookCamera(0, 0);
    }

    public void CreateTrail()
    {
        if (trails.Count > 4)
        {
            GameObject trailObject = trails.Dequeue().gameObject;
            if (trailObject != null)
            {
                Destroy(trailObject);
            }
        }

        trails.Enqueue(Instantiate(trail, transform.position, Quaternion.Euler(90,0,0), transform));
    }


    [Obsolete]
    protected virtual float Attack()
    {
        return 0;
    }

    protected virtual void Fall()
    {
        if(CycleManager.cycleManager == null || CycleManager.cycleManager.userState == (int)UserState.ATTACK)
        {
            Hit(300);
            PlayFallSound(true);
            CinemachineVirtualCameraBase camera = mainCamera.GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCameraBase;
            camera.Follow = null;   
            fallText.StartFallText();     
        }
    }
    protected virtual void BackCheckPoint()
    {
        if (CycleManager.cycleManager == null || CycleManager.cycleManager.userState == (int)UserState.ATTACK)
        {
            CinemachineVirtualCameraBase camera = mainCamera.GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCameraBase;
            camera.Follow = rockObject;
            fallCheckCoroutine = null;
            isFall = false;
            rockRigidbody.velocity = Vector3.zero;
            rockRigidbody.angularVelocity = Vector3.zero;
            rockObject.rotation = Quaternion.identity;
            rockObject.position = checkPoint.position + Vector3.up * 10f;

            camera.ForceCameraPosition(checkPoint.position, checkPoint.rotation);        
        }
    }

    //맞았을 경우 체력마다 다른 mesh를 보여준다.
    public void Hit(int damage)
    {
        currHp -= damage;
        HitReaction();
        //Debug.Log(currHp);
        if (currHp <= 0)
        {
            Die();
        }
        else
        {
            ChangeForm(currHp);
        }
    
    }

    

    public void SetCheckPoint(Vector3 position)
    {
        checkPoint = transform.Find("CheckPoint");
        checkPoint.position = position;
    }

    protected virtual void ChangeDrag()
    {

        if (isGround)
        {
            rockRigidbody.drag = DEFAULT_GROUND_DRAG;
        }
        else
        {
            rockRigidbody.drag = .01f/obstacleMultiple;
        }
    }

    protected virtual void ChangeForm(float hp)
    {
        if (forms==null || forms.Count<1)
        {
            return;
        }

        PlayDestroySound(true);
        float changeRate = rockStatus.Health / forms.Count;
        int formIndex = (int)(currHp / changeRate);
        rockMesh.sharedMesh = forms[formIndex].mesh;
        rockRenderer.materials = forms[formIndex].material;

    }

    public void HitReaction()
    {
        float maxHp = rockStatus.Health;
        if (UIManager.uiManager != null)
        {
            //{ 0925 홍한범
            UIManager.uiManager.PrintFillAmountRockHp(currHp, maxHp);
            //} 0925 홍한범
        }
    }

    protected virtual void Die()
    {
        // ! Photon
        // 안전장치 if 문
        if (dataContainerView.IsMine == true)
        {
            // enemyCameraQueue 를 확인하고 Dequeue 하는 메서드를 다른 ViewID 에게 발사
            photonView.RPC("CheckTeamAndDequeue", RpcTarget.Others, 
                dataContainerView.ViewID.ToString(), photonView.ViewID.ToString());
        }

        PlayDestroySound(true);
        rayfireRigid.Demolish();
        //rayfireRigid.Activate();
        StartCoroutine(EndAttackRoutine());
        //Destroy(gameObject);
    }

    IEnumerator EndAttackRoutine()
    {
        isDestroy = true;
        yield return new WaitForSeconds(2f);
        CycleManager.cycleManager.ChangeCycleAttackToSelect();
        yield return new WaitForSeconds(1f);
        PhotonNetwork.Destroy(gameObject);
    }

    // ! Photon
    // CycleManager 의 CheckTeamAndSaveQueue 메서드와 세트 (매개변수로 송신자 ViewID 와 송신자의 돌 ViewID 를 전달)
    [PunRPC]
    public void CheckTeamAndDequeue(string senderViewID, string senderRockViewID) 
    {
        // RPC 를 보낸 View ID 에서 Team 번호만 추출한다.
        string senderTeamNumber = PhotonNetwork.CurrentRoom.CustomProperties[senderViewID].ToString().Split('_')[1];
        // RPC 를 수신한 View ID 에서 Team 번호만 추출한다.
        string myTeamNumber = PhotonNetwork.CurrentRoom.CustomProperties[dataContainerView.ViewID.ToString()].ToString().Split('_')[1];

        // 만약 송신자와 수신자가 같은 Team 이라면 아무것도 하지 않는다.
        if (senderTeamNumber == myTeamNumber) 
        {
            /*Do Nothing*/
        }
        // 만약 송신자와 수신자가 다른 Team 이라면
        else if (senderTeamNumber != myTeamNumber) 
        {
            // Dequeue 는 Queue 의 크기가 0 이 아닐 때만 작동하게 한다
            if (CameraManager.Instance.enemyCameraQueue.Count != 0) 
            {
                // Queue 에 접근하여 첫번째 돌을 지역변수에 저장한다
                GameObject firstRockInQueue = CameraManager.Instance.enemyCameraQueue.Peek();

                // Queue 의 첫번째 돌의 View ID 와 RPC 를 보낸 돌의 View ID 를 비교한다
                // 만약 지금 죽은 돌이 Queue 에서 첫번째가 아니라면 
                if (firstRockInQueue.GetComponent<PhotonView>().ViewID.ToString() 
                    != senderRockViewID) 
                {
                    // Dequeue 만 한다
                    CameraManager.Instance.enemyCameraQueue.Dequeue();
                }
                // 만약 지금 죽은 돌이 Queue 에서 첫번째라면
                else if (firstRockInQueue.GetComponent<PhotonView>().ViewID.ToString()
                    == senderRockViewID)
                {
                    // Dequeue 하고 그 다음 돌에 카메라를 놓는다
                    CameraManager.Instance.enemyCameraQueue.Dequeue();
                    CameraManager.Instance.SetEnemyCamera(CameraManager.Instance.enemyCameraQueue.Peek());
                }
                //Debug.Log(CameraManager.Instance.enemyCameraQueue.Count);
                //Debug.Log(CameraManager.Instance.enemyCameraQueue.Peek().GetComponent<PhotonView>().ViewID.ToString());            
            }
        }
    }

    protected Vector2 GetInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        playerInput = new Vector2(horizontalInput, verticalInput);
        return playerInput;
    }


    protected virtual void PlaySound(AudioClip clip)
    {
        if (!rockAudio.isPlaying)
        {
            rockAudio.clip = clip;
            rockAudio.Play();
        }

    }
    protected virtual void PlayForceSound(AudioClip clip)
    {
        rockAudio.Stop();
        rockAudio.clip = clip;
        rockAudio.Play();

    }

    protected virtual void PlayMoveSound()
    {
        float magnitude = new Vector3(rockRigidbody.velocity.x, 0, rockRigidbody.velocity.z).magnitude;

        if (isGround && magnitude > 0.1f)
        {
            if (!rockAudio.isPlaying)
            {
                rockAudio.clip = rollAudio;
                rockAudio.Play();
            }
        }
        else
        {
            if (rockAudio.clip == rollAudio)
            {
                rockAudio.Stop();
            }
        }
    }
    protected virtual void PlayJumpSound(bool isForce)
    {
        if (isFall)
        {
            return;
        }

        if (isForce)
        {
            PlayForceSound(jumpAudio);
        }
        else
        {
            PlaySound(jumpAudio);
        }
    }
    protected virtual void PlayCollisionSound(bool isForce)
    {
        if (isFall)
        {
            return;
        }

        if (isForce)
        {
            PlayForceSound(collisionAudio);
        }

        else
        {
            PlaySound(collisionAudio);
        }
    }
    protected virtual void PlayDestroySound(bool isForce)
    {
        if (isFall&&!isDestroy)
        {
            return;
        }

        if (isForce)
        {
            PlayForceSound(destroyAudio);
        }
        else
        {
            PlaySound(destroyAudio);
        }
    }
    protected virtual void PlayFallSound(bool isForce)
    {
        if (isDestroy)
        {
            return;
        }
        if (isForce)
        {
            PlayForceSound(fallAudio);
        }
        else
        {
            PlaySound(fallAudio);
        }
    }

    public float NowSpeed()
    {
        return rockRigidbody.velocity.magnitude;

    }

    public bool IsMove()
    {
        return rockRigidbody.velocity.magnitude > 0;
    }
    public bool IsMove(float speed)
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
        Debug.Log("체력비례 " + maxDamage * .5f * healthRate);
        resultDamage += maxDamage * .5f * speedRate;
        Debug.Log("속도비례 " + maxDamage * .5f * speedRate);

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

