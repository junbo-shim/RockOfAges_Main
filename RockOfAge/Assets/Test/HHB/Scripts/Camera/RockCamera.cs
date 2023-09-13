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
        if (Input.GetKey(KeyCode.Escape))
        {
            Debug.Log("rock -> top");
            nowOnCamera.gameObject.SetActive(false);
            nextOnCamera.gameObject.SetActive(true);
        }
    
    }
}
