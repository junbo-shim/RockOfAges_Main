//using UnityEngine;

//public class KJHSlow : MonoBehaviour
//{
//    public float slowFactor = 0.5f;
//    public string rockLayerName = "Rock";

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.gameObject.layer == LayerMask.NameToLayer(rockLayerName))
//        {
//            other.GetComponent<KJHRockID1>().speed *= slowFactor;
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (other.gameObject.layer == LayerMask.NameToLayer(rockLayerName))
//        {
//            other.GetComponent<RollingBall>().speed /= slowFactor;
//        }
//    }
//}