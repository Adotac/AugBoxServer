using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.UI;
using Epic.OnlineServices.Ecom;
using Epic.OnlineServices.Logging;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PlayEveryWare.EpicOnlineServices;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


public class LoginMenu : MonoBehaviour
{
    
    public Text DemoTitle;
    [Header("Login type")]
    public Dropdown loginTypeDropdown;

    [Header("Host Port")] //Host adress or port
    public RectTransform idContainer;
    private UIConsoleInputField idInputField;

    [Header("TokenID")] //Username
    public RectTransform tokenContainer;
    private UIConsoleInputField tokenInputField;

    [Header("External Type")]
    public RectTransform connectTypeContainer;
    public Dropdown connectTypeDropdown;

    [Header("LoginBtn")]
    public Text loginButtonText;
    private string _OriginalloginButtonText;
    public Button loginButton;
    private Coroutine PreventLogIn = null;
    public Button logoutButton;

    [Header("Events")]
    public UnityEvent OnLogin;
    public UnityEvent OnLogout;

    [Header("Controller")]
    public GameObject UIFirstSelected;
    public GameObject UIFindSelectable;

    private EventSystem system;
    private GameObject selectedGameObject;

    LoginCredentialType loginType = LoginCredentialType.Developer;
    bool useConnectLogin = false;

    // Retain Id/Token inputs across scenes
    public static string IdGlobalCache = string.Empty;
    public static string TokenGlobalCache = string.Empty;


    private void Awake() {
        useConnectLogin = false;
    }

    private void CacheIdInputField(string value)
    {
        IdGlobalCache = value;
    }

    private void CacheTokenField(string value)
    {
        TokenGlobalCache = value;
    }

    public void OnDropdownChange(Dropdown down)
    {
        int value = down.value;

        useConnectLogin = false;
        switch (value)
        {
            case 1:
                loginType = LoginCredentialType.AccountPortal;
                break;
            case 2:
                loginType = LoginCredentialType.PersistentAuth;
                break;
            case 3:
                loginType = LoginCredentialType.ExternalAuth;
                break;
            case 4:
                //select unused type to avoid having to modify all loginType checks
                loginType = LoginCredentialType.Password;
                useConnectLogin = true;
                break;
            case 0:
            default:
                loginType = LoginCredentialType.Developer;
                break;
        }

        ConfigureUIForLogin();
    }

    private void Start() {
        loginButton.enabled = true;
        loginButton.gameObject.SetActive(true);

        idInputField = idContainer.GetComponentInChildren<UIConsoleInputField>();
        tokenInputField = tokenContainer.GetComponentInChildren<UIConsoleInputField>();

#if UNITY_EDITOR || (UNITY_STANDALONE_OSX && EOS_PREVIEW_PLATFORM) || UNITY_STANDALONE_WIN || (UNITY_STANDALONE_LINUX && EOS_PREVIEW_PLATFORM)
        idInputField.InputField.text = "localhost:7777"; //default on pc
#endif

        _OriginalloginButtonText = loginButtonText.text;
        InitConnectDropdown();

        OnDropdownChange(loginTypeDropdown);
        loginTypeDropdown.onValueChanged.AddListener(delegate {OnDropdownChange(loginTypeDropdown);} );
    }

    private void InitConnectDropdown()
    {
        List<Dropdown.OptionData> connectOptions = new List<Dropdown.OptionData>();

        List<ExternalCredentialType> credentialTypes = new List<ExternalCredentialType>
        {
            ExternalCredentialType.DeviceidAccessToken,
            //ExternalCredentialType.GogSessionTicket,
            //ExternalCredentialType.AppleIdToken,
            //ExternalCredentialType.GoogleIdToken,
            //ExternalCredentialType.OculusUseridNonce,
            //ExternalCredentialType.ItchioJwt,
            //ExternalCredentialType.ItchioKey,
            //ExternalCredentialType.AmazonAccessToken
        };

#if UNITY_STANDALONE
        credentialTypes.Add(ExternalCredentialType.SteamSessionTicket);
        credentialTypes.Add(ExternalCredentialType.SteamAppTicket);
        
#endif

#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
        credentialTypes.Add(ExternalCredentialType.DiscordAccessToken);
#endif

        foreach (var type in credentialTypes)
        {
            connectOptions.Add(new Dropdown.OptionData() { text = type.ToString() });
        }

        connectTypeDropdown.options = connectOptions;
    }

