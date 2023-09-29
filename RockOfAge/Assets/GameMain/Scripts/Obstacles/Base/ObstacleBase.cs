using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class ObstacleBase : MonoBehaviour
{
    //obstacle의 베이스

    //스테이터스
    public ObstacleStatus status;

    //건설시 position
    public float buildPositionY;

    //기본 데이터
    protected MeshFilter obstacleMeshFilter;
    protected Rigidbody obstacleRigidBody;
    protected Animator obstacleAnimator;
    protected Renderer obstacleRenderer;
    protected Collider obstacleCollider;
    protected Material originMaterial;
    //타겟
    protected GameObject target;

    //사람
    public PeopleObject peopleObject;
    public int peopleCount = 5;

    //현재체력
    protected float currHealth;

    //건설완성=오브젝트활성화
    protected bool isBuildComplete = false;
    
    [SerializeField]
    public bool dragObstacle = false;

    public static readonly float BUILD_TIME = 5f;


    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        //화면상에 보여질때 처음에는 흰색으로 시작.
        StartBuild(BUILD_TIME);
    }

    void MakePeople()
    {

        if (peopleObject == null)
        {
            return;
        }

        int numberOfPeople = Random.Range(3, peopleCount);
        Vector3 targetPosition = transform.position;
        for (int i = 0; i < numberOfPeople; i++)
        {
            //랜덤한 위치에 벡터 생성
            Vector3 randomOffset = Random.insideUnitSphere;
            randomOffset.y = 1f;
            randomOffset.x *= Random.Range(1f, 2f);
            randomOffset.z *= Random.Range(1f, 2f);


            //랜덤한 위치에 사람 생성
            Vector3 peoplePosition = targetPosition + randomOffset;
            PeopleObject PeopleInstance = Instantiate(peopleObject, peoplePosition, Quaternion.identity);
        }
    }

    //제일 하단 스크립트에서 해당 함수를 불러온다(ONENABLE)
    protected void StartBuild(float time)
    {
        //마테리얼 교체
        originMaterial = obstacleRenderer.material;
        obstacleRenderer.material = BuildManager.instance.white;

        if (obstacleCollider != null)
        {
            if(obstacleCollider is MeshCollider)
            {
                (obstacleCollider as MeshCollider).convex = true;
            }
            obstacleCollider.isTrigger = true;
        }

        StartCoroutine(BuildRoutine(time));
    }

    //일정 시간동안 대기하는 COROUTINE
    protected IEnumerator BuildRoutine(float buildTime)
    {
        float currTime = 0;
        while (currTime < buildTime)
        {
            yield return Time.deltaTime;
            currTime += Time.deltaTime;
        }

        if (gameObject == null)
            yield break;

        isBuildComplete = true;
        obstacleRenderer.material = originMaterial;

        if (obstacleCollider != null)
        {
            obstacleCollider.isTrigger = false;
            if (obstacleCollider is MeshCollider)
            {
                (obstacleCollider as MeshCollider).convex = false;
            }
        }
        MakePeople();
    }

    bool isTest = false;
    
    //맵에 Build
    public virtual ObstacleBase Build(Vector3 position, Quaternion rotate, int currIndex, int count)
    {
        // ! Photon
        //오브젝트 생성
        string ObstacleName = this.gameObject.name;

        ObstacleBase obstacle = PhotonNetwork.Instantiate(ObstacleName, position, rotate).GetComponent<ObstacleBase>();
        
        //스케일 변경
        obstacle.transform.localScale = obstacle.transform.localScale;

        //if (!isTest) 
        { 
            //버튼 데이터 변경
            GameObject unitButton = ResourceManager.Instance.FindUnitGameObjById(status.Id);
            unitButton.GetComponent<CreateButton>().buildCount += 1;
            UIManager.uiManager.RePrintUnitCount(status.Id);
        }
        return obstacle;
    }

    //초기화
    protected virtual void Init()
    {
        status = new ObstacleStatus(status);// Instantiate(status);
        obstacleMeshFilter = GetComponent<MeshFilter>();
        obstacleRigidBody = GetComponent<Rigidbody>();
        obstacleAnimator = GetComponent<Animator>();
        obstacleRenderer = GetComponentInChildren<Renderer>();
        obstacleCollider = GetComponentInChildren<MeshCollider>();
        currHealth = status.Health;
    }
    public virtual void Delete()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    //타겟 서치
    //아마 상위에 없어도 될거로 추정됨
    protected virtual void SearchTarget() { }

    //죽음
    protected virtual void Dead() 
    {
        // ! Photon
        // PhotonNetwork.Destroy 들어가기 전에 각 개체의 건설 갯수를 감소시킨다
        GameObject unitButton = ResourceManager.Instance.FindUnitGameObjById(status.Id);
        unitButton.GetComponent<CreateButton>().buildCount -= 1;
        UIManager.uiManager.RePrintUnitCount(status.Id);
        // PhotonNetwork.Destroy 로 모든 클라이언트에서 파괴한다
        PhotonNetwork.Destroy(gameObject);
    }


    //공격 활성화
    protected virtual void ActiveAttack() { }
    protected virtual void DeactiveAttack() { }

}