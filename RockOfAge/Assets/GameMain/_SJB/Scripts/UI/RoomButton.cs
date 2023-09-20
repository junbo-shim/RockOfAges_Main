using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    public string roomNameDisplay;
    public string roomPWDisplay;
    public string roomPlayerDisplay;

    public TMP_Text displayName;
    public TMP_Text displayPlayers;

    private void Awake()
    {
        displayName = transform.Find("Text_RoomName").GetComponent<TMP_Text>();
        displayPlayers = transform.Find("Text_RoomPlayer").GetComponent<TMP_Text>();

        gameObject.GetComponent<Button>().onClick.AddListener(PressRoomButton);
    }

    public void PressRoomButton() 
    {
        
    }
}
