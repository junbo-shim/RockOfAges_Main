using System.Security.Cryptography.X509Certificates;
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
    // 스텟 정보 출력
    public GameObject statUI;
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
    #endregion

    public void OnPointerClick(PointerEventData eventData)
    { 
    
    }

    public void OnPointerEnter(PointerEventData eventData)
    { 
    
    
    }

    public void OnPointerExit(PointerEventData eventData)
    { 
    
    }

    //{ PackAwake()
    private void PackAwake()
    {
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
            _selectImage.SetActive(true);
        }
        UIManager.uiManager.PrintObstacleCard(id, explain, name, price);
    }
    //} PackOnPointerEnter()

    //{ GetInforFromScriptObj()
    // scriptable 정보
    private void GetInfoFromScriptObj()
    {
        if (obstacleStatus != null)
        {
            id = obstacleStatus.Id;
            obsName = obstacleStatus.ObstacleName;
            price = obstacleStatus.Price;
        }
    }
    //} GetInforFromScriptObj()



}
