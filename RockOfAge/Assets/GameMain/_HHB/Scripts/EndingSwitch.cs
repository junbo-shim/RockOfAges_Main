using Photon.Pun;
using UnityEngine;

public class EndingSwitch : MonoBehaviourPun
{
    private bool isEnd = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isEnd && other.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            int rockViewID = other.gameObject.GetComponentInParent<PhotonView>().ViewID;
            string rockOwnerID = CycleManager.cycleManager.DropLastThreeChar(rockViewID.ToString()) + "001";
            Debug.LogError(rockOwnerID);
            PlayerDataContainer rockOwnerContainer = 
                NetworkManager.Instance.dataContainers.Find(x => x.PlayerViewID == rockOwnerID);
            Debug.LogError(rockOwnerContainer);
            string winnerTeam = rockOwnerContainer.PlayerTeamNum.Split('_')[1];

            Debug.Log("들어감");
            Transform mother = transform.root;
            //if (mother.gameObject.name == winnerTeam)
            //{
            photonView.RPC("LoadEndUI", RpcTarget.All, winnerTeam);
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

    [PunRPC]
    public void LoadEndUI(string winnerTeam)
    {
        string myTeam =
            NetworkManager.Instance.myDataContainer.GetComponent<PlayerDataContainer>().PlayerTeamNum.Split('_')[1];


        Debug.Log("myTeam : " + myTeam);
        Debug.Log("winnerTeam : "+winnerTeam);

        if (myTeam == winnerTeam)
        {
            Debug.Log("Team1");
            CycleManager.cycleManager.DefineWinner();
        }
        else
        {
            Debug.Log("Team2");
            CycleManager.cycleManager.DefineLoser();
        }
        UIManager.uiManager.PrintVicOrLose(myTeam, winnerTeam);
    }
}
