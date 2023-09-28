using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleObject : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> character; // Character 스프라이트들을 저장할 리스트

    [SerializeField]
    private float rotationSpeed = 10f; // 회전 속도

    [SerializeField]
    private float flyAwaySpeed = 5f; // 날아가는 속도
                                     
    [SerializeField]                    //스프라이트에 대한 사운드를 설정합니다.
    private AudioClip[] characterSounds;

    private AudioSource audioSource; // 이 스크립트에서 사용할 AudioSource
    private bool isHighJump; // 고점프 상태 여부를 나타내는 불리언 변수
    private Coroutine jumpCoroutine; // 점프 코루틴을 저장하기 위한 변수
    private Coroutine jumpAndFall; // 점프 후 낙하 코루틴

    private Rigidbody rb;
    private Quaternion originalCameraRotation; // 카메라의 초기 회전값 저장
    private bool cameraRotationStopped = false; // 카메라 회전 멈춤 상태를 나타내는 변수
    private bool isFly = false; // 날아가는 상태 여부
    private bool isUnable = false;

    public BoxCollider boxCollider;

    private void Awake()
    {
        // character 리스트에서 랜덤하게 스프라이트를 선택하기 위한 인덱스
        int index = Random.Range(0, character.Count);

        // 현재 게임 오브젝트의 SpriteRenderer 컴포넌트를 가져와서 선택한 스프라이트로 설정
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = character[index];
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        // AudioSource 컴포넌트를 가져와서 참조
        audioSource = GetComponent<AudioSource>();

        // character 리스트를 비워줌
        character.Clear();
    }

    void Start()
    {
        // 카메라의 초기 회전값 저장
        originalCameraRotation = Camera.main.transform.rotation;

        // Jump 코루틴을 시작
        jumpCoroutine = StartCoroutine(Jump());
    }

    void Update()
    {
        // 카메라의 회전을 사용하여 현재 게임 오브젝트의 회전을 설정
        if (!cameraRotationStopped)
        {
            Vector3 vector3 = Camera.main.transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(new Vector3(0, vector3.y, 0));
        }
    }

    IEnumerator Jump()
    {
        float lowJumpHeight = 0.1f; // 낮은 점프 높이
        float highJumpHeight = 0.2f; // 높은 점프 높이

        // 현재 점프의 높이를 설정하는 변수
        float jumpHeight = isHighJump ? highJumpHeight : lowJumpHeight;

        // 고점프 상태를 번갈아가며 설정
        isHighJump = !isHighJump;

        float jumpDuration = 0.1f; // 점프 지속 시간
        float delayBetweenJumps = Random.Range(0.1f, 0.2f); // 점프 사이의 딜레이 시간을 랜덤하게 설정

        // 점프 시작 위치
        Vector3 startPosition = transform.position;

        // 점프 목표 위치
        Vector3 targetPosition = startPosition + new Vector3(0, jumpHeight, 0);

        // 경과 시간을 초기화
        float elapsedTime = 0;

        // 점프 애니메이션 (올라감)을 위한 루프
        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / jumpDuration);
            yield return null;
        }

        // 경과 시간을 초기화
        elapsedTime = 0;

        // 점프 애니메이션 (내려감)을 위한 루프
        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(targetPosition, startPosition, elapsedTime / jumpDuration);
            yield return null;
        }

        // 점프 사이의 딜레이 후 다음 점프를 시작
        yield return new WaitForSeconds(delayBetweenJumps);
        jumpCoroutine = StartCoroutine(Jump());
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isFly && other.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            // 이전에 실행 중인 점프 코루틴을 멈추기
            if (jumpCoroutine != null)
            {
                StopCoroutine(jumpCoroutine);
            }
            // 랜덤한 스프라이트에 대한 랜덤한 사운드 재생
            int characterIndex = Random.Range(0, characterSounds.Length);
            PlayCharacterSound(characterIndex);


            // 카메라를 멈추고 점프를 멈추고 z축으로 계속 회전하는 코루틴 시작
            jumpAndFall = StartCoroutine(BlowAway());
            cameraRotationStopped = true; // 카메라 회전 멈춤 상태 설정
        }
        
        if (isFly && other.gameObject.layer == LayerMask.NameToLayer("Terrains") && rb.velocity.y < 0)
        {
            rb.isKinematic = true;
            StopAllCoroutines();
            transform.rotation = Quaternion.Euler(90, 0, Random.Range(0, 180));
            Destroy(gameObject, 2f);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isUnable && collision.gameObject.layer == LayerMask.NameToLayer("Terrains"))
        {
            boxCollider.isTrigger = true;
            isUnable = true;
            rb.isKinematic = true;
        }
    }
    private void PlayCharacterSound(int characterIndex)
    {
        if (characterIndex >= 0 && characterIndex < characterSounds.Length)
        {
            audioSource.clip = characterSounds[characterIndex];
            audioSource.Play();
        }
    }
    IEnumerator BlowAway()
    {
        isFly = true;
        rb.isKinematic = false;
        rb.AddForce(Vector3.up * Random.Range(2f, 5f), ForceMode.Impulse);

        while (true)
        {
            // z축을 기준으로 빙글빙글 돌리기
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}