using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : GlobalSingleton<PlayerDataManager>
{
    public Dictionary<string, string> playerData;
    public string playerID;
    public string playerName;
    public string playerPW;

    public string roomCreateName;
    public string roomCreatePW;

    protected override void Awake()
    {
        //playerID = 
    }
}
