using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Common
public partial class UIManager: MonoBehaviour
{
    #region 변수
    [Header("UI")]
    public GameObject commonUI;
    //// ! Photon
    //public GameObject mirrorUI;
    // M키 누른시간과 bool 변수
    private bool _mButtonPressed = false;
    private float _pressedTime = 0f;
    // 플레이어 이름
    [Header("PLAYER NAME TEXT")]
    public TextMeshProUGUI player1Txt;
    public TextMeshProUGUI player2Txt;
    public TextMeshProUGUI player3Txt;
    public TextMeshProUGUI player4Txt;
    // 플레이어 체력
    [Header("TEAM HP IMG")]
    public Image team1HpImg;
    public Image team2HpImg;
    // 플레이어 이미지
    [Header("PLAYER IMG")]
    public Image player1Img;
    public Image player2Img;
    public Image player3Img;
    public Image player4Img;
    // 플레이어 체력
    [Header("PLAYER GOLD TEXT")]
    public TextMeshProUGUI playerGold;
    #endregion

    #region Functions
    //{ GetRotationKey()
    // M 1초 누르면 카메라 켜짐
    public void GetRotationKey()
    {
        if (Input.GetKey(KeyCode.M) == true)
        {
            if (_mButtonPressed == false)
            {
                _mButtonPressed = true;
                _pressedTime = Time.time;
            }
        }
        else
        {
            _mButtonPressed = false;
        }

        if (_mButtonPressed && (Time.time - _pressedTime) >= 1f)
        {
            _mButtonPressed = false;
            _pressedTime = 0f;
            RotateMirror();
        }
    }
    //} GetRotationKey()

    //{ RotateMirror()
    // 미러 카메라를 불러오는 함수
    public void RotateMirror()
    {
        // 상대가 공 굴리고 있을 때 카메라 연동과 함께(포톤처리)
        MirrorRotate mirrorRotate = FindObjectOfType<MirrorRotate>();
        mirrorRotate.RotateMirror();
    }
    //} RotateMirror()

    // ! 서버
    //{ PrintPlayerText()
    // 플레이어 이름 출력하는 함수
    public void PrintPlayerText(string player1_, string player2_, string player3_, string player4_)
    { 
        player1Txt.text = player1_;
        player2Txt.text = player2_;
        player3Txt.text = player3_;
        player4Txt.text = player4_;
    }
    //} PrintPlayerText()

    //! 서버
    // 플레이어 이미지 출력하는 함수
    //{ PrintPlayerImg()
    public void PrintPlayerImg(Image playerImg1_, Image playerImg2_, Image playerImg3_, Image playerImg4_)
    { 
        
    }
    //} PrintPlayerImg()

    //{ PrintTeamHP()
    // 팀1 과 팀2의 체력을 출력하는 함수
    public void PrintTeamHP()
    {
        team1HpImg.fillAmount = CycleManager.cycleManager.team1Hp / 600f;
        team2HpImg.fillAmount = CycleManager.cycleManager.team2Hp / 600f;
    }
    //} PrintTeamHP()

    //! 서버
    //{ PrintMyGold()
    public void PrintMyGold(float playerGold_)
    {
        int intGold = (int)playerGold_;
        playerGold.text = intGold.ToString();
    }
    //} PrintMyGold()
    #endregion
}
