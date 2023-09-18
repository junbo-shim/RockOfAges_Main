using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
    protected Material originMaterial;

    //타겟
    protected GameObject target;

    protected float currHealth;
    protected bool isBuildComplete = false;

    public static readonly float BUILD_TIME = 5f;


    protected void StartBuild(float time)
    {
        originMaterial = obstacleRenderer.material;
        obstacleRenderer.material = BuildManager.instance.white;

        StartCoroutine(BuildRoutine(time));

    }

    protected IEnumerator BuildRoutine(float buildTime)
    {
        float currTime = 0;
        while (currTime < buildTime)
        {
            yield return Time.deltaTime;
            currTime += Time.deltaTime;
        }

        isBuildComplete = true;
        obstacleRenderer.material = originMaterial;

    }

    //맵에 Build
    public virtual ObstacleBase Build(Vector3 position, Quaternion rotate)
    {
        ObstacleBase obstacle = Instantiate(this, position, rotate);
        obstacle.transform.localScale = Vector3.one * .1f;
        GameObject unitButton = ResourceManager.Instance.FindUnitGameObjById(status.Id);
        unitButton.GetComponent<CreateButton>().buildCount += 1;
        UIManager.uiManager.RePrintUnitCount(status.Id);

        return obstacle;
    }

    //초기화
    protected virtual void Init()
    {
        status = new ObstacleStatus(status);
        obstacleMeshFilter = GetComponent<MeshFilter>();
        obstacleRigidBody = GetComponent<Rigidbody>();
        obstacleAnimator = GetComponent<Animator>();
        obstacleRenderer = GetComponentInChildren<Renderer>();
        currHealth = status.Health;
    }

    //타겟 서치
    //아마 상위에 없어도 될거로 추정됨
    protected virtual void SearchTarget() { }

    //죽음
    protected virtual void Dead() { }

    //공격 활성화
    protected virtual void ActiveAttack() { }
    protected virtual void DeactiveAttack() { }

}