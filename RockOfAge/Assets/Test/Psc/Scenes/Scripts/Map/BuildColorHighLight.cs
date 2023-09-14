using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildColorHighLight : MonoBehaviour, IHitObjectHandler
{
    public Vector2 size;
    private GameObject allowPlane;        // 건설 가능시
    private GameObject denyPlane;        // 건설 불가능시

    void Awake()
    {
        allowPlane = transform.Find("BuildAllow").gameObject;
        denyPlane = transform.Find("BuildDeny").gameObject;

        MeshRenderer allowRenderer =  denyPlane.GetComponent<MeshRenderer>();
        MeshRenderer denyRenderer = denyPlane.GetComponent<MeshRenderer>();

        // 불가능한 plane이 더 위에 오게 렌더 큐를 조정
        denyRenderer.material.renderQueue = allowRenderer.material.renderQueue + 1;
    }


    public void UpdateColorHighLightSize(Vector2 _size)
    {
        this.size = _size;
        allowPlane.transform.localScale = new Vector3(size.x, 1, size.y);
        denyPlane.transform.localScale = new Vector3(size.x, 1, size.y);
    }

    public void UpdateColorHighLightColor(bool canBuild)
    {
        if (canBuild)
        {
            allowPlane.SetActive(true);
            denyPlane.SetActive(false);
        }
        else
        {
            allowPlane.SetActive(false);
            denyPlane.SetActive(true); 
        }
    }

    public void Hit(int damage)
    {
        throw new System.NotImplementedException();
    }

    public void HitReaction()
    {
        throw new System.NotImplementedException();
    }
}
