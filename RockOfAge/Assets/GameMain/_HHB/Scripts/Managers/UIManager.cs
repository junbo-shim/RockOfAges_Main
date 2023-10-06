using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

#region Enum StoneTimer 
// 공생성속도 빠름,보통,느림
public enum StoneTimer
{
    Fast = 5, Normal = 7, Slow = 10
}
#endregion

// Select UI
public partial class UIManager : MonoBehaviour
{ 
    public static UIManager uiManager;

    #region 변수
    [Header("CARD")]
    public Sprite[] rockSprites;
    public Sprite[] obstructionSprites;
    public Image cardImage;
    public Image cardSandImage;
    public Image cardClockImage;
    public TextMeshProUGUI cardNameTxt;
    public TextMeshProUGUI cardInfoTxt;
    public TextMeshProUGUI cardGoldTxt;
    // Holder
    [Header("ROCK HOLDER")]
    public Sprite[] rockHolderSprites;
    // Selection To Defence
    [Header("PLAYERS SELECT READY")]
    public TextMeshProUGUI readyTxt;
    public Image readyImg;
    // selectionUI
    [Header("UI")]
    public GameObject userSelectUI;
    public GameObject escUI;
    // endingUI
    public GameObject endingUI;
    // escUI 작동시 다른 UI 체크
    private List<GameObject> nowOnUI = new List<GameObject>();

    #endregion

    private void Awake()
    {
        uiManager = this;
        DontDestroyOnLoad(uiManager);
        Debug.LogError("1");
        SwitchUIManager("rockSelectUI");
        Debug.LogError("2");
        SwitchUIManager("commonUI");
        Debug.LogError("3");
        SwitchUIManager("attackUI");
        Debug.LogError("4");
        SwitchUIManager("defenceUI");
        Debug.LogError("5");
        SwitchUIManager("endingUI");
        Debug.LogError("6");
        SwitchUIManager("escUI");
        Debug.LogError("7");
        InitnowOnUIList();
        Debug.LogError("8");
        GetDirection();
        Debug.LogError("9");


        //GetEndUI();
        //AssignEndUIButtons();
        PrintPlayerText(NetworkManager.Instance.dataContainers);

        Debug.LogError("00000000000000");
    }

    public void Start()
    {
        quitButton.onClick.AddListener(() => QuitGame());
        userLobbyButton.onClick.AddListener(() => BackToLobby());
    }

    #region Functions
    //{ SwitchUIManager()
    // UI 스케일을 1, 0.001로 왔다갔다 하는 함수 (매개변수에 UI하위의 게임오브젝트 이름)
    // selectUI는 한번쓰고 버림
    public void SwitchUIManager(string uiName_)
    {
        switch (uiName_)
        {
            case "rockSelectUI":
                SwitchRockSelectUI();
                break;
            case "commonUI":
                SwitchCommonUI();
                break;
            case "attackUI":
                SwitchAttackUI();
                break;
            case "defenceUI":
                SwitchDefenceUI();
                break;
            case "endingUI":
                SwitchEndingUI();
                break;
            case "escUI":
                SwitchESCUI();
                break;
        }
    }

    #region UISwitcher
    public void SwitchRockSelectUI()
    {
        Vector3 scale = rockSelectUI.transform.localScale;
        float minScale = 0.001f;
        if (scale == Vector3.one)
        {
            rockSelectUI.transform.localScale = Vector3.one * minScale;
        }
        else if (scale == Vector3.one * minScale)
        {
            rockSelectUI.transform.localScale = Vector3.one;
        }
    }

    public void SwitchDefenceUI()
    {
        Vector3 scale = defenceUI.transform.localScale;
        float minScale = 0.001f;
        if (scale == Vector3.one)
        {
            defenceUI.transform.localScale = Vector3.one * minScale;
        }
        else if (scale == Vector3.one * minScale)
        {
            defenceUI.transform.localScale = Vector3.one;
        }
    }
    public void SwitchAttackUI()
    {
        Vector3 scale = attackUI.transform.localScale;
        float minScale = 0.001f;
        if (scale == Vector3.one)
        {
            attackUI.transform.localScale = Vector3.one * minScale;
        }
        else if (scale == Vector3.one * minScale)
        {
            attackUI.transform.localScale = Vector3.one;
        }
    }
    public void SwitchCommonUI()
    {
        Vector3 scale = commonUI.transform.localScale;
        float minScale = 0.001f;
        if (scale == Vector3.one)
        {
            commonUI.transform.localScale = Vector3.one * minScale;
        }
        else if (scale == Vector3.one * minScale)
        {
            commonUI.transform.localScale = Vector3.one;
        }
    }

    public void SwitchEndingUI()
    {
        Vector3 scale = endingUI.transform.localScale;
        float minScale = 0.001f;
        if (scale == Vector3.one)
        {
            endingUI.transform.localScale = Vector3.one * minScale;
        }
        else if (scale == Vector3.one * minScale)
        {
            endingUI.transform.localScale = Vector3.one;
        }
    }

    public void SwitchESCUI()
    {
        Vector3 scale = escUI.transform.localScale;
        float minScale = 0.001f;
        if (scale == Vector3.one)
        {
            escUI.transform.localScale = Vector3.one * minScale;
        }
        else if (scale == Vector3.one * minScale)
        {
            escUI.transform.localScale = Vector3.one;
        }
    }

