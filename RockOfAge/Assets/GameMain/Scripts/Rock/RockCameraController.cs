using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RockCameraController : MonoBehaviour
{
    CinemachineFreeLook cineFreeLook;

    Transform target = null;
    Coroutine delay = null;

    public static readonly float PLAYER_INPUT_WAIT = 3F;
    public float distance = 10.0f;  // 카메라와 타겟 간 초기 거리

    private Vector3 offset;  // 카메라와 타겟 간의 오프셋


    private void Awake()
    {
        cineFreeLook = GetComponent<CinemachineFreeLook>();

    }

    // Update is called once per frame
    void Update()
    {
       /* Debug.Log(Input.GetAxisRaw("Mouse X"));
        Debug.Log(Input.GetAxisRaw("Mouse Y"));
        Debug.Log(cineFreeLook.LookAt);*/

        if (cineFreeLook.LookAt == null && target ==null )
        {
            return;
        }

        /* if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0)
         {
             delayTime = PLAYER_INPUT_WAIT;
             if (delay == null)
             {
                 offset = new Vector3(0, 2, -3);
                 target = cineFreeLook.LookAt;
                 cineFreeLook.LookAt = null;
                 cineFreeLook.Follow = null;
                 delay = StartCoroutine(FollowDelayRoutine());
             }
         }*/
        //{ 0930 홍한범 조건 추가
        if (CycleManager.cycleManager._isESCed == true)
        {
            return;
        }
        //} 0930 홍한범 조건 추가

        float mouseX = Input.GetAxis("Mouse X");
        float mouseSensitive = 5f;

        if (target != null)
        {

            // offset 값 변경 (y 축은 무시)
            offset = Quaternion.AngleAxis(mouseX, Vector3.up) * offset;

            // 카메라 위치 업데이트 (y 축 값은 고정)
            Vector3 newPosition = target.position + offset;
            newPosition.y = transform.position.y;  // y 축 값 고정
            transform.position = newPosition;

            // 카메라가 항상 타겟을 바라보도록 설정
            //transform.LookAt(-target.position);
           // cineFreeLook.LookAt = target;
        }
        
    }
    float delayTime = 0;
    IEnumerator FollowDelayRoutine()
    {
        Debug.Log("?");
        while (delayTime >= 0)
        {
            yield return new WaitForSeconds( Time.deltaTime );
            delayTime -= Time.deltaTime;
        }
        cineFreeLook.LookAt = target;
        cineFreeLook.Follow = target;
        target = null;
        delay = null;

    }
}
