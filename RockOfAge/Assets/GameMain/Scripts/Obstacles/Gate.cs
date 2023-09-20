using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour, IHitObjectHandler
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
    private float gateHP = GATE_MAX_HP;

    private void Awake()
    {
        gateCollider = transform.Find("DoorCollider");
        ChangePhase();
    }

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
        }

        if (prePhase != currPhase)
        {
            gateSkin[(int)prePhase].SetActive(false);
            gateSkin[(int)currPhase].SetActive(true);
        }

    }

    public void Hit(int damage)
    {
        //기타 성문 공격 당할시 처리
        //예상 : 카메라, 싸이클처리?

        gateHP -= damage;
        HitReaction();
        ChangePhase();
    }

    public void HitReaction()
    {
        //애니메이션, 텍스트, 소리 등 싸이클과는 무관한 리액션을 여기서 구현
        //폴리싱?
    }
}


public enum GatePhase
{
    NORMAL = 0,
    CRACK = 1,
    COLLAPSE = 2,
    DESTROY = 3
}