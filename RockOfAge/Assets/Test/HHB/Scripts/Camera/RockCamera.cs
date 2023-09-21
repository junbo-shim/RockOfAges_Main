using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class RockCamera : MonoBehaviour
{
    public CinemachineVirtualCamera nowOnCamera;
    public CinemachineVirtualCamera nextOnCamera;

    // ! Photon
    public PhotonView dataContainerView;

    // ! Photon
    private void Awake() 
    {
        dataContainerView = NetworkManager.Instance.myDataContainer.GetComponent<PhotonView>();
    }

    private void Update()
    {
        // ! Photon
        if (dataContainerView.IsMine == true)
        { 
            MoveCameraToTopView();
        }
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
