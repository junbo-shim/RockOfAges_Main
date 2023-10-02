using UnityEngine;
using Cinemachine;
using System.Collections;

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

        if (!CameraManager.isControlled && CycleManager.cycleManager.userState == (int)UserState.DEFENCE)
        {
            MoveCameraFromKeyBoard();
            RotateCameraTransition();
            MoveCameraFromMouse();
            ScrollMouse();
            ClampCameras();
        }
        CameraManager.Instance.UpdateMyCameraCenterPoint();
        if (!CameraManager.isControlled)
        {
            ChangeCameraToRock();
        }
    }

    //{ MoveCameraFromInput()
    public void MoveCameraFromKeyBoard()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");

        if (transposer != null)
        {
            moveDir = new Vector3(xInput, 0f, zInput).normalized;
            if (moveDir != Vector3.zero)
            {
                Vector3 moveDistance = moveDir * cameraSpeed * Time.deltaTime;

                targetPosition = transform.position + moveDistance;

                // SmothDamp 원래위치/ 최종위치/ 이전 프레임 속도/ 수렴시간(작을수록 빨리 움직임) 
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
                FollowNowOnCamera();
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
        if (CycleManager.cycleManager.userState == (int)UserState.ATTACK)
        {
            rockCamera.gameObject.SetActive(true);
            nowOnCamera.gameObject.SetActive(false);
            nextOnCamera.gameObject.SetActive(false);
        }
    }
    //} ChangeCameraToRock()

    //{ MoveCameraFromMouse()
    public void MoveCameraFromMouse()
    {
        if (ControlCameraPosition())
        {
            if (Input.mousePosition.x > Screen.width - edgeSize)
            {
                EdgeMove(Vector3.right);
            }
            else if (Input.mousePosition.x < edgeSize)
            {
                EdgeMove(Vector3.left);
            }
            else if (Input.mousePosition.y < edgeSize)
            {
                EdgeMove(-Vector3.forward);
            }
            else if (Input.mousePosition.y > Screen.height - edgeSize)
            {
                EdgeMove(-Vector3.back);
            }
            FollowNowOnCamera();
        }
    }
    //} MoveCameraFromMouse()


    //{ EdgeMove(Vector3 dir)
    private void EdgeMove(Vector3 dir)
    {
        Vector3 moveDistance = dir * cameraSpeed * Time.deltaTime;

        targetPosition = transform.position + moveDistance;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

    }
    //} EdgeMove(Vector3 dir)

    public bool ControlCameraPosition()
    {
        Vector3 cameraPosition = nowOnCamera.transform.position;
        float minX = -150f;
        float maxX = 150f;
        float minZ = -150f;
        float maxZ = 150f;

        if (cameraPosition.x < maxX && cameraPosition.x > minX &&
            cameraPosition.z < maxZ && cameraPosition.z > minZ)
        {
            return true;
        }
        else
        {
            ControlEdgeToMoveCamera();
            return false;
        }
    }

    public void ControlEdgeToMoveCamera()
    {
        Vector3 cameraPosition = nowOnCamera.transform.position;
        if (cameraPosition.x >= 150f)
        {
            cameraPosition.x -= 5f;
        }
        if (cameraPosition.x <= -150f)
        {
            cameraPosition.x += 5f;
        }
        if (cameraPosition.z >= 150f)
        {
            cameraPosition.z -= 5f;
        }
        if (cameraPosition.z <= -150f)
        {
            cameraPosition.z += 5f;
        }
        nowOnCamera.transform.position = cameraPosition;
        FollowNowOnCamera();
    }

    public void FollowNowOnCamera()
    {
        if (nextOnCamera != null)
        {
            float nowX = transform.position.x;
            float nowZ = transform.position.z;
            float nextY = nextOnCamera.transform.position.y;
            nextOnCamera.transform.position = new Vector3(nowX, nextY, nowZ);
        }
    }

    public void ScrollMouse()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        float scrollSpeed = 30f;
        float targetY = default;
        float velocity = 0f;
        if (scrollInput == 0f)
        {
            return;
        }
        if (scrollInput != 0f)
        {
            targetY = nowOnCamera.transform.position.y - scrollInput * scrollSpeed;
        }
        float newY = Mathf.SmoothDamp(nowOnCamera.transform.position.y, targetY, ref velocity, smoothTime);
        Vector3 cameraPosition = nowOnCamera.transform.position;
        cameraPosition.y = newY;
        nowOnCamera.transform.position = cameraPosition;
        FollowNowOnCamera();
    }

    public void ClampCameras()
    {
        if (nowOnCamera.name == "TopViewCamera")
        {
            OnCheckTerrains();
            if (nowOnCamera.transform.position.y > 150f)
            {
                nowOnCamera.transform.position = new Vector3(nowOnCamera.transform.position.x, 150f, nowOnCamera.transform.position.z);
            }
            else if (nowOnCamera.transform.position.y < 10f)
            {
                nowOnCamera.transform.position = new Vector3(nowOnCamera.transform.position.x, 30f, nowOnCamera.transform.position.z);
            }

            #region LEGACY
            //nowOnCamera.transform.position = new Vector3(nowOnCamera.transform.position.x, 20f, nowOnCamera.transform.position.z);
            //Vector3 cameraCenter = nowOnCamera.transform.position;
            //Vector3 dirCamera = nowOnCamera.transform.forward;
            //Ray ray = new Ray(cameraCenter, dirCamera);
            //RaycastHit hit;
            //if (Physics.Raycast(ray, out hit))
            //{
            //    GameObject hitObj = hit.collider.gameObject;
            //    int hitLayer = hitObj.layer;
            //    if (hitLayer == LayerMask.NameToLayer("Terrains"))
            //    {
            //        float newCameraY = hitObj.transform.position.y + 20f;
            //        nowOnCamera.transform.position = new Vector3(nowOnCamera.transform.position.x, newCameraY, nowOnCamera.transform.position.z);
            //    }
            //}
            #endregion 

        }
        else if (nowOnCamera.name == "ClickedTopViewCamera")
        {
            OnCheckTerrains();
            if (nowOnCamera.transform.position.y > 80f)
            {
                nowOnCamera.transform.position = new Vector3(nowOnCamera.transform.position.x, 80f, nowOnCamera.transform.position.z);
            }
            if (nowOnCamera.transform.position.y < 10f)
            {
                nowOnCamera.transform.position = new Vector3(nowOnCamera.transform.position.x, 30f, nowOnCamera.transform.position.z);
            }
        }
    }


    public void OnCheckTerrains()
    {
        Vector3 cameraCenter = nowOnCamera.transform.position;
        Vector3 dirCamera = nowOnCamera.transform.forward;
        Ray ray = new Ray(cameraCenter, dirCamera);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObj = hit.collider.gameObject;
            int hitLayer = hitObj.layer;
            if (hitLayer == LayerMask.NameToLayer("Terrains"))
            {
                float dis = Mathf.Abs(hitObj.transform.position.y - nowOnCamera.transform.position.y);
                if (dis < 10f)
                {
                    float newCameraY = hitObj.transform.position.y + 15f;
                    nowOnCamera.transform.position = new Vector3(nowOnCamera.transform.position.x, newCameraY, nowOnCamera.transform.position.z);
                }
            }
        }
    }

    #region LEGACY
    //private bool isTerrain = false;
    //private bool isChanged = true;

    //public void OnCheckTerrains()
    //{
    //    Vector3 cameraCenter = nowOnCamera.transform.position;
    //    Vector3 dirCamera = nowOnCamera.transform.forward;
    //    Ray ray = new Ray(cameraCenter, dirCamera);
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        GameObject hitObj = hit.collider.gameObject;
    //        int hitLayer = hitObj.layer;
    //        if (hitLayer == LayerMask.NameToLayer("Terrains") && !isTerrain)
    //        {
    //            isTerrain = true;
    //            isChanged = true;
    //            MoveYCamera();
    //        }
    //        else if (hitLayer != LayerMask.NameToLayer("Terrains") && isTerrain)
    //        {
    //            isTerrain = false;
    //            isChanged = true;
    //            MoveYCamera();
    //        }
    //    }
    //}

    //public void MoveYCamera()
    //{
    //    float targetY = default;
    //    float originalX = nowOnCamera.transform.position.x;
    //    float originalZ = nowOnCamera.transform.position.z;
    //    if (isTerrain)
    //    {
    //        targetY = nowOnCamera.transform.position.y + 15f;
    //    }
    //    else
    //    {
    //        targetY = nowOnCamera.transform.position.y - 15f;
    //    }
    //    //float newY = Mathf.Lerp(nowOnCamera.transform.position.y, targetY, smoothSpeed * Time.deltaTime);
    //    //nowOnCamera.transform.position = new Vector3(originalX, newY, originalZ);
    //    isChanged = true;
    //    StartCoroutine(LerpCameraPosition(targetY, originalX, originalZ));

    //}

    //IEnumerator LerpCameraPosition(float targetY, float originalX, float originalZ)
    //{
    //    float time = 0f;
    //    float targetTime = 2f;

    //    while (time < targetTime)
    //    {
    //        float newY = Mathf.Lerp(nowOnCamera.transform.position.y, targetY, time / targetTime);
    //        nowOnCamera.transform.position = new Vector3(originalX, newY, originalZ);
    //        time += Time.deltaTime;
    //        yield return null;
    //    }
    //}

    //private void OnCollisionStay(Collision collision)
    //{
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("Terrains"))
    //    {
    //        float originalX = nowOnCamera.transform.position.x;
    //        float originalZ = nowOnCamera.transform.position.z;
    //        float targetY = nowOnCamera.transform.position.y + 15f;
    //        nowOnCamera.transform.position = new Vector3(originalX, targetY, originalZ);
    //    }
    //}
    #endregion
}
