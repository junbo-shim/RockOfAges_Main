using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraManager : GlobalSingleton<CameraManager>
{
    public static Queue<GameObject> enemyCameraQueue = new Queue<GameObject>(2);
    public static Vector3 myCameraPosition;
    public static bool isControlled = false;
    // PostProcess
    private PostProcessVolume postProcessVolume;
    private DepthOfField depthOfField;
    private MotionBlur motionBlur;
    private bool isBlur = false;

    protected override void Awake()
    {
        GameObject postProcessObj = Global_PSC.FindTopLevelGameObject("Post-process Volume");
        postProcessVolume = postProcessObj.GetComponent<PostProcessVolume>();
    }

    public void SetCameraMotionBlur(GameObject rock)
    {
        postProcessVolume.profile.TryGetSettings(out motionBlur);
        if (rock != null)
        { 
            motionBlur.active = true;
            Rigidbody rockRigidBody = rock.GetComponentInChildren<Rigidbody>();
            if (rockRigidBody != null)
            {
                //Debug.Log("들어감");
                int vel = (int)rockRigidBody.velocity.magnitude;
                if (vel > 20)
                {
                    //Debug.Log("속도 20이상");
                    motionBlur.shutterAngle.value = 320f;
                }
                else 
                {
                    float normalizedVel = Mathf.Clamp01(vel / 20f);
                    float mappedValue = Mathf.Lerp(200f, 300f, normalizedVel);
                    motionBlur.shutterAngle.value = mappedValue;
                    //Debug.Log("shutterAngle : " + mappedValue);
                }
            }
        }
        else { motionBlur.shutterAngle.overrideState = false; /*Debug.Log("나감");*/ }
    }

    public void OffCameraMotionBlur()
    {
        GameObject postProcessObj = Global_PSC.FindTopLevelGameObject("Post-process Volume");
        postProcessVolume = postProcessObj.GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out motionBlur);
        motionBlur.shutterAngle.overrideState = false;
    }

    //public void DebugCameraBlur()
    //{ 
    //    postProcessVolume.profile.TryGetSettings(out depthOfField);

    //    depthOfField.focalLength.value = 1f;
    //    depthOfField.focalLength.value = 150f;

    //    depthOfField.aperture.value = 0.1f;
    //    depthOfField.aperture.value = 16f;

    //}

    public void SetCameraBlurEffect()
    {
        postProcessVolume.profile.TryGetSettings(out depthOfField);
        isBlur = !isBlur;
        if (isBlur == true)
        {

            StartCoroutine(LerpFocalLength(1f, 150f, 0.3f));
            StartCoroutine(LerpAperture(16f, 0.1f, 0.3f));
        }
        else 
        {
            StartCoroutine(LerpFocalLength(150f, 1f, 0.3f));
            StartCoroutine(LerpAperture(0.1f, 16f, 0.3f));
        }
    }

    private IEnumerator LerpFocalLength(float startAperture, float endAperture, float duration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            depthOfField.focalLength.value = Mathf.Lerp(startAperture, endAperture, t);
            yield return null;
        }

        depthOfField.focalLength.value = endAperture;
    }
    private IEnumerator LerpAperture(float startAperture, float endAperture, float duration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            depthOfField.aperture.value = Mathf.Lerp(startAperture, endAperture, t);
            yield return null;
        }

        depthOfField.aperture.value = endAperture;
    }

    public void UpdateMyCameraCenterPoint()
    {
        if (CycleManager.cycleManager.userState == (int)UserState.DEFENCE)
        {
            GameObject topViewCamera = Global_PSC.FindTopLevelGameObject("TopViewCamera");
            if (topViewCamera.activeSelf == true)
            {
                GameObject clickedTopViewCamera = Global_PSC.FindTopLevelGameObject("ClickedTopViewCamera");
                myCameraPosition = clickedTopViewCamera.transform.position;
            }
            else { myCameraPosition = topViewCamera.transform.position; }

        }
    }

    public void SetRockCamera(GameObject userRock, Vector3 startPoint)
    {
        Transform motherRock = userRock.transform;
        Transform childRock = motherRock.Find("RockObject");
        Vector3 cameraTransform = startPoint;
        childRock.transform.position = startPoint;
        GameObject rockCamera = Global_PSC.FindTopLevelGameObject("RockCamera");
        // lookFree 카메라 설정
        CinemachineFreeLook virtualRockCamera = rockCamera.GetComponent<CinemachineFreeLook>();
        virtualRockCamera.transform.position = cameraTransform;
        virtualRockCamera.Follow = childRock.transform;
        virtualRockCamera.LookAt = childRock.transform;

        CinemachineOrbitalTransposer[] transposers = new CinemachineOrbitalTransposer[3];
        CinemachineComposer[] composers = new CinemachineComposer[3];

        for (int i = 0; i < 3; i++)
        {
            composers[i] = virtualRockCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
            transposers[i] = virtualRockCamera.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>();

            transposers[i].m_ZDamping = 0f;
            transposers[i].m_YDamping = 0f;
            composers[i].m_BiasY = 0.43f;
            composers[i].m_SoftZoneWidth = 0.2f;
            composers[i].m_SoftZoneHeight = 0.2f;
            composers[i].m_HorizontalDamping = 0f;
            composers[i].m_VerticalDamping = 0f;
            composers[i].m_ScreenY = 0.5f;
        }
    }

    //public void SetEnemyCamera(GameObject enemyRock)
    //{
    //    Transform motherRock = enemyRock.transform;
    //    Transform childRock = motherRock.Find("RockObject");
    //    GameObject rockCamera = Global_PSC.FindTopLevelGameObject("EnemyRockCamera");

    //    CinemachineFreeLook virtualRockCamera = rockCamera.GetComponent<CinemachineFreeLook>();
    //    virtualRockCamera.Follow = childRock.transform;
    //    virtualRockCamera.LookAt = childRock.transform;

    //    CinemachineOrbitalTransposer[] transposers = new CinemachineOrbitalTransposer[3];
    //    CinemachineComposer[] composers = new CinemachineComposer[3];

    //    for (int i = 0; i < 3; i++)
    //    {
    //        composers[i] = virtualRockCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
    //        transposers[i] = virtualRockCamera.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>();

    //        transposers[i].m_ZDamping = 0f;
    //        transposers[i].m_YDamping = 0f;
    //        composers[i].m_BiasY = 0.43f;
    //        composers[i].m_SoftZoneWidth = 0.2f;
    //        composers[i].m_SoftZoneHeight = 0.2f;
    //        composers[i].m_HorizontalDamping = 0f;
    //        composers[i].m_VerticalDamping = 0f;
    //        composers[i].m_ScreenY = 0.5f;
    //    }
    //}


    public void TurnOffSelectCamera()
    {
        GameObject topViewCamera = Global_PSC.FindTopLevelGameObject("TopViewCamera");
        GameObject selectCamera = Global_PSC.FindTopLevelGameObject("SelectCamera");
        selectCamera.SetActive(false); 
        topViewCamera.SetActive(true);
    }

    public void GetAllCameraOff()
    {
        #region 모든 카메라
        GameObject[] turnOnCameras = new GameObject[8];
        turnOnCameras[0] = Global_PSC.FindTopLevelGameObject("SelectCamera");
        turnOnCameras[1] = Global_PSC.FindTopLevelGameObject("TopViewCamera");
        turnOnCameras[2] = Global_PSC.FindTopLevelGameObject("ClickedTopViewCamera");
        turnOnCameras[3] = Global_PSC.FindTopLevelGameObject("RockCamera");
        turnOnCameras[4] = Global_PSC.FindTopLevelGameObject("GameEndCameraTeam1");
        turnOnCameras[5] = Global_PSC.FindTopLevelGameObject("GameEndCameraTeam2");
        turnOnCameras[6] = Global_PSC.FindTopLevelGameObject("EnemyCamera");
        turnOnCameras[7] = Global_PSC.FindTopLevelGameObject("EnemyRockCamera");
        #endregion

        foreach (GameObject camera in turnOnCameras)
        {
            camera.SetActive(false);
        }
    }

    public void TurnOnGameEndCamera()
    {
        GetAllCameraOff();
        GameObject endingCamera = Global_PSC.FindTopLevelGameObject("GameEndResultCamera");
        endingCamera.SetActive(true);
    }

    public void ShowBreakDoor(GameObject door)
    {
        FindMyTeamFromObj(door);
    }

    public void FindMyTeamFromObj(GameObject door)
    {
        Transform doorRoot = door.transform.root;
        if (doorRoot.name == "Team1")
        {
            GameObject endingTeamCamera = Global_PSC.FindTopLevelGameObject("GameEndCameraTeam1");
            endingTeamCamera.SetActive(true);
            StartCoroutine(WaitForCamera(endingTeamCamera));

        }
        else if (doorRoot.name == "Team2")
        {
            GameObject endingTeamCamera = Global_PSC.FindTopLevelGameObject("GameEndCameraTeam2");
            endingTeamCamera.SetActive(true);
            StartCoroutine(WaitForCamera(endingTeamCamera));
        }
    }

    IEnumerator WaitForCamera(GameObject endingTeamCamera)
    {
        yield return new WaitForSeconds(4f); 
        endingTeamCamera.SetActive(false);
    }

    public void SetEnemyCameraOrder()
    {
        // GameObject로 가지고 있는게 맞나?
        // 상대가 공을 가지고 있다면 Queue에 넣음 (먼저 고른사람이 뒤늦게 소환하면?)
        // 상대가 공을 소환하고 Enqueue
        // 상대 공이 부서지면 Dequeue
        // Peek 첫번째 요소 체크
        // Count 갯수 체크
        // virtual
        if (enemyCameraQueue.Count >= 3)
        {
            Debug.Log("CAMERA LOGIC ERROR");
        }

    }

    public void TurnOnTopViewCamera()
    {
        GameObject topViewCamera = Global_PSC.FindTopLevelGameObject("TopViewCamera");
        GameObject rockCamera = Global_PSC.FindTopLevelGameObject("RockCamera");
        rockCamera.SetActive(false);
        topViewCamera.SetActive(true);
    }

    public void MoveTurnOnCameraPosition(Vector2 position)
    { 
        if (CycleManager.cycleManager.userState == (int)UserState.DEFENCE)
        {
            Vector3 target = default;
            Vector3 original = default;
            GameObject topViewCamera = Global_PSC.FindTopLevelGameObject("TopViewCamera");
            if (topViewCamera.activeSelf == false)
            {
                GameObject topViewClickedCamera = Global_PSC.FindTopLevelGameObject("ClickedTopViewCamera");
                original = topViewClickedCamera.transform.position;
                target = new Vector3(position.x, topViewClickedCamera.transform.position.y, position.y);
                StartCoroutine(MoveCameraPosition(topViewClickedCamera, original, target));
            }
            else 
            {
                original = topViewCamera.transform.position;
                target = new Vector3(position.x, topViewCamera.transform.position.y, position.y);
                StartCoroutine(MoveCameraPosition(topViewCamera, original, target));
            }
        }
    }

    IEnumerator MoveCameraPosition(GameObject camera, Vector3 original, Vector3 target)
    {
        float time = 0f;
        float moveTime = 0.5f;
        while (time < moveTime)
        {
            time += Time.deltaTime;
            camera.transform.position = Vector3.Lerp(original, target , time/moveTime);
            yield return null;
        }

        camera.transform.position = target;
    }

    public void SetGameEndCamera(Transform transform)
    {
        GameObject gameEndCameraObj = Global_PSC.FindTopLevelGameObject("GameEndResultCamera");
        CinemachineVirtualCamera endCamera = gameEndCameraObj.GetComponent<CinemachineVirtualCamera>();
        endCamera.m_LookAt = transform;
    }

}
