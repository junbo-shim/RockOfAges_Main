using UnityEngine;

public class EndingSwitch : MonoBehaviour
{
    private bool isEnd = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isEnd && other.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            //Transform mother = transform.root;
            //if (mother.gameObject.name == "Team1")
            //{
            //    누구의 승리인지 판별해서CycleManager Enum Result 결정
            //    DefineWinner 이나
            //    DefineLoser 를 부르면됨
            //}
            //else if (mother.gameObject.name == "Team2")
            //{ 

            //}
            isEnd = true;
            Debug.Log(other.name);
            CycleManager.cycleManager.DefineWinner(); // 실험용
            other.gameObject.GetComponentInParent<RockBase>().isDestroy = true;
            other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            other.gameObject.transform.position = new Vector3(2111.84f, -9.45f, 75.42f);
            CameraManager.Instance.SetGameEndCamera(other.gameObject.transform);


        }
    }
}
