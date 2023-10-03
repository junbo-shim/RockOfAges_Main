using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class ObstacleBase : MonoBehaviourPun
{
    //obstacle의 베이스

    public Transform obstacleParent;

    //스테이터스
    public ObstacleStatus status;

    //건설시 position
    public float buildPositionY;

    //기본 데이터
    protected MeshFilter obstacleMeshFilter;
    protected Rigidbody obstacleRigidBody;
    protected Animator obstacleAnimator;
    //protected Renderer obstacleRenderer;
    protected Collider obstacleCollider;
    protected AudioSource audioSource;
    protected Material[] originMaterial;
    protected Renderer[] obstacleRenderers;
    protected Queue<Material[]> originMaterials;
    protected Queue<Material[]> dissolveMaterials;
    //타겟
    protected GameObject target;

    //사람
    public PeopleObject peopleObject;
    public int peopleCount = 5;

    //현재체력
    protected float currHealth;

    //건설완성=오브젝트활성화
    public bool isBuildComplete = false;
    
    [SerializeField]
    public bool dragObstacle = false;

    public static readonly float BUILD_TIME = 5f;

    private Material dissolveMaterial = null;
    private int _Width = 0;
    private int _CutOff = 0;

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        //화면상에 보여질때 처음에는 흰색으로 시작.
        StartBuild(BUILD_TIME);
    }

    protected void MakePeople()
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
    protected virtual void StartBuild(float time)
    {

        if (obstacleRenderers.Count() == 0)
        {
            return;
        }

        if (BuildManager.instance == null)
        {
            return;
        }

        //자식에 있는 모든 renderer 처리
        foreach (var renderer in obstacleRenderers)
        {
            //마테리얼 교체
            originMaterial = renderer.materials;
            originMaterials.Enqueue(renderer.materials);

            //디졸브될 마테리얼 생성
            Material[] subMaterial = new Material[originMaterial.Length];

            //각 배열에 들어갈 마테리얼 초기화
            _Width = Shader.PropertyToID("_Width");
            _CutOff = Shader.PropertyToID("_CutOff");

            for (int i = 0; i < subMaterial.Length; i++)
            {
                //dissolve용 material 추가
                subMaterial[i] = Instantiate( BuildManager.instance.white, transform);

                //dissolve용 material 초기화
                if (subMaterial[i] != null)
                {
                    subMaterial[i].SetFloat(_CutOff, 0);
                    subMaterial[i].SetFloat(_Width, 0);
                }
            }

            //현재 renderer에 해당하는 dissolve마테리얼 저장
            dissolveMaterials.Enqueue(subMaterial);

            //마테리얼 교체
            renderer.materials = subMaterial;

        }

        if (obstacleRigidBody != null)
        {
            obstacleRigidBody.useGravity = false;
        }
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
    protected virtual IEnumerator BuildRoutine(float buildTime)
    {
        float currTime = 0;
        while (currTime < buildTime)
        {
            yield return Time.deltaTime;
            currTime += Time.deltaTime;
            foreach(var dissolveMaterialArray in dissolveMaterials)
            {
                foreach(var dissolveMaterial in dissolveMaterialArray)
                {
                    dissolveMaterial.SetFloat(_CutOff, currTime / buildTime * 0.5f);
                    dissolveMaterial.SetFloat(_Width, currTime / buildTime * 0.5f);
                }
            }
        }

        foreach (var dissolveMaterialArray in dissolveMaterials)
        {
            foreach (var dissolveMaterial in dissolveMaterialArray)
            {
                dissolveMaterial.SetFloat(_CutOff, 1);
                dissolveMaterial.SetFloat(_Width, 1);
            }
        }

        if (gameObject == null)
            yield break;


        foreach (var renderer in obstacleRenderers)
        {
            //마테리얼 교체
            renderer.materials = originMaterials.Dequeue();
        }

        //더이상 쓰지않는 저장공간 참조 제거
        originMaterial = null;
        originMaterials = null;
        dissolveMaterials = null;

        isBuildComplete = true;

        if (obstacleCollider != null)
        {
            obstacleCollider.isTrigger = false;
            if (obstacleCollider is MeshCollider)
            {
                (obstacleCollider as MeshCollider).convex = false;
            }
        }
        if (obstacleRigidBody != null)
        {
            obstacleRigidBody.useGravity = true;
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

        //ObstacleBase obstacle = Instantiate(this, position, rotate);
        ObstacleBase obstacle = PhotonNetwork.Instantiate(ObstacleName, position, rotate).GetComponent<ObstacleBase>();
        
        //스케일 변경
        obstacle.transform.localScale = obstacle.transform.localScale;

        
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("PscTestScene"))
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
        status =  Instantiate(status, transform);
        obstacleMeshFilter = GetComponent<MeshFilter>();
        obstacleRigidBody = GetComponent<Rigidbody>();
        obstacleAnimator = GetComponent<Animator>();
        //obstacleRenderer = GetComponentInChildren<Renderer>();
        obstacleCollider = GetComponentInChildren<MeshCollider>();
        obstacleRenderers = GetComponentsInChildren<Renderer>();
        audioSource = GetComponent<AudioSource>();
        originMaterials = new Queue<Material[]>();
        dissolveMaterials = new Queue<Material[]>();
        currHealth = status.Health;
    }
    public virtual void Delete()
    {
        photonView.RPC("RemoveInMaster", RpcTarget.All);
    }

    //타겟 서치
    //아마 상위에 없어도 될거로 추정됨
    protected virtual void SearchTarget() { }

    //죽음
    protected virtual void Dead() { }


    //공격 활성화
    protected virtual void ActiveAttack() { }
    protected virtual void DeactiveAttack() { }

    [PunRPC]
    public void PlayAnimationTrigger(string valueName)
    {
        obstacleAnimator.SetTrigger(valueName);
    }

    [PunRPC]
    public void RemoveInMaster()
    {
        // 게임 오브젝트를 비활성화하거나 파괴
        //gameObject.SetActive(false);
        // 또는 Destroy(gameObject);

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void DestroyPhotonViewObject()
    {
        photonView.RPC("RemoveInMaster", RpcTarget.All);
    }

}