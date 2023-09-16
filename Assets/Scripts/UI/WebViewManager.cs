using Gpm.WebView;
using System.Collections.Generic;
using Ratic_Kit.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class WebViewManager : MonoBehaviour
{

    [SerializeField] private GameObject _closeGO;
    [SerializeField] private WebViewHandler _webViewHandler;

    public Color badkgroundColorYemek;
    public Color badkgroundColorDefault;

    public Image ImgBackground;

    public GameObject InitScreen;
    public GameObject TournamentScreen;

    public GameObject ButtonBackView;



    #region UI

    public void GoToTournament()
    {
        InitScreen.SetActive(false);
        TournamentScreen.SetActive(true);
    }

    public void BackButton()
    {
        InitScreen.SetActive(true);
        TournamentScreen.SetActive(false);
    }

    #endregion

    #region Monobehaviours

    private void Awake()
    {
        Singleton();
    }

    #endregion

    #region Singleton

    public static WebViewManager instance;

    private void Singleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion


    public void OpenWebViewViaUrlYemeksepeti()
    {
        //_webViewHandler.OpenWebViewViaUrl("https://ratic.co/yemeksepeti/?username=salvador&email=ali@ratic.io");
        _webViewHandler.OpenWebViewViaUrl("https://ratic.co/turkcell/?username=salvador&email=ali@ratic.io");
        //GpmWebView.ShowUrl("https://ratic.co/yemeksepeti/?username=salvador&email=ali@ratic.io", GetConfiguration(), OnWebViewCallback, new List<string> { "test-scheme" });

        ButtonBackView.SetActive(true);

    }

    public void OpenWebView(string url)
    {
        if (string.IsNullOrEmpty(url) == false)
        {
            //_closeGO.SetActive(true);
            // GpmWebView.ShowUrl(url, GetConfiguration(), OnWebViewCallback, new List<string> { "test-scheme" });
            _webViewHandler.OpenWebViewViaUrl(url);
        }
        else
        {
            Debug.LogError("[SampleWebView] Input url is empty.");
        }
    }


    private void OnWebViewCallback(GpmWebViewCallback.CallbackType callbackType, string data, GpmWebViewError error)
    {
        Debug.Log("OnWebViewCallback: " + callbackType);
        switch (callbackType)
        {
            case GpmWebViewCallback.CallbackType.Open:

                //ImgBackground.color = badkgroundColorYemek;

                //ButtonBackView.SetActive(true);

                if (error != null)
                {
                    Debug.LogFormat("Fail to open WebView. Error:{0}", error);

                }
                break;
            case GpmWebViewCallback.CallbackType.Close:

                //ButtonBackView.SetActive(false);

                //ImgBackground.color = badkgroundColorDefault;

                if (error != null)
                {
                    Debug.LogFormat("Fail to close WebView. Error:{0}", error);
                }
                break;
            case GpmWebViewCallback.CallbackType.PageStarted:
                if (string.IsNullOrEmpty(data) == false)
                {
                    Debug.LogFormat("PageStarted Url : {0}", data);
                }
                break;
            case GpmWebViewCallback.CallbackType.PageLoad:
                if (string.IsNullOrEmpty(data) == false)
                {
                    Debug.LogFormat("Loaded Page:{0}", data);
                }
                break;
            case GpmWebViewCallback.CallbackType.MultiWindowOpen:
                Debug.Log("MultiWindowOpen");
                break;
            case GpmWebViewCallback.CallbackType.MultiWindowClose:
                Debug.Log("MultiWindowClose");
                break;
            case GpmWebViewCallback.CallbackType.Scheme:
                Debug.LogFormat("Scheme:{0}", data);
                break;
            case GpmWebViewCallback.CallbackType.GoBack:
                Debug.Log("GoBack");
                break;
            case GpmWebViewCallback.CallbackType.GoForward:
                Debug.Log("GoForward");
                break;
            case GpmWebViewCallback.CallbackType.ExecuteJavascript:
                Debug.LogFormat("ExecuteJavascript data : {0}, error : {1}", data, error);
                break;
        }

    }


    public void Close()
    {
        Debug.Log("Close bny");

        if (_webViewHandler.webViewObject != null)
        {
            ButtonBackView.SetActive(false);
            _closeGO.SetActive(false);
            Destroy(_webViewHandler.webViewObject.gameObject);
        }
    }


    private GpmWebViewRequest.Configuration GetConfiguration()
    {
        GpmWebViewRequest.CustomSchemePostCommand customSchemePostCommand = new GpmWebViewRequest.CustomSchemePostCommand();
        customSchemePostCommand.Close("CUSTOM_SCHEME_POST_CLOSE");

        return new GpmWebViewRequest.Configuration()
        {
            style = GpmWebViewStyle.POPUP,
            isClearCache = true,
            isClearCookie = true,
            backgroundColor = "#FF2B85",
            title = "     ",
            navigationBarColor = "#FF2B85",
            isNavigationBarVisible = true,
            isBackButtonVisible = true,
            isForwardButtonVisible = true,
            supportMultipleWindows = true,

            //addJavascript
            customSchemePostCommand = customSchemePostCommand,

            position = GetConfigurationPosition(),
            size = GetConfigurationSize(),
            margins = GetConfigurationMargins(),

#if UNITY_IOS
            contentMode = GpmWebViewContentMode.MOBILE,
            isMaskViewVisible = false,
            isAutoRotation = true
#endif
        };
    }


    #region Helpers


    private GpmWebViewRequest.Position GetConfigurationPosition()
    {
        return new GpmWebViewRequest.Position
        {
            hasValue = true,
            x = 0,
            y = 0
        };
    }

    private GpmWebViewRequest.Size GetConfigurationSize()
    {
        return new GpmWebViewRequest.Size
        {
            hasValue = true,
            //width = (int)(Screen.height * 0.8f),
            width = (int)(Screen.height * 1f),
            //height = (int)(Screen.height * 0.8f)
            height = (int)(Screen.height * 1f)
        };
    }

    private GpmWebViewRequest.Margins GetConfigurationMargins()
    {
        return new GpmWebViewRequest.Margins
        {
            hasValue = true,
            left = (int)(Screen.width*0.03f),
            //top = (int)(Screen.height * 0.15f),
            top = (int)(Screen.height*0.05f),
            right = (int)(Screen.width*0.03f),
            bottom = 0
        };
    }

    #endregion


}
