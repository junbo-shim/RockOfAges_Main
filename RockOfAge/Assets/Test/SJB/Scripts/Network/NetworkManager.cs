using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System;


public class NetworkManager : GlobalSingleton<NetworkManager>
{
    private Transform titlePanel;
    private Transform loginPanel;
    private Transform signupPanel;


    protected override void Awake()
    {
        FindAllPanels();
    }

    protected override void Update()
    {

    }

    private void FindAllPanels() 
    {
        titlePanel = GameObject.Find("Panel_Title").transform;
        loginPanel = titlePanel.Find("Panel_Login").transform;
        signupPanel = titlePanel.Find("Panel_Signup").transform;
    }






    #region 빠른 시작-PlayFab
    public void StartQuick() 
    {
        
    }
    #endregion

    #region 로그인-PlayFab
    public void Login()
    {
        TMP_InputField emailInput = loginPanel.Find("InputField_Email").GetComponent<TMP_InputField>();
        TMP_InputField passwordInput = loginPanel.Find("InputField_Password").GetComponent<TMP_InputField>();

        var request = new LoginWithEmailAddressRequest { Email = emailInput.text, 
            Password = passwordInput.text };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result) 
    {
        Debug.LogFormat("로그인 성공");
        loginPanel.GetComponentInChildren<TMP_Text>().color = Color.green;
        loginPanel.GetComponentInChildren<TMP_Text>().text = "로그인 성공";
    }

    private void OnLoginFailure(PlayFabError error) 
    {
        Debug.LogFormat("로그인 실패\n오류 코드 : {0}", error);
        loginPanel.GetComponentInChildren<TMP_Text>().color = Color.red;
        loginPanel.GetComponentInChildren<TMP_Text>().text = "로그인 실패, 오류 코드 : " + error;
    }
    #endregion

    #region Email 회원가입-PlayFab
    public void Register() 
    {
        TMP_InputField emailInput = signupPanel.Find("InputField_Email").GetComponent<TMP_InputField>();
        TMP_InputField passwordInput = signupPanel.Find("InputField_Password").GetComponent<TMP_InputField>();
        TMP_InputField nicknameInput = signupPanel.Find("InputField_Nickname").GetComponent<TMP_InputField>();

        var request = new RegisterPlayFabUserRequest { Email = emailInput.text, 
            Password = passwordInput.text, Username = nicknameInput.text, DisplayName = nicknameInput.text };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result) 
    {
        Debug.LogFormat("계정 등록 성공");
        signupPanel.GetComponentInChildren<TMP_Text>().color = Color.green;
        signupPanel.GetComponentInChildren<TMP_Text>().text = "계정 등록 성공";
    }

    private void OnRegisterFailure(PlayFabError error) 
    {
        Debug.LogFormat("계정 등록 실패\n오류 코드 : {0}", error);
        signupPanel.GetComponentInChildren<TMP_Text>().color = Color.red;
        signupPanel.GetComponentInChildren<TMP_Text>().text = "계정 등록 실패, 오류 코드 : " +  error;
    }
    #endregion
}