    private void ConfigureUIForLogin()
    {
        // if (OnLogout != null)
        // {
        //     OnLogout.Invoke();
        // }

        DemoTitle.gameObject.SetActive(true);
        loginTypeDropdown.gameObject.SetActive(true);

        loginButtonText.text = _OriginalloginButtonText;
        if (PreventLogIn != null)
            StopCoroutine(PreventLogIn);

        if (useConnectLogin)
        {
            ConfigureUIForConnectLogin();
        }
        else
        {
            switch (loginType)
            {
                case LoginCredentialType.AccountPortal:
                    ConfigureUIForAccountPortalLogin();
                    break;
                case LoginCredentialType.PersistentAuth:
                    ConfigureUIForPersistentLogin();
                    break;
                case LoginCredentialType.ExternalAuth:
                    ConfigureUIForExternalAuth();
                    break;
                case LoginCredentialType.Developer:
                default:
                    ConfigureUIForDevAuthLogin();
                    break;
            }
        }

        // Controller
        //EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(UIFirstSelected);
    }
    private void ConfigureUIForLogout()
    {
        DemoTitle.gameObject.SetActive(false);
        loginTypeDropdown.gameObject.SetActive(false);

        loginButton.gameObject.SetActive(false);
        logoutButton.gameObject.SetActive(true);

        idContainer.gameObject.SetActive(false);
        tokenContainer.gameObject.SetActive(false);
        connectTypeContainer.gameObject.SetActive(false);

        if (OnLogin != null)
        {
            OnLogin.Invoke();
        }
    }

    public void OnLogoutButtonClick()
    {
        if (EOSManager.Instance.GetLocalUserId() == null)
        {
            EOSManager.Instance.ClearConnectId(EOSManager.Instance.GetProductUserId());
            ConfigureUIForLogin();
            return;
        }

        EOSManager.Instance.StartLogout(EOSManager.Instance.GetLocalUserId(), (ref LogoutCallbackInfo data) => {
            if (data.ResultCode == Result.Success)
            {
                print("Logout Successful. [" + data.ResultCode + "]");
                ConfigureUIForLogin();
            }

        });
    }
    private void ConfigureUIForDevAuthLogin()
    {
        // loginTypeDropdown.value = loginTypeDropdown.options.FindIndex(option => option.text == "Dev Auth");

        if (!string.IsNullOrEmpty(IdGlobalCache))
        {
            idInputField.InputField.text = IdGlobalCache;
        }

        if (!string.IsNullOrEmpty(TokenGlobalCache))
        {
            tokenInputField.InputField.text = TokenGlobalCache;
        }

        DemoTitle.gameObject.SetActive(true);
        loginTypeDropdown.gameObject.SetActive(true);

        idContainer.gameObject.SetActive(true);

        connectTypeContainer.gameObject.SetActive(false);
        
        tokenContainer.gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(UIFirstSelected);

    }

    private void ConfigureUIForAccountPortalLogin()
    {
        DemoTitle.gameObject.SetActive(true);
        loginTypeDropdown.gameObject.SetActive(true);

        idContainer.gameObject.SetActive(true);

        connectTypeContainer.gameObject.SetActive(false);
        
        tokenContainer.gameObject.SetActive(false);

        loginButtonText.text = _OriginalloginButtonText;
        if (PreventLogIn != null)
            StopCoroutine(PreventLogIn);

        

    }

    private void ConfigureUIForPersistentLogin()
    {
        connectTypeContainer.gameObject.SetActive(false);

        idContainer.gameObject.SetActive(false);

        tokenContainer.gameObject.SetActive(false);
    }

