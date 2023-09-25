using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Global_PSC
{
    public static void ShakeFreeLookCamera(this Camera mainCamera, float AmplitudeGain, float FrequencyGain)
    {

        CinemachineBrain camerabrsin = mainCamera.GetComponent<CinemachineBrain>();
        if (camerabrsin == null) return;
            CinemachineFreeLook camera = camerabrsin.ActiveVirtualCamera as CinemachineFreeLook;

        camera.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = AmplitudeGain;
        camera.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = AmplitudeGain;
        camera.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = AmplitudeGain;

        camera.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = FrequencyGain;
        camera.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = FrequencyGain;
        camera.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = FrequencyGain;
    }


    public static void InitLocalTransformData(Transform _GameObject, Transform parent=null)
    {
        _GameObject.parent = parent.transform;
        _GameObject.localPosition = Vector3.zero;
        _GameObject.localRotation = Quaternion.identity;
        _GameObject.localScale = Vector3.one;
    }
    public static int FindLayerToName(string layerName)
    {

        int layerIndex = LayerMask.NameToLayer(layerName);

        if (layerIndex == -1)
        {
            Debug.LogWarning("레이어 " + layerName + "를 찾을 수 없습니다.");
            return FindLayerToName("Default");
        }
        
        return 1 << layerIndex;
    }

    public static Vector3 GetWorldMousePositionFromMainCamera(float depth)
    {
        Vector3 mouseCurrPos = Input.mousePosition;
        mouseCurrPos.z = depth;
        return Camera.main.ScreenToWorldPoint(mouseCurrPos);

    }
    public static Vector3 GetWorldMousePositionFromMainCamera()
    {
        // 카메라
        Camera mainCamera = Camera.main;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, 1000, Global_PSC.FindLayerToName("Terrains")))
        {
            return raycastHit.point;
        }

        // 반환합니다.
        return Vector3.zero ;

    }

    public static float GetHeight(this GameObject _object, float sizeMultiple)
    {
        MeshFilter meshFilter = _object.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            // 메쉬 데이터 가져오기
            Mesh mesh = meshFilter.sharedMesh;

            if (mesh != null)
            {
                // 메쉬의 경계 상자 (Bounds) 가져오기
                Bounds bounds = mesh.bounds;

                // 높이 계산
                float height = bounds.size.y * sizeMultiple;

                // 결과 출력
                //Debug.Log("Object Height: " + height);

                return height;
            }
        }
        return 1f;
    }

    public static GameObject FindTopLevelGameObject(string name_)
    {
        GameObject[] rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in rootObjs)
        {
            if (obj.name == name_)
            {
                return obj;
            }
        }
        return null;
    }
}