    //{ ShutDownAllUIExpectEnding()
    // 다 끄고 게임 엔딩 UI만 킴
    public void ShutDownAllUIExpectEnding()
    {
        commonUI.SetActive(false);
        attackUI.SetActive(false);
        defenceUI.SetActive(false);
        rockSelectUI.SetActive(false);

        SwitchUIManager("endingUI");
    }
    //} ShutDownAllUIExpectEnding()

    #endregion
    //} SwitchUIManager()

    //{ PrintCard
    public void PrintRockCard(int id_, string name_, string explain_, float time_)
    {
        float maxTime = 0.1f;
        // 돌일때
        if (id_ <= 10)
        {
            cardClockImage.gameObject.SetActive(true);
            cardSandImage.gameObject.SetActive(true);
            cardNameTxt.text = name_;
            cardInfoTxt.text = explain_;
            cardGoldTxt.text = ""; // 출력 없음
            cardSandImage.fillAmount = (float)ConvertCoolTimeToEnum(time_) * maxTime;
            MatchIDToSprite(id_);
        }
    }
    //} PrintCard

    //{ PrintObstacleCard()
    public void PrintObstacleCard(int id_, string name_, string explain_, float gold_)
    {
        // 방해물일때
        if (id_ > 10)
        {
            cardNameTxt.text = name_;
            cardInfoTxt.text = explain_;
            cardGoldTxt.text = gold_.ToString();
            cardClockImage.gameObject.SetActive(false);
            cardSandImage.gameObject.SetActive(false);
            MatchIDToSprite(id_);
        }
    }
    //} PrintObstacleCard()

    //{ StoneTimer ConvertCoolTimeToEnum
    // 공 생성 쿨타임을 빠름/느림/보통으로 모래시계 프린트를 위한 Enum 변환기
    public StoneTimer ConvertCoolTimeToEnum(float time_)
    {
        float normalMaxTime = 35f;
        float normalMinTime = 30f;

        // 빠름
        if (time_ > normalMaxTime)
        {
            return StoneTimer.Fast;
        }
        // 보통
        else if (time_ >= normalMinTime && time_ <= normalMaxTime)
        {
            return StoneTimer.Normal;
        }
        // 느림
        else
        {
            return StoneTimer.Slow;
        }
    }
    //} StoneTimer ConvertCoolTimeToEnum

    //{ MatchIDToSprite()
    public void MatchIDToSprite(int id_)
    {
        if (id_ < 10)
        {
            int index = id_ - 1;
            cardImage.sprite = rockSprites[index];
        }
        if (id_ > 10)
        {
            int index = id_ - 11;
            cardImage.sprite = obstructionSprites[index];
        }
    }
    //} MatchIDToSprite()

    //{ MatchHolderIDSprite()
    public void MatchHolderIDSprite(Image image_, int id_)
    {
        if (id_ < 10)
        {
            int index = id_ - 1;
            image_.sprite = rockHolderSprites[index];
        }
        if (id_ > 10)
        {
            int index = id_ - 11;
            image_.sprite = obstructionSprites[index];
        }
    }
    //} MatchHolderIDSprite()

    //{ PrintReadyText()
    public void PrintReadyText()
    {
        readyTxt.text = "준비!";
        readyImg.gameObject.SetActive(true);
    }
    //} PrintReadyText()

    //{ PrintReadyText()
    public void PrintNotReadyText()
    {
        readyTxt.text = "준비중";
        readyImg.gameObject.SetActive(false);
    }
    //} PrintReadyText()

    //{ ShutDownUserSelectUI()
    public void ShutDownUserSelectUI()
    {
        userSelectUI.SetActive(false);
    }
    //} ShutDownUserSelectUI()
    #endregion

    public void InitnowOnUIList()
    {
        nowOnUI.Add(rockSelectUI);
        nowOnUI.Add(commonUI);
        nowOnUI.Add(attackUI);
        nowOnUI.Add(defenceUI);
    }

    public void SwitchUIForESCUI()
    {
        // 참이면
        if (CycleManager.cycleManager._isESCed == true)
        {
            List<GameObject> uiToRemove = new List<GameObject>();

            foreach (GameObject ui in nowOnUI)
            {
                // 켜져있지 않은 UI는 제거 대상에 추가
                if (ui.gameObject.transform.localScale != Vector3.one)
                {
                    uiToRemove.Add(ui);
                }
            }

            // 제거 대상 UI 삭제
            foreach (GameObject ui in uiToRemove)
            {
                nowOnUI.Remove(ui);
            }

            // 스케일 0.001로 설정
            foreach (GameObject ui in nowOnUI)
            {
                ui.gameObject.transform.localScale = Vector3.one * 0.001f;
            }
        }
        // 만약 esc가 풀린다면
        else if (CycleManager.cycleManager._isESCed == false)
        {
            // UI 복귀
            foreach (GameObject ui in nowOnUI)
            {
                ui.gameObject.transform.localScale = Vector3.one;
            }

            // 리스트 초기화
            nowOnUI.Clear();
            InitnowOnUIList();
        }
    }
}
