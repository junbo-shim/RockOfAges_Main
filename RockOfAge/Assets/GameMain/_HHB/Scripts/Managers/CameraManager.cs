using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : GlobalSingleton<CameraManager>
{
    public static Queue<GameObject> enemyCameraQueue = new Queue<GameObject>(2);

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
}
