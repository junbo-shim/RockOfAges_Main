using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class StickyCowObstacle : ObstacleBase
{
    [SerializeField]
    Transform[] cowPosition;

    [SerializeField]
    GameObject singleCow;
    List<GameObject> cowList;

    private void Start()
    {
        transform.GetChild(0).localScale = Vector3.zero;
    }

    protected override void Init()
    {
        base.Init();
        status = Instantiate(status, transform);
        obstacleMeshFilter = GetComponent<MeshFilter>();
        obstacleRenderers = GetComponentsInChildren<Renderer>();
        obstacleRigidBody = GetComponent<Rigidbody>();
    }

    public override ObstacleBase Build(Vector3 position, Quaternion rotate, int currIndex, int count)
    {
        cowList = new List<GameObject>();
        ObstacleBase obstacle;
        obstacle = PhotonNetwork.Instantiate(gameObject.name, position, rotate).GetComponent<ObstacleBase>();
       
        obstacle.transform.localScale = obstacle.transform.localScale;
        {
            //버튼 데이터 변경
            GameObject unitButton = ResourceManager.Instance.FindUnitGameObjById(status.Id);
            unitButton.GetComponent<CreateButton>().buildCount += 1;
            UIManager.uiManager.RePrintUnitCount(status.Id);
        }

        cowPosition = new Transform[obstacle.transform.childCount];
        for(int i = 0; i < cowPosition.Length; i++)
        {
            cowPosition[i] = obstacle.transform.GetChild(i);
        }


        foreach (var respownPosition in cowPosition)
        {
            GameObject cow = PhotonNetwork.Instantiate(singleCow.name, respownPosition.position, respownPosition.rotation);

            cow.transform.localScale = singleCow.transform.localScale; 
            cowList.Add(cow);
        }

        for(int i = 0; i < cowList.Count; i++)
        {
            cowList[i].GetComponent<CowSingleObstacle>().obstacleParent = obstacle.transform;
            //cowList[i].transform.parent = obstacle.transform;

        }

        Invoke("DestroyPhotonViewObject", 1f);

        return obstacle;
    }

}
