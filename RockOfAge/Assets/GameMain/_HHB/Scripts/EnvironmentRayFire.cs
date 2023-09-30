using UnityEngine;
using RayFire;


// 한번에 부서지는 게임오브젝트
public class EnvironmentRayFire : MonoBehaviour
{
    private MeshRenderer[] meshRenderers;

    public GameObject refObj;
    private void Awake()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        CreateRayFireComponents();
    }

    private void CreateRayFireComponents()
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            GameObject meshGo = meshRenderer.gameObject;
            meshGo.AddComponent(typeof(RayfireRigid));

            // meshDemolition/Fragments/amount
            // reference에서는 영향을 받지 않음
            //meshGo.GetComponent<RayfireRigid>().meshDemolition.am = meshDemolitionAmount;

            // Physics/material/type
            // material의 속성에는 영향을 주지만, 시뮬레이션 자체에는 영향을 크게 주지않는듯함.
            // reference가 가지는 demolition 방식이 더 주요한 인자
            meshGo.GetComponent<RayfireRigid>().physics.mt = MaterialType.Glass;

            // DemolitionType/referenceDemolition
            meshGo.GetComponent<RayfireRigid>().demolitionType = DemolitionType.ReferenceDemolition;

            // Materials/Inner
            // reference에서는 영향을 받지 않음
            //meshGo.GetComponent<RayfireRigid>().materials.iMat = inner;
            //Debug.Log(inner.name);

            //Materials/Outer
            // reference에서는 영향을 받지 않음
            //meshGo.GetComponent<RayfireRigid>().materials.oMat = outer;
            //Debug.Log(outer.name);

            // Reference
            meshGo.GetComponent<RayfireRigid>().referenceDemolition.reference = refObj;

            // fading/type
            meshGo.GetComponent<RayfireRigid>().fading.fadeType = FadeType.FallDown;
            meshGo.GetComponent<RayfireRigid>().fading.fadeTime = 1f;
            // Main/Initialzation
            meshGo.GetComponent<RayfireRigid>().Initialize();


        }
    }

    // meshRenderer들을 다 가져와서 부셔야함
    // RPC
    private void DemolishMeshRenderers()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            MeshRenderer meshRenderer = meshRenderers[i];

            GameObject meshGo = meshRenderer.gameObject;
            //meshGo.GetComponent<MeshFilter>().mesh = meshes[i];

            meshRenderer.enabled = false;

            meshGo.GetComponent<RayfireRigid>().Demolish();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            DemolishMeshRenderers();
        }
    }
}
