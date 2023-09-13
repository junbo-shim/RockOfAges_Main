using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        ItemManager.itemManager.userRockChoosed[0] = id;
        // 여기서 수비 버튼 인스턴스
        UIManager.uiManager.PrintDefenceUI();
        CycleManager.cycleManager.userState = (int)UserState.Defence;
        UIManager.uiManager.TurnOffRockSelectUI();
    }
}
