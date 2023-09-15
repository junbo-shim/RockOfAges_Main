using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    private void Awake()
    {
        ButtonManager.Instance.CreateThisManager();
        NetworkManager.Instance.CreateThisManager();
        PlayerDataManager.Instance.CreateThisManager();
        //ResourceManager.Instance.CreateThisManager();
    }
}
