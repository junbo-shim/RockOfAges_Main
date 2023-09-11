using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

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
    // Card
    public Sprite[] rockSprites;
    public Sprite[] obstructionSprites;
    public Image cardImage;
    public Image cardSandImage;
    public Image cardClockImage;
    public TextMeshProUGUI cardNameTxt;
    public TextMeshProUGUI cardInfoTxt;
    public TextMeshProUGUI cardGoldTxt;
    // Holder
    public Sprite[] rockHolderSprites;
    #endregion

    private void Awake()
    {
        uiManager = this;
    }

    #region Functions
    //{ PrintCard
    public void PrintRockCard(int id_, string name_, string explain_, float time_)
    {
        float maxTime = 0.1f;
        // 돌일때
        if (id_ <= 10)
        {
            cardNameTxt.text = name_;
            cardInfoTxt.text = explain_;
            cardGoldTxt.text = ""; // 출력 없음
            cardSandImage.fillAmount = (float)ConvertCoolTimeToEnum(time_) * maxTime;
            MatchIDToSprite(id_);
        }
    }
    //} PrintCard

    //{ PrintObstacleCard()
    // TODO
    public void PrintObstacleCard(int id_, string name_, string explain_, float gold_)
    { 
    
    }
    //} PrintObstacleCard()

    //{ StoneTimer ConvertCoolTimeToEnum
    public StoneTimer ConvertCoolTimeToEnum(float time_)
    {
        float normalMaxTime = 65f;
        float normalMinTime = 55f;

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
            int index = id_ - 10;
            cardImage.sprite = obstructionSprites[index];
        }
    }
    //} MatchIDToSprite()

    public void MatchHolderIDSprite(Image image_, int id_)
    {
        if (id_ < 10)
        {
            int index = id_ - 1;
            image_.sprite = rockHolderSprites[index];
        }
        if (id_ > 10)
        {
            int index = id_ - 10;
            image_.sprite = obstructionSprites[index];
        }
    }


    #endregion
}