    private void ConfigureUIForExternalAuth()
    {
        connectTypeContainer.gameObject.SetActive(false);
        idContainer.gameObject.SetActive(false);

        tokenContainer.gameObject.SetActive(false);
    }

    private void ConfigureUIForConnectLogin()
    {
        connectTypeContainer.gameObject.SetActive(true);
        idContainer.gameObject.SetActive(false);

        tokenContainer.gameObject.SetActive(false);
        

    }

    // For now, the only supported login type that requires a 'username' is the dev auth one
    bool SelectedLoginTypeRequiresUsername()
    {
        return loginType == LoginCredentialType.Developer;
    }

    // For now, the only supported login type that requires a 'password' is the dev auth one
    bool SelectedLoginTypeRequiresPassword()
    {
        return loginType == LoginCredentialType.Developer;
    }
    private IEnumerator TurnButtonOnAfter15Sec()
    {
        for (int i = 15; i >= 0; i--)
        {
            yield return new WaitForSecondsRealtime(1);
            loginButtonText.text = _OriginalloginButtonText + " (" + i + ")";
        }
        loginButton.enabled = true;
        loginButtonText.text = _OriginalloginButtonText;
    }
    private void StartLoginWithSteam()
    {
        var steamManager = SteamScript.SteamManager.Instance;
        string steamId = steamManager?.GetSteamID();
        string steamToken = steamManager?.GetSessionTicket();
        if(steamId == null)
        {
            Debug.LogError("ExternalAuth failed: Steam ID not valid");
        }
        else if (steamToken == null)
        {
            Debug.LogError("ExternalAuth failed: Steam session ticket not valid");
        }
        else
        {
            EOSManager.Instance.StartLoginWithLoginTypeAndToken(
                    LoginCredentialType.ExternalAuth,
                    ExternalCredentialType.SteamSessionTicket,
                    steamId,
                    steamToken,
                    StartLoginWithLoginTypeAndTokenCallback);
        }
    }
    public void OnLoginButtonClick()
    {
        string usernameAsString = idInputField.InputField.text.Trim();
        string passwordAsString = tokenInputField.InputField.text.Trim();

        if (SelectedLoginTypeRequiresUsername() && usernameAsString.Length <= 0)
        {
            print("Username is missing.");
            return;
        }

        if (SelectedLoginTypeRequiresPassword() && passwordAsString.Length <= 0)
        {
            print("Password is missing.");
            return;
        }

        loginButton.enabled = false;
        if(PreventLogIn!=null)
            StopCoroutine(PreventLogIn);
        PreventLogIn = StartCoroutine(TurnButtonOnAfter15Sec());
        print("Attempting to login...");

        // Disabled at the moment to work around a crash that happens
        //LoggingInterface.SetCallback((LogMessage logMessage) =>{
        //    print(logMessage.Message);
        //});

        if (useConnectLogin)
        {
           Debug.Log("Function not supported!!");
        }
        else if (loginType == LoginCredentialType.ExternalAuth)
        {
            StartLoginWithSteam();
        }
        else if (loginType == LoginCredentialType.PersistentAuth)
        {
            EOSManager.Instance.StartPersistentLogin((Epic.OnlineServices.Auth.LoginCallbackInfo callbackInfo) =>
            {
                // In this state, it means one needs to login in again with the previous login type, or a new one, as the
                // tokens are invalid
                if (callbackInfo.ResultCode != Epic.OnlineServices.Result.Success)
                {
                    print("Failed to login with Persistent token [" + callbackInfo.ResultCode + "]");
                    loginType = LoginCredentialType.Developer;
                    ConfigureUIForLogin();
                }
                else
                {
                    StartLoginWithLoginTypeAndTokenCallback(callbackInfo);
                }
            });
        }
        else
        {
            // Deal with other EOS log in issues
            EOSManager.Instance.StartLoginWithLoginTypeAndToken(loginType,
                                                                    usernameAsString,
                                                                    passwordAsString,
                                                                    StartLoginWithLoginTypeAndTokenCallback);
        }
    }

