using UnityEngine;
using RayFire;


// 한번에 부서지는 게임오브젝트
public class EnvironmentRayFire : MonoBehaviour
{
    private MeshRenderer[] meshRenderers;

    public GameObject refObj;
    public AudioClip audioClip;
    public bool isKinematic = false;
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

            // limitation/collision/by collision
            meshGo.GetComponent<RayfireRigid>().limitations.col = false;

            // Simulation/simulationType
            meshGo.GetComponent<RayfireRigid>().simulationType = SimType.Sleeping;


            // Physics/material/type
            // material의 속성에는 영향을 주지만, 시뮬레이션 자체에는 영향을 크게 주지않는듯함.
            // reference가 가지는 demolition 방식이 더 주요한 인자
            meshGo.GetComponent<RayfireRigid>().physics.mt = MaterialType.Brick;            
            


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

            // MeshDemolition/Properties/meshInput
            meshGo.GetComponent<RayfireRigid>().meshDemolition.inp = RFDemolitionMesh.MeshInputType.AtInitialization;

            // MeshDemolition/Advanced
            // removeCollinear
            //meshGo.GetComponent<RayfireRigid>().meshDemolition.prp.rem = true;
            //// colliderType
            //meshGo.GetComponent<RayfireRigid>().meshDemolition.prp.col = RFColliderType.Sphere;
            //// sizeFilter
            //meshGo.GetComponent<RayfireRigid>().meshDemolition.prp.szF = 7f;
            // runtimeCaching
            meshGo.GetComponent<RayfireRigid>().meshDemolition.ch.tp = CachingType.ByFragmentsPerFrame;
            meshGo.GetComponent<RayfireRigid>().meshDemolition.ch.frm = 1;
            meshGo.GetComponent<RayfireRigid>().meshDemolition.ch.frg = 3;



            // Main/Initialzation
            meshGo.GetComponent<RayfireRigid>().Initialize();

            Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();
            rigidBody.isKinematic = isKinematic;

            //MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
            //meshCollider.convex = false;


        }
    }

    // meshRenderer들을 다 가져와서 부셔야함
    // RPC
    private void DemolishMeshRenderers()
    {
        PlayDestroySound();
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
            //this.gameObject.layer = LayerMask.NameToLayer("Environment");
            DemolishMeshRenderers();
        }
    }

    public void PlayDestroySound()
    {
        SoundManager.soundManager.GetSoundPooling(this.gameObject, audioClip);
    }
}
