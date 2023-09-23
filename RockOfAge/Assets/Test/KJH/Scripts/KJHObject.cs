using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KJHObject : MonoBehaviour
{
    // Character 스프라이트들을 저장할 리스트
    [SerializeField]
    private List<Sprite> character;
    [SerializeField]
    private float rotationSpeed = 360f;
    [SerializeField]
    private float flyAwaySpeed = 5f;

    public LayerMask Rock;
    // 고점프 상태를 나타내는 불리언 변수
    private bool isHighJump;

    private void Awake()
    {
        // character 리스트에서 랜덤하게 스프라이트를 선택하기 위한 인덱스
        int index = Random.Range(0, character.Count);

        // 현재 게임 오브젝트의 SpriteRenderer 컴포넌트를 가져오고 선택한 스프라이트로 설정
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = character[index];

        // 선택한 스프라이트의 인덱스를 디버그 로그에 출력
        Debug.Log(index);

        // character 리스트를 비워줌
        character.Clear();
    }

    void Start()
    {
        // Jump 코루틴을 시작
        StartCoroutine(Jump());
    }

    void Update()
    {
        // 카메라의 회전을 사용하여 현재 게임 오브젝트의 회전을 설정
        Vector3 vector3 = Camera.main.transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(0, vector3.y, 0));
    }

    IEnumerator Jump()
    {
        // 점프 높이 상수 값
        float lowJumpHeight = 0.1f;
        float highJumpHeight = 0.2f;

        // 현재 점프의 높이를 설정하는 변수
        float jumpHeight = isHighJump ? highJumpHeight : lowJumpHeight;

        // 고점프 상태를 번갈아가며 설정
        isHighJump = !isHighJump;

        // 점프 지속 시간
        float jumpDuration = 0.1f;

        // 점프 사이의 딜레이 시간을 랜덤하게 설정
        float delayBetweenJumps = Random.Range(0.1f, 0.2f);

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
        StartCoroutine(Jump());
    }
    
    void OnTriggerEnter(Collision collision)
    {
        Debug.Log("충돌했나>?");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            StartCoroutine(BlowAway());
        }
    }

    IEnumerator BlowAway()
    {
        float blowAwayDuration = 5f;
        float elapsedTime = 0;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, 0, 0);

        while (elapsedTime < blowAwayDuration)
        {
            elapsedTime += Time.deltaTime;

            // Rotate around the z-axis
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            // Move the object upward
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / blowAwayDuration);

            yield return null;
        }
    }
}