    //-------------------------------------------------------------------------
    private void StartConnectLoginWithLoginCallbackInfo(LoginCallbackInfo loginCallbackInfo)
    {
        EOSManager.Instance.StartConnectLoginWithEpicAccount(loginCallbackInfo.LocalUserId, (Epic.OnlineServices.Connect.LoginCallbackInfo connectLoginCallbackInfo) =>
        {
            if (connectLoginCallbackInfo.ResultCode == Result.Success)
            {
                print("Connect Login Successful. [" + connectLoginCallbackInfo.ResultCode + "]");
                ConfigureUIForLogout();
            }
            else if (connectLoginCallbackInfo.ResultCode == Result.InvalidUser)
            {
                // ask user if they want to connect; sample assumes they do
                EOSManager.Instance.CreateConnectUserWithContinuanceToken(connectLoginCallbackInfo.ContinuanceToken, (Epic.OnlineServices.Connect.CreateUserCallbackInfo createUserCallbackInfo) =>
                {
                    print("Creating new connect user");
                    EOSManager.Instance.StartConnectLoginWithEpicAccount(loginCallbackInfo.LocalUserId, (Epic.OnlineServices.Connect.LoginCallbackInfo retryConnectLoginCallbackInfo) =>
                    {
                        if (retryConnectLoginCallbackInfo.ResultCode == Result.Success)
                        {
                            ConfigureUIForLogout();
                        }
                        else
                        {
                            // For any other error, re-enable the login procedure
                            ConfigureUIForLogin();
                        }
                    });
                });
            }
        });
    }

    //-------------------------------------------------------------------------//

    public void StartLoginWithLoginTypeAndTokenCallback(LoginCallbackInfo loginCallbackInfo)
    {
        if (loginCallbackInfo.ResultCode == Epic.OnlineServices.Result.AuthMFARequired)
        {
            // collect MFA
            // do something to give the MFA to the SDK
            print("MFA Authentication not supported in sample. [" + loginCallbackInfo.ResultCode + "]");
        }
        else if (loginCallbackInfo.ResultCode == Result.AuthPinGrantCode)
        {
            ///TODO(mendsley): Handle pin-grant in a more reasonable way
            Debug.LogError("------------PIN GRANT------------");
            Debug.LogError("External account is not connected to an Epic Account. Use link below");
            Debug.LogError($"URL: {loginCallbackInfo.PinGrantInfo?.VerificationURI}");
            Debug.LogError($"CODE: {loginCallbackInfo.PinGrantInfo?.UserCode}");
            Debug.LogError("---------------------------------");
        }
        else if (loginCallbackInfo.ResultCode == Epic.OnlineServices.Result.Success)
        {
            StartConnectLoginWithLoginCallbackInfo(loginCallbackInfo);
        }
        else if (loginCallbackInfo.ResultCode == Epic.OnlineServices.Result.InvalidUser)
        {
            print("Trying Auth link with external account: " + loginCallbackInfo.ContinuanceToken);
            EOSManager.Instance.AuthLinkExternalAccountWithContinuanceToken(loginCallbackInfo.ContinuanceToken,
                                                                            LinkAccountFlags.NoFlags,
                                                                            (Epic.OnlineServices.Auth.LinkAccountCallbackInfo linkAccountCallbackInfo) =>
                                                                            {
                                                                                StartConnectLoginWithLoginCallbackInfo(loginCallbackInfo);
                                                                            });
        }

        else
        {
            print("Error logging in. [" + loginCallbackInfo.ResultCode + "]");
        }

        // Re-enable the login button and associated UI on any error
        if (loginCallbackInfo.ResultCode != Epic.OnlineServices.Result.Success)
        {
            ConfigureUIForLogin();
        }
    }

    public void OnExitButtonClick()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }
}