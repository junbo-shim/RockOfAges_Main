using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Defence UI
public partial class UIManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject rockSelectUI;
    [Header("BUTTON PREFAB")]
    public GameObject rockselect;
    public GameObject unitSelect;
    [Header("UI")]
    public GameObject defenceUI;
    [Header("SPRITE MATCH")]
    public Sprite[] selectRockSprite;
    public Sprite[] selectUnitSprite;

    //{ ChangeStateUnitSelectToRockSelect()
    // 유닛선택이 완료되면 공선택하는 UI출력하는 함수
    public void ChangeStateUnitSelectToRockSelect()
    {
        SwitchUIManager("rockSelectUI");
        ItemManager.itemManager.userRockChoosed[0] = 0;
        InstantiateUserSelect();
        ShutDownUserSelectUI();
        SwitchUIManager("commonUI");
    }
    //} ChangeStateUnitSelectToRockSelect()

    public void ChangeAttackToSelect()
    {
        ItemManager.itemManager.userRockChoosed[0] = 0;
        SwitchUIManager("rockSelectUI");
        SwitchUIManager("attackUI");
    }

    // 유저가 선택한 유닛들을 버튼을 프린트하는 함수
    public void InstantiateUserSelect()
    {
        InstantiateRockImgForAttack();
        InstantiateUnitImgForDenfence();
    }

    #region 유저선택 출력함수
    //{ InstantiateRockImgForAttack()
    public void InstantiateRockImgForAttack()
    {
        GameObject motherObj = GameObject.Find("RockSelectUI");
        for(int n = 0; n < ItemManager.itemManager.rockSelected.Count; n++)
        {
            GameObject rockImg = Instantiate(rockselect, motherObj.transform.Find("RockChooseButtons").Find("RockView").Find("Viewport").Find("Content"));
            rockImg.name = "rockSelect" + n;
            SelectButton selectButton = FindObjectOfType<SelectButton>();
            selectButton.id = ItemManager.itemManager.rockSelected[n];
        }
    }
    //} InstantiateRockImgForAttack()

    //{ MatchImageToIDSprite()
    public void MatchImageToIDSprite(Image image_, int id_)
    {
        if (id_ <= 10)
        {
            int index = id_ - 1;
            image_.sprite = selectRockSprite[index];
        }
        if (id_ > 10)
        {
            int index = id_ - 11;
            image_.sprite = selectUnitSprite[index];
        }
    }
    //} MatchImageToIDSprite()

    //{ InstantiateUnitImgForDenfence()
    public void InstantiateUnitImgForDenfence()
    {
        GameObject motherObj = GameObject.Find("DefenceUI");
        ItemManager.itemManager.unitSelected.Sort();

        foreach (int n in ItemManager.itemManager.unitSelected)
        {
            GameObject unitImg = Instantiate(unitSelect, motherObj.transform.Find("DefenceHolder").Find("UnitButton").Find("UnitView").Find("Viewport").Find("Content"));
            unitImg.name = "unitSelect" + n;
            CreateButton creatButton = FindObjectOfType<CreateButton>();
            creatButton.id = n;
            creatButton.buildCount = 0;

            TextMeshProUGUI[] textElements = unitImg.GetComponentsInChildren<TextMeshProUGUI>();

            float gold;
            int buildLimit;
            ResourceManager.Instance.GetUnitGoldAndBuildLimitFromID(creatButton.id, out gold, out buildLimit);

            creatButton.buildLimit = buildLimit;

            foreach (var text in textElements)
            {
                if (text.name == "GoldTxt")
                {
                    text.text = gold.ToString();
                }
                if (text.name == "UnitCountTxt")
                {
                    text.text = creatButton.buildCount.ToString() + "/" + buildLimit.ToString();
                }
            }
        }
    }
    //} InstantiateUnitImgForDenfence()
    #endregion

    //{ RePrintUnitCount()
    // 유닛을 클릭하면 재출력하는 로직
    public void RePrintUnitCount(int id_)
    {
        TextMeshProUGUI unitTxt = ResourceManager.Instance.FindUnitTextById(id_);
        if (unitTxt != null)
        {
            float gold;
            int buildLimit;
            ResourceManager.Instance.GetUnitGoldAndBuildLimitFromID(id_, out gold, out buildLimit);
            GameObject target = ResourceManager.Instance.FindUnitGameObjById(id_);
            Debug.LogFormat("id = {0}",target.name);
            CreateButton creatbutton = target.GetComponent<CreateButton>();
            int buildCount = creatbutton.buildCount;
            unitTxt.text = buildCount.ToString() + "/" + buildLimit.ToString();
        }
    }

}
