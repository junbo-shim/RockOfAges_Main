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
    private string playerIDCache;


    protected override void Awake()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            Debug.Log("2");
            Debug.Log(PhotonNetwork.IsConnected);
        }
        FindAllPanels();
    }

    protected override void Update()
    {

    }


    #region UI 오브젝트 찾아오는 메서드
    private void FindAllPanels() 
    {
        titlePanel = GameObject.Find("Panel_Title").transform;
        loginPanel = titlePanel.Find("Panel_Login").transform;
        signupPanel = titlePanel.Find("Panel_Signup").transform;
    }
    #endregion

    #region 빠른 시작-PlayFab
    public void StartQuick() 
    {
        Debug.Log("PlayFab authenticating using Custom ID...");

        var request = new LoginWithCustomIDRequest { CustomId = PlayFabSettings.DeviceUniqueIdentifier,
            CreateAccount = true };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
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
        Debug.Log("PlayFab authenticated. Requesting photon token...");

        playerIDCache = result.PlayFabId;
        var tokenRequest = new GetPhotonAuthenticationTokenRequest() 
        { PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime };

        PlayFabClientAPI.GetPhotonAuthenticationToken(tokenRequest, AuthenticateWithPhoton, OnLoginFailure);

        //Debug.LogFormat("로그인 성공");
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

    #region Photon Token 요청 및 인증-Photon
    private void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult tokenResult)
    {
        Debug.LogFormat("Photon token acquired: " + tokenResult.PhotonCustomAuthenticationToken + "  Authentication complete.");

        var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };
        
        customAuth.AddAuthParameter("username", playerIDCache);
        customAuth.AddAuthParameter("token", tokenResult.PhotonCustomAuthenticationToken);

        PhotonNetwork.AuthValues = customAuth;

        Debug.Log("1");
        Debug.Log(PhotonNetwork.IsConnected);
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("3");
        Debug.Log(PhotonNetwork.IsConnected);
        //Debug.Log(PhotonNetwork.ConnectUsingSettings());
    }
    #endregion
}
