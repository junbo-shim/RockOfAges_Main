using UnityEngine;
using System;
using TMPro;
using Photon.Pun;

public class Room : MonoBehaviour
{
    private string roomName;
    private int playerNumbers;
    private string roomPW;

    private TMP_Text roomNameText;
    private TMP_Text playerNumbersText;

    private void Awake()
    {
        roomName = NetworkManager.Instance.CreateRoomPopup.Find("InputField_RoomName").GetComponent<TMP_InputField>().text;
        roomPW = NetworkManager.Instance.CreateRoomPopup.Find("InputField_RoomPW").GetComponent<TMP_InputField>().text;
        //playerNumbers = PhotonNetwork.;
        roomNameText = transform.Find("Text_RoomName").GetComponent<TMP_Text>();
        playerNumbersText = transform.Find("Text_RoomPlayer").GetComponent<TMP_Text>();
    }

    public void PressRoom() 
    {
        
    }
}
