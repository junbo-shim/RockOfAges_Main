using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneWall : HoldObstacleBase, IHitObjectHandler
{
    [SerializeField]
    private ObstacleBase mainObject;
    [SerializeField]
    private ObstacleBase subObject;
    [SerializeField]
    private ObstacleBase connectObject;

    private GameObject[] stateMesh;

    private Collider currCollider;

    private void Awake()
    {
        Init();
    }



    protected override void Init()
    {
        base.Init();
        stateMesh = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            stateMesh[i] = transform.GetChild(i).gameObject;
            stateMesh[i].SetActive(false);
        }
        stateMesh[0].SetActive(true);
        currCollider = stateMesh[0].GetComponent<MeshCollider>();
        obstacleRenderers = stateMesh[0].GetComponentsInChildren<Renderer>();
    }

    //맵에 Build
    public override ObstacleBase Build(Vector3 position, Quaternion rotate, int currIndex, int count)
    {
        ObstacleBase obstacle;
        if (currIndex == 0 || currIndex == count - 1)
        {
            obstacle = PhotonNetwork.Instantiate(mainObject.name, position, rotate).GetComponent<ObstacleBase>();
        }
        else
        {
            obstacle = PhotonNetwork.Instantiate("StoneWallSub", position, rotate).GetComponent<ObstacleBase>();
        }
        NetworkManager.Instance.myDataContainer.GetComponent<PlayerDataContainer>().playerGold -= obstacle.status.Price;
        NetworkManager.Instance.myDataContainer.GetComponent<PlayerDataContainer>().CheckGold();
        obstacle.transform.localScale = obstacle.transform.localScale;
        {
            //버튼 데이터 변경
            GameObject unitButton = ResourceManager.Instance.FindUnitGameObjById(status.Id);
            unitButton.GetComponent<CreateButton>().buildCount += 1;
            UIManager.uiManager.RePrintUnitCount(status.Id);
        }
        return obstacle;

    }
    void ChangePhase()
    {
        if (status.Health / transform.childCount > currHealth)
        {
            stateMesh[0].SetActive(false);
            stateMesh[1].SetActive(true);
        }
    }

    public void Hit(int damage)
    {
        if (!isBuildComplete)
        {
            Delete();
        }

        currHealth -= damage;
        HitReaction();
        if(currHealth <= 0)
        {
            Dead();
        }
    }

    public void HitReaction()
    {


        ChangePhase();
    }

    protected override void Dead()
    {
        photonView.RPC("Demolish", RpcTarget.All);
    }

    [PunRPC]
    public void Demolish()
    {
        stateMesh[1].SetActive(false);
        stateMesh[0].SetActive(true);
        stateMesh[0].GetComponent<EnvironmentRayFire>().DemolishMeshRenderers();

    }
}
