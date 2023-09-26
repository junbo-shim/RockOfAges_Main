using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gate : MonoBehaviourPun, IHitObjectHandler, IPunObservable
{
    private GatePhase currPhase = GatePhase.NORMAL;    
    private Transform gateCollider;

    [SerializeField]
    private List<GameObject> gateSkin;

    const int GATE_MAX_HP = 600;
    const float GATE_CRACK_CHANGE = .3f;
    //600 599~200 199~1 0
    //1 1~0.3 0.3~0 0


    [Range(0, GATE_MAX_HP)]
    [SerializeField]
    //private float gateHP = GATE_MAX_HP;
    //{ 0924 홍한범
    // 게이트의 체력을 받아옴
    private float gateHP = default;
    //} 0924 홍한범
   

    // PSC Editted
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) 
    {
        if (stream.IsWriting) 
        {
            stream.SendNext(gateHP);
        }
        else 
        {
            gateHP = (float)stream.ReceiveNext();
        }
        PrintHP();
        ChangePhase();
    }

    //{ 0924 홍한범
    // Awake -> Start
    private void Start()
    {
        gateCollider = transform.Find("DoorCollider");
        //{ 0924 홍한범
        DefineTeamHP();
        //} 0924 홍한범
        ChangePhase();
    }
    //} 0924 홍한범


    //{ 0924 홍한범
    //{ UpdateTeamHP()
    // 성문의 root를 통해서 teamHp를 define
    public void DefineTeamHP()
    { 
        Transform root = gameObject.transform.root;
        if (root.name == "Team1")
        {
            gateHP = CycleManager.cycleManager.team1Hp;
        }
        else if (root.name == "Team2")
        {
            gateHP = CycleManager.cycleManager.team2Hp;
        }
    }
    //} UpdateTeamHP()
    //} 0924 홍한범


    void ChangePhase()
    {

        GatePhase prePhase = currPhase;
        if (gateHP == GATE_MAX_HP)
        {
            currPhase = GatePhase.NORMAL;
        }
        else if (gateHP < GATE_MAX_HP && gateHP> GATE_MAX_HP*GATE_CRACK_CHANGE)
        {
            currPhase = GatePhase.CRACK;
        }
        else if (gateHP <= GATE_MAX_HP * GATE_CRACK_CHANGE && gateHP > 0)
        {
            currPhase = GatePhase.COLLAPSE;
        }
        else
        {
            currPhase = GatePhase.DESTROY;
            gateCollider.gameObject.SetActive(false);
            CameraManager.Instance.ShowBreakDoor(this.gameObject);
        }

        if (prePhase != currPhase)
        {
            gateSkin[(int)prePhase].SetActive(false);
            gateSkin[(int)currPhase].SetActive(true);
        }
    }

    //{ 0924 홍한범
    //{PrintHP()
    // 체력 깎인만큼 출력
    //[PunRPC]
    public void PrintHP()
    {
        Transform root = gameObject.transform.root;
        if (root.name == "Team1")
        {
            CycleManager.cycleManager.team1Hp = gateHP;
            //gateHP = CycleManager.cycleManager.team1Hp;
        }
        else if (root.name == "Team2")
        {
            CycleManager.cycleManager.team2Hp = gateHP;
            //gateHP = CycleManager.cycleManager.team2Hp;
        }
        UIManager.uiManager.PrintTeamHP();
        //Debug.Log("이 게이트 체력" + gateHP);
        //Debug.Log("팀1 :" + CycleManager.cycleManager.team1Hp);
        //Debug.Log("팀2 :" + CycleManager.cycleManager.team2Hp);
    }
    //} 0924 홍한범
    //}PrintHP()




    public void Hit(int damage)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            gateHP -= damage;
        }
        else
        {
            photonView.RPC("RPCHit", RpcTarget.MasterClient, damage);
        }
        photonView.RPC("HitReaction", RpcTarget.All);
    }

    [PunRPC]
    void RPCHit(int damage)
    {
        gateHP -= damage;

        //{ 0924 홍한범
    }

    [PunRPC]
    public void HitReaction()
    {
        //애니메이션, 텍스트, 소리 등 싸이클과는 무관한 리액션을 여기서 구현
        //폴리싱?


        //기타 성문 공격 당할시 처리
        //예상 : 카메라, 싸이클처리?
        //} 0924 홍한범
    }
}


public enum GatePhase
{
    NORMAL = 0,
    CRACK = 1,
    COLLAPSE = 2,
    DESTROY = 3
}