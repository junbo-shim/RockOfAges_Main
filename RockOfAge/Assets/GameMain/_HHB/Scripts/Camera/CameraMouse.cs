using UnityEngine;
using Cinemachine;


public class CameraMouse : MonoBehaviour
{
    // topViewCamera 컴포넌트
    public CinemachineVirtualCamera nowOnCamera;
    // 휠 클릭시 이동되는 카메라
    public CinemachineVirtualCamera nextOnCamera;
    // 돌 카메라
    //public CinemachineVirtualCamera rockCamera;
    public CinemachineFreeLook rockCamera;
    // topViewCamera 움직임 제어
    private CinemachineTransposer transposer;
    // X,Z 유저 입력
    private float xInput;
    private float zInput;
    // 움직임 방향
    private Vector3 moveDir;
    // 최종 움직임 포지션
    private Vector3 targetPosition;
    // 카메라 이동 속도
    private float cameraSpeed = 500f;
    // smoothDamp 
    private float smoothTime = 0.02f;
    // ref velocity
    private Vector3 velocity = Vector3.zero;
    // 마우스 이동 엣지간격
    private float edgeSize = 50f;


    private void Start()
    {
        transposer = nowOnCamera.GetCinemachineComponent<CinemachineTransposer>();
    }

    private void Update()
    {
        //if (/*CycleManager.cycleManager.userState == (int)UserState.Defence*/)
        { 
            MoveCameraFromKeyBoard();
            RotateCameraTransition();    
            MoveCameraFromMouse();
        }
        ChangeCameraToRock();
    }

    //{ MoveCameraFromInput()
    public void MoveCameraFromKeyBoard()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");

        if (transposer != null)
        {
            moveDir = new Vector3(xInput ,0f, zInput).normalized;
            if(moveDir != Vector3.zero)
            {
                Vector3 moveDistance = moveDir * cameraSpeed * Time.deltaTime;

                targetPosition = transform.position + moveDistance;

                // SmothDamp 원래위치/ 최종위치/ 이전 프레임 속도/ 수렴시간(작을수록 빨리 움직임) 
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

                // 다음 카메라도 같이 이동
                if (nextOnCamera != null)
                {
                    float nowX = transform.position.x;
                    float nowZ = transform.position.z;
                    float nextY = nextOnCamera.transform.position.y;
                    nextOnCamera.transform.position = new Vector3(nowX, nextY, nowZ);
                }

            }
        }
    }
    //} MoveCameraFromInput()

    //{ RotateCameraFromInput()
    public void RotateCameraTransition()
    {
        if (Input.GetMouseButtonDown(2) == true)
        {
            nowOnCamera.gameObject.SetActive(false);
            nextOnCamera.gameObject.SetActive(true);
        }
    }
    //} RotateCameraFromInput()

    //{ ChangeCameraToRock()
    public void ChangeCameraToRock()
    {
        //IsMine
        if (CycleManager.cycleManager.userState == (int)UserState.ATTACK)
        {
            Debug.Log("top -> rock");
            rockCamera.gameObject.SetActive(true);
            nowOnCamera.gameObject.SetActive(false);
            nextOnCamera.gameObject.SetActive(false);
        }
    }
    //} ChangeCameraToRock()

    //{ MoveCameraFromMouse()
    public void MoveCameraFromMouse()
    {
        if (Input.mousePosition.x > Screen.width - edgeSize)
        {
            EdgeMove(Vector3.right);
        }
        if (Input.mousePosition.x < edgeSize)
        {
            EdgeMove(Vector3.left);
        }
        if (Input.mousePosition.y < edgeSize)
        {
            EdgeMove(-Vector3.forward);
        }
        if (Input.mousePosition.y > Screen.height - edgeSize)
        {
            EdgeMove(-Vector3.back);
        }

    }
    //} MoveCameraFromMouse()


    //{ EdgeMove(Vector3 dir)
    private void EdgeMove(Vector3 dir)
    {
        Vector3 moveDistance = dir * cameraSpeed * Time.deltaTime;

        targetPosition = transform.position + moveDistance;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // 다음 카메라도 같이 이동
        if (nextOnCamera != null)
        {
            float nowX = transform.position.x;
            float nowZ = transform.position.z;
            float nextY = nextOnCamera.transform.position.y;
            nextOnCamera.transform.position = new Vector3(nowX, nextY, nowZ);
        }
    }
    //} EdgeMove(Vector3 dir)
}
