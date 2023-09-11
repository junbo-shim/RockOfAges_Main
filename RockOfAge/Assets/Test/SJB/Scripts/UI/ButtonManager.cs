using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonManager : GlobalSingleton<ButtonManager>
{
    private Transform titlePanel;

    private Button optionButton;
    private Button quickStartButton;
    private Button loginButton;
    private Button signupButton;
    private Button quitButton;

    private Button startButton;
    private Button resetPWButton;
    private Button closeButton;

    private Button registerButton;
    private Button closeButton2;
    
    protected override void Awake()
    {
        titlePanel = GameObject.Find("Panel_Title").transform;
        FindAllButtons();
        SetAllButton();
        MakePanelsDefault();
    }




    #region 타이틀 씬의 모든 버튼을 찾아서 저장하는 메서드
    private void FindAllButtons() 
    {
        optionButton = titlePanel.Find("Button_Option").GetComponent<Button>();
        quickStartButton = titlePanel.Find("Button_QuickStart").GetComponent<Button>();
        loginButton = titlePanel.Find("Button_Login").GetComponent<Button>();
        signupButton = titlePanel.Find("Button_Signup").GetComponent<Button>();
        quitButton = titlePanel.Find("Button_Quit").GetComponent<Button>();

        startButton = titlePanel.Find("Panel_Login").Find("Button_Start").GetComponent<Button>();
        resetPWButton = titlePanel.Find("Panel_Login").Find("Button_ResetPW").GetComponent<Button>();
        closeButton = titlePanel.Find("Panel_Login").Find("Button_Close").GetComponent<Button>();

        registerButton = titlePanel.Find("Panel_Signup").Find("Button_Register").GetComponent<Button>();
        closeButton2 = titlePanel.Find("Panel_Signup").Find("Button_Close2").GetComponent<Button>();
    }
    #endregion




    #region 버튼에 AddListener 세팅하는 메서드
    private void SetAllButton() 
    {
        optionButton.onClick.AddListener(PressOption);
        quickStartButton.onClick.AddListener(PressQuickStart);
        loginButton.onClick.AddListener(PressLogin);
        signupButton.onClick.AddListener(PressSignup);
        quitButton.onClick.AddListener(PressQuit);

        startButton.onClick.AddListener(PressStart);
        resetPWButton.onClick.AddListener(PressResetPW);
        closeButton.onClick.AddListener(PressClose);

        registerButton.onClick.AddListener(PressRegister);
        closeButton2.onClick.AddListener(PressClose2);
    }
    #endregion




    #region 모든 창을 작게 만드는 메서드
    public void MakePanelsDefault() 
    {
        Transform loginPanel = titlePanel.Find("Panel_Login").transform;
        Transform signupPanel = titlePanel.Find("Panel_Signup").transform;

        loginPanel.localScale = Vector3.zero;
        signupPanel.localScale = Vector3.zero;
    }
    #endregion





    #region 버튼 기능 메서드
    public void PressOption() 
    {
        /*Empty For Now*/
    }

    public void PressQuickStart() 
    {
        /*임의의 Auth 부여*/
    }

    public void PressLogin() 
    {
        Transform loginPanel = titlePanel.Find("Panel_Login").transform;
        loginPanel.localScale = Vector3.one;
    }

    public void PressSignup() 
    {
        Transform signupPanel = titlePanel.Find("Panel_Signup").transform;
        signupPanel.localScale = Vector3.one;
    }

    public void PressQuit() 
    {
        if (UnityEditor.EditorApplication.isPlaying == true) 
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else 
        {
            Application.Quit();
        }
    }

    public void PressStart() 
    {
        NetworkManager.Instance.Login();
    }

    public void PressResetPW() 
    {
        /**/
    }

    public void PressRegister() 
    {
        NetworkManager.Instance.Register();
    }

    public void PressClose()
    {
        Transform loginPanel = titlePanel.Find("Panel_Login").transform;
        loginPanel.localScale = Vector3.zero;

        TMP_InputField emailInput = loginPanel.Find("InputField_Email").GetComponent<TMP_InputField>();
        TMP_InputField passwordInput = loginPanel.Find("InputField_Password").GetComponent<TMP_InputField>();

        emailInput.text = default;
        passwordInput.text = default;
    }

    public void PressClose2()
    {
        Transform signupPanel = titlePanel.Find("Panel_Signup").transform;
        signupPanel.localScale = Vector3.zero;

        TMP_InputField emailInput = signupPanel.Find("InputField_Email").GetComponent<TMP_InputField>();
        TMP_InputField passwordInput = signupPanel.Find("InputField_Password").GetComponent<TMP_InputField>();
        TMP_InputField nicknameInput = signupPanel.Find("InputField_Nickname").GetComponent<TMP_InputField>();

        emailInput.text = default;
        passwordInput.text = default;
        nicknameInput.text = default;
    }
    #endregion
}