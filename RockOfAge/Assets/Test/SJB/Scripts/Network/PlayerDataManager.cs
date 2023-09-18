using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Photon.Pun;

public class PlayerDataManager : GlobalSingleton<PlayerDataManager>
{
    public Dictionary<string, string> playerData;
    public string playerID;
    public string playerName;
    public string playerPW;

    public string roomCreateName;
    public string roomCreatePW;

    public string teamNumber;
    public string playerNumber;

    protected override void Awake()
    {
        gameObject.GetComponent<PhotonView>();
    }

    [PunRPC]
    private void SavePlayerTeamAndNumber(string teamNum, string playerNum) 
    {
        teamNumber = teamNum;
        playerNumber = playerNum;
    }

    [PunRPC]
    private void ResetPlayerTeamAndNumber() 
    {
        teamNumber = default;
        playerNumber = default;
    }

    public void ChangeCameraLayerMask(string player, string team)
    { 
        
    }
}
