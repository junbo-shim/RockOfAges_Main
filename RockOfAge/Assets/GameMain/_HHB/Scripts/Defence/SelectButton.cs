using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class SelectButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int id;
    public Image rockImg;
    public GameObject _selectedImg;

    private void Start()
    {
        UIManager.uiManager.MatchImageToIDSprite(rockImg, id);
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
        ChangeRockStateSelectToCreate();

    }

    public void ChangeRockStateSelectToCreate()
    {
        ItemManager.itemManager.userRockChoosed[0] = id;
        UIManager.uiManager.SwitchUIManager("defenceUI");
        CycleManager.cycleManager.userState = (int)UserState.DEFENCE;
        SoundManager.soundManager.BGMCycle();
        UIManager.uiManager.SwitchUIManager("rockSelectUI");
        StartCoroutine(CycleManager.cycleManager.WaitForRock());
    }
}
