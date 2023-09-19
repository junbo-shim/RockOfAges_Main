using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CatObstacle : MoveObstacleBase, IHitObjectHandler
{
    private float attackRange = 8f;
    private Transform rockTransform;
    private Transform catMother;
    private Transform cat;
    private float moveSpeed = 0.2f;
    private float turnSpeed = 50f;
    private bool isAttacked = false;

    private void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        cat = transform.GetChild(0);
        catMother = transform;
        status = new ObstacleStatus(status);
        obstacleRigidBody = transform.GetChild(0).GetComponent<Rigidbody>();
        obstacleAnimator = transform.GetChild(0).GetComponent<Animator>();
        obstacleRenderer = transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>();
        currHealth = status.Health;
        standPosition = catMother.transform.position;
    }

    private void Update()
    {
        if (!isAttacked && FindTarget())
        {
            MoveCat(rockTransform);
        }
        else
        { Return(); }
    }

    //이동
    public void MoveCat(Transform rockTransform) 
    {
        obstacleAnimator.SetBool("isMoving", true);
        Quaternion lookDir = Quaternion.LookRotation(rockTransform.position - catMother.position);
        catMother.rotation = Quaternion.Slerp(catMother.rotation, lookDir, turnSpeed * Time.deltaTime);
        catMother.transform.position = Vector3.Lerp(catMother.transform.position,
            rockTransform.position, moveSpeed * Time.deltaTime);
    }

    protected bool FindTarget() 
    {
        int layerMask = Global_PSC.FindLayerToName("Rock");
        Collider[] rock = Physics.OverlapSphere(transform.position,attackRange,layerMask);
        if (rock != null && rock.Length > 0)
        {
            Transform rockGameObj = rock[0].gameObject.transform;
            rockTransform = rockGameObj.transform;
            return true;
        }
        else { return false; }

    }

    //복귀
    public override void Return()
    {
        obstacleAnimator.SetBool("isAttack", false);
        obstacleAnimator.SetBool("isMoving", true);
        Quaternion lookDir = Quaternion.LookRotation(-catMother.transform.position + standPosition);
        catMother.rotation = Quaternion.Slerp(catMother.rotation, lookDir, turnSpeed * Time.deltaTime);
        catMother.transform.position = Vector3.Lerp(catMother.transform.position,
            standPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(catMother.transform.position, standPosition) <= 0.5f)
        {
            isAttacked = false;
            obstacleAnimator.SetBool("isMoving", false);
            catMother.LookAt(rockTransform);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rock") && !isAttacked)
        {
            ActiveAttack();
        }
    }

    public void Hit(int damage)
    {
        HitReaction();
        /*Do Nothing*/
    }
    public void HitReaction()
    {
        /*Do Nothing*/
    }

    //공격 활성화
    protected override void ActiveAttack()
    {
        obstacleAnimator.SetBool("isMoving", false);
        obstacleAnimator.SetBool("isAttack", true);
        StartCoroutine(AttackRock());       
    }

    IEnumerator AttackRock()
    {
        //HitReaction();
        cat.transform.rotation *= Quaternion.Euler(0f, 40f, 0f);
        Vector3 catMotherOriginalPosition = catMother.transform.position;
        catMother.transform.SetParent(rockTransform, false);
        catMother.transform.localPosition = new Vector3(0f, 0f, -0.5f); ;
        catMother.transform.localRotation = Quaternion.identity;
        yield return new WaitForSeconds(5);
        catMother.transform.parent = null;
        catMother.transform.position = catMotherOriginalPosition;
        isAttacked = true;
        cat.transform.rotation *= Quaternion.Euler(0f, -40f, 0f);
    }
}
