using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region 변수
    // 읽어올 스크립터블 정보
    public ObstacleStatus obstacleStatus;
    #region ObstacleScriptable
    /// UnitScriptable 변수명
    /// id = Id
    /// name = obsName
    /// price = Price
    [HideInInspector]
    public int id;
    [HideInInspector]
    public string obsName;
    [HideInInspector]
    public float price;
    [HideInInspector]
    public string explain;
    #endregion ObstacleScriptable
    // 
    public GameObject holderButton;
    // 선택 이미지
    private GameObject _selectImage;
    // 제거 이미지
    private GameObject _removeImage;
    // 자기 자신의 이미지
    private Image _image;
    // 클릭 색 조절
    private Color _orignialColor;
    private Color _clickedColor;
    [HideInInspector]
    public bool _isChecked { get; private set; }
    // holderButton 생성 x,y좌표
    [HideInInspector]
    public float xPos;
    [HideInInspector]
    public float yPos;
    #endregion

    private void Awake()
    {
        PackAwake();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PackOnPointerClick();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PackOnPointerEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PackOnPointerExit();
    }

    #region Packed
    //{ PackAwake()
    private void PackAwake()
    {
        xPos = 298f;
        yPos = 405f;
        _image = GetComponent<Image>();
        _orignialColor = _image.color;
        _clickedColor = new Color(100f / 255f, 100f / 255f, 100f / 255f);
        _selectImage = transform.Find("SelectImage").gameObject;
        _removeImage = transform.Find("RemoveImage").gameObject;
        GetInfoFromScriptObj();
    }
    //} PackAwake()

    //{ PackOnPointerEnter()
    public void PackOnPointerEnter()
    {
        if (ItemManager.itemManager.CheckItemList(id))
        {
            _removeImage.SetActive(true);
        }
        else
        {
            if (ItemManager.itemManager.CheckUserListCapacity(1) == true)
            {
                _selectImage.SetActive(true);
            }
        }
        UIManager.uiManager.PrintObstacleCard(id, name, explain, price);
        if (_isChecked == false)
        {
            if (ItemManager.itemManager.CheckUserListCapacity(1) == true)
            { 
                InstantiatePreviewHolder();
            }
        }
    }
    //} PackOnPointerEnter()

    //{ PackOnPointerExit()
    public void PackOnPointerExit()
    {
        _removeImage.SetActive(false);
        _selectImage.SetActive(false);
        DestroyPreviewHolder();
    }
    //} PackOnPointerExit()

    //{ PackOnPointerClick()
    public void PackOnPointerClick()
    {
        // 유저가 가지고 있지 않다면
        if (ItemManager.itemManager.CheckItemList(id) == false)
        {
            if (ItemManager.itemManager.CheckUserListCapacity(1) == true)
            {
                // original -> dark
                _image.color = _clickedColor;
                ItemManager.itemManager.unitSelected.Add(id);
                _isChecked = true;
                InstantiateUnitHolder(id);
            }
        }
        else
        // 가지고 있다면
        {
            // dark -> orignial
            _image.color = _orignialColor;
            ItemManager.itemManager.unitSelected.Remove(id);
            _isChecked = false;
            ItemManager.itemManager.UnitRePrintHolder();
        }
    }
    //} PackOnPointerClick()
    #endregion

    #region Functions
    //{ GetInforFromScriptObj()
    // scriptable 정보
    private void GetInfoFromScriptObj()
    {
        if (obstacleStatus != null)
        {
            id = obstacleStatus.Id;
            obsName = obstacleStatus.ObstacleName;
            price = obstacleStatus.Price;
            explain = obstacleStatus.TempString;
        }
    }
    //} GetInforFromScriptObj()

    //{ InstantiatePreviewHolder()
    // Holder 미리보기 - 검은색 인스턴스화
    public void InstantiatePreviewHolder()
    {
        Transform parentTransform = transform.parent;
        GameObject newHolderButton = Instantiate(holderButton, parentTransform);
        newHolderButton.name = "UnitHolderPreview";
        Image image = newHolderButton.GetComponent<Image>();
        UIManager.uiManager.MatchHolderIDSprite(image, id);
        image.color = _clickedColor;
        RectTransform rectTransform = newHolderButton.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(xPos - (float)(84 * ItemManager.itemManager.unitCount), yPos);
    }
    //} InstantiatePreviewHolder()

    //{ DestroyPreviewHolder()
    // Holder 미리보기 Destroy
    public void DestroyPreviewHolder()
    {
        GameObject unitHolder = GameObject.Find("UnitHolderPreview");
        if (unitHolder != null)
        {
            Destroy(unitHolder);
        }
    }
    //} DestroyPreviewHolder()

    //{ InstantiateHolder()
    // Holder 생성
    // 제거 로직은 ItemManager가 관리함
    public void InstantiateUnitHolder(int id_)
    {
        Transform parentTransform = transform.parent;
        GameObject newHolderButton = Instantiate(holderButton, parentTransform);
        newHolderButton.name = "UnitHolder";
        Image image = newHolderButton.GetComponent<Image>();
        UIManager.uiManager.MatchHolderIDSprite(image, id_);
        RectTransform rectTransform = newHolderButton.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(xPos - (float)(84 * ItemManager.itemManager.unitCount), yPos);
        UnitHolderButton myHolderButton = FindObjectOfType<UnitHolderButton>();
        myHolderButton.id = id_;
        ItemManager.itemManager.unitCount++;
    }
    //} InstantiateHolder()

    //{ BackToOriginalColor()
    // Holder를 클릭시 원래대로 색을 되돌리는 함수
    public void BackToOriginalColor(int id_)
    {
        // 유저가 아이템을 가지고 있지 않다면
        if (ItemManager.itemManager.CheckItemList(id_) == false)
        {
            _image.color = _orignialColor;
        }
    }
    //} BackToOriginalColor()

    #endregion
}
