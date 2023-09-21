using UnityEngine;
using Cinemachine;

public class RockCamera : MonoBehaviour
{
    public CinemachineVirtualCamera nowOnCamera;
    public CinemachineVirtualCamera nextOnCamera;

    private void Update()
    {
        MoveCameraToTopView();
    }

    public void MoveCameraToTopView()
    {
        // 죽으면 바꾸기
        if (Input.GetKey(KeyCode.Escape))
        {
            Debug.Log("rock -> top");
            nowOnCamera.gameObject.SetActive(false);
            nextOnCamera.gameObject.SetActive(true);
        }
    
    }
}
