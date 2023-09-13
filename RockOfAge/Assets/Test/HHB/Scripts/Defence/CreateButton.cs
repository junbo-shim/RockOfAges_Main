using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CreateButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int id;
    public Image unitImg;
    public GameObject _selectedImg;

    private void Start()
    {
        UIManager.uiManager.MatchImageToIDSprite(unitImg, id);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _selectedImg.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _selectedImg.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(id);
    }
}
