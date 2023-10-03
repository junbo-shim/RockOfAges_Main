using UnityEngine;

namespace RayFire
{
    public class EnvironmentEvent : MonoBehaviour
    {
        private bool isCollision = false;
        public void OnCollisionEnter(Collision collision)
        {
            ChangeLayerMaskToEnvironment(collision);
        }

        public void ChangeLayerMaskToEnvironment(Collision collision)
        {
            if (!isCollision && CheckCondition(collision))
            {
                isCollision = true;
                this.gameObject.layer = LayerMask.NameToLayer("Environment");

                int layer1 = LayerMask.NameToLayer("Environment");
                int layer2 = LayerMask.NameToLayer("Rock");
                int layer3 = LayerMask.NameToLayer("Castle");
                int layer4 = LayerMask.NameToLayer("Obstacles");
                int layer5 = LayerMask.NameToLayer("Walls");
                Physics.IgnoreLayerCollision(layer1, layer2); // 환경과 돌
                Physics.IgnoreLayerCollision(layer1, layer3); // 환경과 성
                Physics.IgnoreLayerCollision(layer1, layer4); // 환경과 방해물
                Physics.IgnoreLayerCollision(layer1, layer5); // 환경과 벽
            }
        }

        public bool CheckCondition(Collision collision)
        {
            return
                collision.gameObject.layer == LayerMask.NameToLayer("Environment") ||
                collision.gameObject.layer == LayerMask.NameToLayer("Rock") ||
                collision.gameObject.layer == LayerMask.NameToLayer("Castle") ||
                collision.gameObject.layer == LayerMask.NameToLayer("Obstacles") ||
                collision.gameObject.layer == LayerMask.NameToLayer("Walls");
        }

    }

}

