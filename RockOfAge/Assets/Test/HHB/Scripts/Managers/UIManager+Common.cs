using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Common
public partial class UIManager: MonoBehaviour
{
    #region 변수
    public GameObject commonUI;
    // M키 누른시간과 bool 변수
    private bool _mButtonPressed = false;
    private float _pressedTime = 0f;
    // 플레이어 이름
    public TextMeshProUGUI player1Txt;
    public TextMeshProUGUI player2Txt;
    public TextMeshProUGUI player3Txt;
    public TextMeshProUGUI player4Txt;
    // 플레이어 체력
    public Image team1HpImg;
    public Image team2HpImg;
    // 플레이어 이미지
    public Image player1Img;
    public Image player2Img;
    public Image player3Img;
    public Image player4Img;
    // 플레이어 체력
    public TextMeshProUGUI playerGold;
    #endregion


    #region Functions
    //! 스케일 조정으로 바꾸기
    //{ TurnOnCommonUI()
    // commonUI 켜는 함수
    public void TurnOnCommonUI()
    {
        commonUI.transform.localScale = Vector3.one;
    }
    //} TurnOnCommonUI()

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

    //! 서버
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

    //! 서버
    //{ PrintTeamHP()
    // 팀1 과 팀2의 체력을 출력하는 함수
    public void PrintTeamHP()
    {
        //team1HpImg.fillAmount = CycleManager.cycleManager.   /teamMaxHp;
        //team2HpImg.fillAmount = CycleManager.cycleManager.   /teamMaxHp;
    }
    //} PrintTeamHP()

    //! 서버
    //{ PrintMyGold()
    public void PrintMyGold()
    {
        //playerGold.text = 
    }
    //} PrintMyGold()
    #endregion
}
