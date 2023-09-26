using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CatObstacle : MoveObstacleBase, IHitObjectHandler
{
    public AudioSource audioSource;
    public AudioClip followingSound;
    public AudioClip attackSound;

    private float attackRange = 8f;
    private Transform rockTransform;
    private Transform catMother;
    private Transform cat;
    private float moveSpeed = 0.5f;
    private float turnSpeed = 50f;
    private bool isAttacked = false;
    private Vector3 catMotherOriginalPosition;
    private Quaternion catMotherOrigianlRotation;

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
        catMotherOrigianlRotation = catMother.transform.rotation;
        catMotherOriginalPosition = catMother.transform.position;

    }

    private void Update()
    {
        if (!isAttacked && FindTarget())
        {
            MoveCat(rockTransform);
        }
        else if (isAttacked && obstacleAnimator.GetBool("isMoving"))
        { StartCoroutine(GoReturn()); }
        else { return; }
    }

    //이동
    public void MoveCat(Transform rockTransform) 
    {
        audioSource.clip = followingSound;
        audioSource.Play();
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


    IEnumerator GoReturn()
    {
       obstacleAnimator.SetBool("isAttack", false);
       obstacleAnimator.SetBool("isMoving", true);
        if (catMother.position != standPosition)
        {
            catMother.transform.position = Vector3.Lerp(catMother.transform.position,
                standPosition, moveSpeed * Time.deltaTime);
            Quaternion lookDir = Quaternion.LookRotation(standPosition - catMother.position);
            catMother.rotation = lookDir;
        }
        yield return new WaitUntil(() => GetBack());
        isAttacked = false;
        obstacleAnimator.SetBool("isMoving", false);
    }

    private bool GetBack()
    {
        if (Vector3.Distance(catMother.transform.position, standPosition) <= 2f)
        {
            return true;
        }
        else { return false; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rock") && !isAttacked)
        {
            audioSource.clip = attackSound;
            audioSource.Play();
            Debug.Log("공격됨");
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
        isAttacked = true;
        Debug.Log("들어옴");
        obstacleAnimator.SetBool("isMoving", false);
        obstacleAnimator.SetTrigger("Attack");

        //obstacleAnimator.SetBool("isAttack", true);
        StartCoroutine(AttackRock());       
    }

    IEnumerator AttackRock()
    {
        float startTime = 0f;
        // 공의 input값을 참조로 받아서 위치를 설정해야할듯?
        //catMother.transform.rotation *= Quaternion.Euler(0f, 40f, 0f);
        catMother.transform.rotation = catMotherOrigianlRotation;
        // 디버프
        SetDebuff(0.5f, 2f, 0.5f);
        while (startTime <= 5f)
        {
            startTime += Time.deltaTime;
            yield return null;
            catMother.transform.position = rockTransform.position + new Vector3(0.4f, 0.4f, 0f);

            //catMother.transform.rotation *= Quaternion.Euler();
        }
        // 원수치 복구
        SetDebuffBack();

        obstacleAnimator.SetBool("isMoving", true);

        //catMother.transform.position = catMotherOriginalPosition;
    }

    public void SetDebuff(float velocityValue, float massValue, float jumpValue)
    {
        rockTransform.GetComponentInParent<RockBase>().SetObstacleMultiple(velocityValue, massValue, jumpValue);
    }

    public void SetDebuffBack()
    {
        rockTransform.GetComponentInParent<RockBase>().ResetDebuff();
    }
}
