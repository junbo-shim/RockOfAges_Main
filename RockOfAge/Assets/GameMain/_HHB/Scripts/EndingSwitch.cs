using Photon.Pun;
using UnityEngine;

public class EndingSwitch : MonoBehaviour
{
    private bool isEnd = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isEnd && other.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            int rockViewID = other.gameObject.GetComponent<PhotonView>().ViewID;
            string rockOwnerID = CycleManager.cycleManager.DropLastThreeChar(rockViewID.ToString());

            PlayerDataContainer rockOwnerContainer = 
                NetworkManager.Instance.dataContainers.Find(x => x.PlayerViewID == rockOwnerID);

            string winnerTeam = rockOwnerContainer.PlayerTeamNum.Split('_')[1];


            Transform mother = transform.root;
            //if (mother.gameObject.name == winnerTeam)
            //{
            rockOwnerContainer.GetComponent<PhotonView>().RPC("LoadEndUI", RpcTarget.All, winnerTeam);
            //}
            //else
            //{
                
            //}
            isEnd = true;
            //Debug.Log(other.name);
            //CycleManager.cycleManager.DefineWinner(); // 실험용
            other.gameObject.GetComponentInParent<RockBase>().isDestroy = true;
            other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            other.gameObject.transform.position = new Vector3(2111.84f, -9.45f, 75.42f);
            CameraManager.Instance.SetGameEndCamera(other.gameObject.transform);
        }
    }
}
