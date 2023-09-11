using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitHolderButton : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
{
    #region 변수
    // 제거 이미지
    private GameObject _removeImage;
    // UnitButton에서 받은 ID
    [HideInInspector]
    public int unitId;
    // 자기 자신의 이미지
    private Image _image;
    // 클릭 색 조절
    private Color _orignialColor;
    private Color _clickedColor;
    #endregion

    private void Awake()
    {
        PackAwake();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PackOnPointerExit();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        PackOnPointerEnter();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        PackOnPointerClick();
    }

    #region Packed
    //{ PackAwake()
    private void PackAwake()
    {
        _image = GetComponent<Image>();
        _orignialColor = _image.color;
        _clickedColor = new Color(100f / 255f, 100f / 255f, 100f / 255f);
        _removeImage = transform.Find("RemoveImage").gameObject;
    }
    //} PackAwake()

    //{ PackOnPointerExit()
    private void PackOnPointerExit()
    {
        _image.color = _orignialColor;
        _removeImage.SetActive(false);
    }
    //} PackOnPointerExit()

    //{ PackOnPointerEnter()
    private void PackOnPointerEnter()
    {
        _image.color = _clickedColor;
        if (ItemManager.itemManager.CheckItemList(unitId))
        {
            _removeImage.SetActive(true);
        }
    }
    //} PackOnPointerEnter()

    //{ PackOnPointerClick()
    private void PackOnPointerClick()
    {
        ItemManager.itemManager.unitSelected.Remove(unitId);
        ItemManager.itemManager.UnitRePrintHolder();
        UnitButton[] unitButtons = FindObjectsOfType<UnitButton>();
        foreach (UnitButton unitButton in unitButtons)
        {
            if (unitButton.id == unitId)
            {
                unitButton.BackToOriginalColor(unitId);
            }
        }
    }
    //} PackOnPointerClick()
    #endregion
}
