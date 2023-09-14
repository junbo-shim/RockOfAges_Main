using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Defence UI
public partial class UIManager : MonoBehaviour
{
    public GameObject rockSelectUI;
    public GameObject attackSelect;
    public GameObject unitSelect;
    public GameObject defenceUI;
    public Sprite[] selectRockSprite;
    public Sprite[] selectUnitSprite;
    public void PrintRockSelectUI()
    {
        rockSelectUI.transform.localScale = Vector3.one;
    }

    public void TurnOffRockSelectUI()
    {
        rockSelectUI.transform.localScale = Vector3.one * 0.001f;
    }

    public void PrintDefenceUI()
    {
        defenceUI.transform.localScale = Vector3.one;
    }

    public void TurnOffDefenceUI()
    {
        defenceUI.transform.localScale = Vector3.one * 0.001f;
    }

    //{ InstantiateRockImgForAttack()
    public void InstantiateRockImgForAttack()
    {
        GameObject motherObj = GameObject.Find("RockSelectUI");
        for(int n = 0; n < ItemManager.itemManager.rockSelected.Count; n++)
        {
            GameObject rockImg = Instantiate(attackSelect, motherObj.transform.Find("RockChooseButtons").Find("RockView").Find("Viewport").Find("Content"));
            rockImg.name = "rockSelect " + n;
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
        for (int n = 0; n < ItemManager.itemManager.unitSelected.Count; n++)
        {
            GameObject unitImg = Instantiate(unitSelect, motherObj.transform.Find("DefenceHolder").Find("UnitButton").Find("UnitView").Find("Viewport").Find("Content"));
            unitImg.name = "unitSelect " + n;
            CreateButton creatButton = FindObjectOfType<CreateButton>();
            creatButton.id = ItemManager.itemManager.unitSelected[n];

            TextMeshProUGUI[] textElements = unitImg.GetComponentsInChildren<TextMeshProUGUI>();

            // scriptable Object가 달린게 없어서 실험 불가
            //float gold;
            //int buildLimit;
            //ResourceManager.Instance.GetUnitGoldAndBuildLimitFromID(creatButton.id, 
            //    ResourceManager.Instance.GetGameObjectByID(creatButton.id), out gold, out buildLimit);

            //foreach (var text in textElements)
            //{
            //    if (text.name == "GoldTxt")
            //    { 
            //        text.text = gold.ToString();
            //    }
            //    if (text.name == "UnitCountTxt")
            //    {
            //        text.text = 0+ "/" + buildLimit.ToString();
            //    }
            //}
        }
    }
    //} InstantiateUnitImgForDenfence()
}
